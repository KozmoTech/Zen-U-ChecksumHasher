using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KozmoTech.CoreFx.System;
using System.Buffers;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

public sealed partial class FileHasherViewModel : ObservableObject, IDisposable
{
    public void Dispose()
    {
        Hashers.ForEach(h => h.Dispose());
        ComputeProgress.Dispose();
    }

    [ObservableProperty]
    private FileInfoViewModel? fileInfo = null;

    /// <summary>
    /// Get a collection of all supported hashers. Use <see cref="HashCalculatorViewModel.IsEnabled"/> to control whether
    /// a specific hasher should participate in the calculation.
    /// </summary>
    public ReadOnlyCollection<HashCalculatorViewModel> Hashers { get; } =
        new List<HashCalculatorViewModel>
        {
            new(HashAlgorithmType.MD5),
            new(HashAlgorithmType.SHA1) { IsEnabled = false },
            new(HashAlgorithmType.SHA256),
            new(HashAlgorithmType.SHA384) { IsEnabled = false },
            new(HashAlgorithmType.SHA512),
        }.AsReadOnly();

    /// <summary>
    /// Get the current progress of hashcode calculation or verification.
    /// </summary>
    public ProgressViewModel ComputeProgress { get; } = new();

    /// <summary>
    /// Compute the hashcode(s) of <see cref="FileInfo"/> for all <see cref="Hashers"/> that are <see cref="HashCalculatorViewModel.IsEnabled"/>.
    /// </summary>
    /// <param name="cancellation">Token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task represents the asynchronous hashcode calculation operation.</returns>
    [RelayCommand]
    internal async Task ComputeAllHashesAsync(CancellationToken cancellation = default)
    {
        await using (ComputeProgress.StartNewOperation())
        {
#if DEBUG
            var watch = Stopwatch.StartNew();
#endif
            var hashers = Hashers.Where(h => h.IsEnabled).ToArray();
            await ComputeAllHashesAsync(hashers, ComputeProgress.PercentageProgress, cancellation);
            ComputeProgress.Complete();
#if DEBUG
            watch.Stop();
            Debug.WriteLine($"ComputeAllHashes Time: {watch.Elapsed}");
#endif
        }
    }

    /// <summary>
    /// Concurrently compute the checksum(s) of <see cref="FileInfo"/> using <paramref name="hashers"/>.
    /// </summary>
    /// <param name="hashers">A collection of hashers that will be used to compute the checksums.</param>
    /// <param name="progress">An <see cref="IProgress{T}"/> instance to report the progress in percentage (from 0% to 100%).</param>
    /// <param name="cancellation">Token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task represents the asynchronous hashcode calculation operation.</returns>
    /// <remarks>
    /// Traditionally, we execute the following steps in sequence to compute the checksums:
    /// <list type="number">
    /// <item>Read a block of bytes from <see cref="FileInfo"/></item>
    /// <item>Append the block to all <paramref name="hashers"/></item>
    /// <item>Read a block of bytes from <see cref="FileInfo"/></item>
    /// <item>Append the block to all <paramref name="hashers"/></item>
    /// <item>...</item>
    /// </list>
    /// But this is not optimal. In my implementation, I run <b>step 2 and step 3 in parallel</b> because their data flow does not overlap.
    /// In exchange, I need to allocate one more temporary buffer to hold the next block.
    /// </remarks>
    private async Task ComputeAllHashesAsync(IReadOnlyCollection<HashCalculatorViewModel> hashers, IProgress<double> progress, CancellationToken cancellation = default)
    {
        if (FileInfo is null)
            return;
        if (hashers.Count == 0)
            return;

        ComputeProgress.StageProgress.Report(HashingProgressStage.Preparing);

        hashers.ForEach(h => h.Reset());
        using var input = await ((IContentProvider)FileInfo).CreateContentReaderAsync();
        byte[] cur = new byte[ContentReadBufferSize], nxt = new byte[ContentReadBufferSize];

        ComputeProgress.StageProgress.Report(HashingProgressStage.Calculating);
        long totalReadN = 0;
        for (var readN = await input.ReadAsync(cur.AsMemory(), cancellation); readN > 0; (cur, nxt) = (nxt, cur))
        {
            totalReadN += readN;
            var block = new ArraySegment<byte>(cur, 0, readN);
            await Task.WhenAll(ReadNextInputAsync(), AppendBlockToHashersAsync());
            progress.Report((double)totalReadN / (FileInfo.TotalLength ?? (ulong)input.Length));

            async Task ReadNextInputAsync() =>
                readN = await input.ReadAsync(nxt.AsMemory(), cancellation).ConfigureAwait(false);

            Task AppendBlockToHashersAsync() =>
                Parallel.ForEachAsync(hashers, (h, _) =>
                {
                    h.AppendBlock(block);
                    return ValueTask.CompletedTask;
                });
        }

        hashers.ForEach(h => h.Finish());
    }

    private const int ContentReadBufferSize = 65536;
}
