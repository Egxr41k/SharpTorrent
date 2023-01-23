using MonoTorrent;
using SharpTorrent.MVVM.Model;
using SharpTorrent.MVVM.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SharpTorrent.MVVM.ViewModels
{
    internal class MainViewModel : Base.ViewModel
    {
        public SharpTorrentViewModel SharpTorrentViewModel { get; set; }
        public MainViewModel(SharpTorrentViewModel sharpTorrentViewModel)
        {
            SharpTorrentViewModel = sharpTorrentViewModel;
            
        }
    }
}
