using Microsoft.Win32;
using MonoTorrent;
using MonoTorrent.Client;
using SharpTorrent.MVVM.Model;
using SharpTorrent.MVVM.View;
using System;
using System.Collections.Generic;
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
        public Base.Command? MaximizeCommand { get; set; }
        public Base.Command? MinimizeCommand { get; set; }
        public Base.Command? CloseCommand { get; set; }
        public Base.Command AddNewCommand { get; set; }

        #region Icons
        public BitmapImage MinimizeIcon
        {
            get => minimizeIcon;
            set => Set(ref minimizeIcon, value);
        }
        private BitmapImage minimizeIcon;

        public BitmapImage MaximizeIcon
        {
            get => maximizeIcon;
            set => Set(ref maximizeIcon, value);
        }
        private BitmapImage maximizeIcon;

        public BitmapImage CloseIcon
        {
            get => closeIcon;
            set => Set(ref closeIcon, value);
        }
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

        static List<TorrentViewModel> TorrentVMs;

        public void AddNewActiveTorrent(TorrentManager manager)
        {
            TorrentVMs.Add(new TorrentViewModel
            {
                IsActive = true,
                TorrentName = manager.Name,
                TorrentPath = manager.SavePath,
                ProgressBarValue = (int)manager.Progress
            });

            ActiveTorrents.Add(new ListBoxItem
            {
                DataContext = TorrentVMs[^1],
            });

        }
        private void IconsInit()
        {
            string IconsDirectory = MainModel.RESOURCEPATH + "\\Icons";
            string[] IconsFiles = Directory.GetFiles(IconsDirectory);

            closeIcon = new BitmapImage(new Uri(IconsFiles[0]));
            maximizeIcon = new BitmapImage(new Uri(IconsFiles[1]));
            minimizeIcon = new BitmapImage(new Uri(IconsFiles[2]));
        }

        public MainViewModel()
        {
            HomeVM = new();
            currentView = HomeVM;

            AddNewCommand = new Base.Command(o =>
            {
                //AddNewActiveTorrent();
                AddNewTorrentView view = new();
                view.Show();
            });

            IconsInit();
            AppCommandsInit();
        }
    }
}
