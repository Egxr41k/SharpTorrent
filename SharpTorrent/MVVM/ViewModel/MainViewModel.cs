using Microsoft.Win32;
using MonoTorrent;
using MonoTorrent.Client;
using SharpTorrent.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class MainViewModel : Base.ViewModel
    {
        public HomeViewModel HomeVM { get; set; }
        public Base.Command OpenFileCommand { get; set; }
        public Base.Command AddNewTorrent { get; set; }


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

        public void TorrentsPreviwInit()
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
        }
    }
}
