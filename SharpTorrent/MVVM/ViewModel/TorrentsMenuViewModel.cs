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
    internal class TorrentsMenuViewModel : Base.ViewModel
    {
        public Base.Command AddNewCommand { get; set; }

        private ObservableCollection<ListBoxItem> activeTorrents = new();
        public ObservableCollection<ListBoxItem> ActiveTorrents
        {
            get => activeTorrents;
            set => Set(ref activeTorrents, value);
        }

        private MainViewModel MainVM
        {
            get => (MainViewModel)MainViewModel.MainWindow.DataContext;
        }

        private ListBoxItem selectedItem;
        public ListBoxItem SelectedItem
        {
            get => selectedItem;
            set
            {
                Set(ref selectedItem, value);
                MainVM.CurrentView = value.DataContext;
            }
        }

        List<TorrentViewModel> TorrentVMs = new();

        private void AddNewActiveTorrent(TorrentManager manager)
        {
            TorrentVMs.Add(new TorrentViewModel());

            ActiveTorrents.Add(new ListBoxItem
            {
                DataContext = TorrentVMs.Last(),
            });
        }

        private void TestMenuInit()
        {
            // пока остановимся здесь.
            // оснваная задача на ближайшие пару дней -
            // детальная прорабока визуальной части, 
            // а миенно наполнение елементов списка меню
            // повезет, если удастаться это сделать до конца недели,
            // это будет означать то что я движусь чуть быстрее чем я расчитовал.

            for (int i = 0; i < 10; i++)
            {
                TorrentVMs.Add(new TorrentViewModel
                {
                    IsActive = true,
                    ProgressBarValue = Convert.ToInt32($"{i}0"),
                    TorrentName = $"torrent number {i}"
                });

                ActiveTorrents.Add(new ListBoxItem
                {
                    DataContext = TorrentVMs.Last(),
                });

            #region ListBoxItem content init
            //var converter = new BrushConverter();
            //var content = new TextBlock
            //{
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    FontSize = 12.0,
            //    Height = 47,
            //    Foreground = Brushes.White,
            //    //Text = "{Binding TorrentName}"\
            //    Text = $"torrent number {i}",
            //};
            #endregion
            }            
        }


        public TorrentsMenuViewModel()
        {
            TestMenuInit();
            
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
