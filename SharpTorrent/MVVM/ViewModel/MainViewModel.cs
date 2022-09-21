using Microsoft.Win32;
using MonoTorrent;
using MonoTorrent.Client;
using SharpTorrent.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class MainViewModel : Base.ViewModel
    {
        public HomeViewModel HomeVM { get; set; }
        public Base.Command OpenFileCommand { get; set; }
        public Base.Command AddNewTorrent { get; set; }
        public Base.Command? MaximizeCommand { get; set; }
        public Base.Command? MinimizeCommand { get; set; }
        public Base.Command? CloseCommand { get; set; }

        #region Icons
        public BitmapImage MinimizeIcon { get => minimizeIcon; }
        private BitmapImage minimizeIcon;

        public BitmapImage MaximizeIcon { get => maximizeIcon; }
        private BitmapImage maximizeIcon;

        public BitmapImage CloseIcon { get => closeIcon; }
        private BitmapImage closeIcon;

        #endregion
        private static Window MainWindow
        {
            get => Application.Current.MainWindow;
        }

        

        private void AppCommandsInit()
        {
            MinimizeCommand = new(o =>
            {
                MainWindow.WindowState = WindowState.Minimized;
            });

            MaximizeCommand = new(o =>
            {
                if (MainWindow.WindowState == WindowState.Normal)
                    MainWindow.WindowState = WindowState.Maximized;
                else MainWindow.WindowState = WindowState.Normal;
            });

            CloseCommand = new(o => MainWindow.Close());
        }


        private string active = "No Active torrents yet";
        public string Active
        {
            get => active;
            set => Set(ref active, value);
        }

        private string stoped = "No Stoped torrents yet";
        public string Stoped
        {
            get => stoped;
            set => Set(ref stoped, value);
        }

        private ObservableCollection<ListBoxItem> activeTorrents = new();
        public ObservableCollection<ListBoxItem> ActiveTorrents
        {
            get => activeTorrents;
            set => Set(ref activeTorrents, value);
        }


        private ObservableCollection<ListBoxItem> stopedTorrents = new();
        public ObservableCollection<ListBoxItem> StopedTorrents
        {
            get => stopedTorrents;
            set => Set(ref stopedTorrents, value);
        }

        private object currentView;
        public object CurrentView
        {
            get => currentView;
            set => Set(ref currentView, value);
        }

        private void TorrentsPreviwInit()
        {
            foreach (TorrentManager manager in MainModel.Engine.Torrents)
            {
                ActiveTorrents.Add(
                    new ListBoxItem
                    {
                        DataContext = new TorrentViewModel
                        {
                            IsActive = true,
                            TorrentName = manager.Name,
                            TorrentPath = manager.SavePath,
                            ProgressBarValue = (int)manager.Progress
                        }
                    }
                );
            }
        }
        private void IconsInit()
        {
            string iconsDirectory = MainModel.RESOURCEPATH + "\\Icons";
            string[] iconsFiles = Directory.GetFiles(iconsDirectory);
            BitmapImage[] icons =
            {
                CloseIcon,
                MaximizeIcon,
                MinimizeIcon,
            };
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i] = new BitmapImage(new Uri(iconsFiles[i]));
            }
        }

        public MainViewModel()
        {
            HomeVM = new();
            currentView = HomeVM;

            OpenFileCommand = new Base.Command(/*async*/ o =>
            {
                OpenFileDialog ofd = new()
                {
                    Filter = "Torrent Files(*.torrent)|*.torrent"
                };

                if (ofd.ShowDialog() == true)
                {
                    //await Downloader.DownloadAsync(ofd.FileName);
                }
            });

            AddNewTorrent = new Base.Command(o =>
            {

            });

            IconsInit();
        }
    }
}
