﻿using CommunityToolkit.Mvvm.DependencyInjection;
using KozmoTech.ZenUtility.ChecksumHasher.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application, IMainThreadDispatcher
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Ioc.Default.ConfigureServices(ConfigureServices());
        InitializeComponent();
    }

    public MainWindow? MainWindow { get; private set; }

    public void Dispatch(Action worker) => MainWindow?.DispatcherQueue.TryEnqueue(new DispatcherQueueHandler(worker));

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
    }

    private IServiceProvider ConfigureServices() => new ServiceCollection()
        .AddSingleton<AppSettingsViewModel>()
        .AddSingleton<FileHasherViewModel>()
        .AddSingleton<IMainThreadDispatcher>(this)
        .BuildServiceProvider();
}
