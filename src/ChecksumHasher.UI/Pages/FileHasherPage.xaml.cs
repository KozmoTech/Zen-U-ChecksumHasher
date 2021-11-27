using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// A page used to calculate and compare file checksums.
/// </summary>
public sealed partial class FileHasherPage : Page
{
    public FileHasherPage()
    {
        InitializeComponent();
        SortedHashers = new(ViewModel.Hashers, true)
        {
            SortDescriptions =
            {
                new SortDescription(nameof(HashCalculatorViewModel.Algorithm), SortDirection.Ascending),
            },
        };
    }

    public FileHasherViewModel ViewModel => ViewModelLocator.FileHasherViewModel;

    public AdvancedCollectionView SortedHashers { get; }
}
