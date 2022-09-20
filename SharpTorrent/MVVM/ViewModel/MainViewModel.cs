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
        public Base.Command OpenFileCommand { get; set; }


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

        public ObservableCollection<ListBoxItem> torrentListItems = new();

        public void TorrentsPreviwInit()
        {
            foreach (TorrentManager manager in MainModel.Engine.Torrents)
            {
                var TorrentVM = new TorrentViewModel
                {
                    IsActive = true,
                    //TorrentName = torrent...
                    //TorrentPath = torrent...
                };

                torrentListItems.Add(new ListBoxItem
                {
                    DataContext = TorrentVM
                });

            }
        }

        public MainViewModel()
        {

            OpenFileCommand = new Base.Command(/*async*/ o =>
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Text Files(*.txt)|*.txt"
                };

                if (ofd.ShowDialog() == true)
                {
                    //await Downloader.DownloadAsync(ofd.FileName);
                }
            });

        }
    }
}
