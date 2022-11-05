using Microsoft.Win32;
using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class TorrentsMenuViewModel : Base.ViewModel
    {
        public Base.Command AddNewCommand { get; set; }


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

        List<TorrentViewModel> TorrentVMs = new();

        private ListBoxItem selectedItem;

        public ListBoxItem SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, value);
        }


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
        public TorrentsMenuViewModel()
        {
            AddNewCommand = new Base.Command(o =>
            {
                //AddNewActiveTorrent();
                //AddNewTorrentView view = new();
                //view.Show();
                OpenFileDialog ofd = new()
                {
                    Filter = "Torrent Files(*.torrent)|*.torrent"
                };

                if (ofd.ShowDialog() == true)
                {
                    // need to create the folder with name of name opened file

                    var DownloadFile = ofd.FileName;        // return full path to the File

                    string dirName = DownloadFile
                                    .Split('\\').Last() // return the file name with extension
                                    .Split('.').First();// return file name without extension


                    string directoryPath = Path.Combine(
                                           Path.GetDirectoryName(ofd.FileName) + // return the path of parrent directory
                                           "\\" +
                                           dirName);

                    Directory.CreateDirectory(directoryPath);

                    var SaveDirectory = directoryPath; // the end
                    //Task<TorrentManager> task;
                    try
                    {
                        //var task = MainModel.DownloadAsync(DownloadFile, SaveDirectory);
                        //Thread.Sleep(1000);
                        //AddNewActiveTorrent(task.Result);
                    }
                    catch (Exception) { }
                }

            });

        }
    }
}
