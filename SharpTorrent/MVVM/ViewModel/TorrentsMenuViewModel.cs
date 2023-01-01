using Microsoft.Win32;
using MonoTorrent.Client;
using SharpTorrent.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class TorrentModel
    {
        public TorrentModel(string name, int precentComplete, bool isActive)
        {
            IsActive = isActive;
            TorrentName = name;
            ProgressBarValue = precentComplete;
        }

        public bool IsActive { get; private set; }
        public string TorrentName { get; private set; }
        public int ProgressBarValue { get; private set; }
    }

    internal class TorrentsMenuViewModel : Base.ViewModel
    {
        public Base.Command AddNewCommand { get; set; }

        private ObservableCollection<TorrentViewModel> activeTorrents = new();
        public ObservableCollection<TorrentViewModel> ActiveTorrents
        {
            get => activeTorrents;
            set => Set(ref activeTorrents, value);
        }

        private MainViewModel MainVM = (MainViewModel)MainViewModel.MainWindow.DataContext;
        

        private TorrentViewModel selectedItem;
        TorrentViewModel torrentVM = new();
        public TorrentViewModel SelectedItem
        {
            get => selectedItem;
            set
            {

                Set(ref selectedItem, value);
                if (MainVM.CurrentView is HomeViewModel)
                {
                    torrentVM = new();
                    MainVM.CurrentView = torrentVM;
                }
                else 
                {
                    torrentVM.TorrentName = value.TorrentName;
                    torrentVM.ProgressBarValue = value.ProgressBarValue;
                    torrentVM.IsActive = true;
                };
                MainVM.CurrentView = torrentVM;
                
            }
        }

        List<TorrentViewModel> TorrentVMs = new();

        private void AddNewActiveTorrent(TorrentManager manager)
        {
            //TorrentVMs.Add(new TorrentViewModel());
        }

        public static List<TorrentModel> torrentModels = new();
        private void TestMenuInit(int itemsCount)
        {
            //torrentModels = new TorrentModel[itemsCount];
            for (int i = 0; i < itemsCount; i++)
            {
                //torrentModels.Add(new($"torrent number {i}", Convert.ToInt32($"{i}0"), true));
                
                //TorrentVMs.Add(new TorrentViewModel());

                ActiveTorrents.Add(new TorrentViewModel()
                {
                    TorrentName = $"torrent number {i}",
                    ProgressBarValue = Convert.ToInt32($"{i}0"),
                    IsActive = true,
                });

            }            
        }


        public TorrentsMenuViewModel()
        {
            TestMenuInit(10);
            
            //if (MainVM != null)
            //{
            //    var TorrentVm = new TorrentViewModel();
            //    TorrentVm.IsActive = true;
            //    TorrentVm.ProgressBarValue = Convert.ToInt32($"{50}");
            //    TorrentVm.TorrentName = $"sucсsess";
            //    MainVM.CurrentView = TorrentVm;
            //}

            AddNewCommand = new Base.Command(o =>
            {

                OpenFileDialog ofd = new()
                {
                    Filter = "Torrent Files(*.torrent)|*.torrent"
                };

                if (ofd.ShowDialog() == true)
                {
                    string DownloadFile = ofd.FileName;

                    string dirName = GetFileName(DownloadFile);
                    string dirPath = Path.GetDirectoryName(ofd.FileName) ??
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    string SaveDirectory = Path.Combine(dirPath + "\\" + dirName);

                    if (!Directory.Exists(SaveDirectory))
                        Directory.CreateDirectory(SaveDirectory);

                    try
                    {
                        StartDownload(DownloadFile, SaveDirectory);
                    }
                    catch (Exception) { }
                }


            });
        }

        private void StartDownload(string file, string directory)
        {
            //это юудет следущим этапом, пока поработает над визуалом,
            //благо теперь вся логика приложения храниться раздельно в еще одном файле
            Task task1 = new Task(() =>
            {
                var task = MainModel.DownloadAsync(file, directory);
            });
            task1.Wait();

            AddNewActiveTorrent(MainModel.Manager);
            MainModel.Manager.StartAsync();
        }
        private static string GetFileName(string path) =>
            path.Split('\\').Last().Split('.').First();

    }
}
