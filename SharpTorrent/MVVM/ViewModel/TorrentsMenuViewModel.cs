using Microsoft.Win32;
using MonoTorrent;
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

        private ObservableCollection<ListBoxItem> activeTorrents = new();
        public ObservableCollection<ListBoxItem> ActiveTorrents
        {
            get => activeTorrents;
            set => Set(ref activeTorrents, value);
        }

        private ListBoxItem selectedItem;
        public ListBoxItem SelectedItem
        {
            get => selectedItem;
            set
            {
                Set(ref selectedItem, value);
                foreach(TorrentViewModel vm in TorrentVMs)
                    if (selectedItem.DataContext == vm)
                        MainViewModel.currentView = vm;
            }
            
        }

        List<TorrentViewModel> TorrentVMs = new();

        public void ShowTorrent(TorrentManager manager)
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

            #region Test area
            for (int i = 0; i < 10; i++)
            {
                TorrentVMs.Add(new TorrentViewModel
                {
                    IsActive = true,
                    ProgressBarValue = Convert.ToInt32($"{i}0"),
                    TorrentName = $"torrent number {i}"
                });

                #region ListBoxItem content init
                var converter = new BrushConverter();
                var content = new StackPanel().Children;

                content.Add(new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontSize = 12.0,
                    Height = 47,
                    Foreground = Brushes.White,
                    //Text = "{Binding TorrentName}"\
                    Text = $"torrent number {i}",
                });

                content.Add(new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    //Value = "{Binding ProgressBarValue}"
                    Value = Convert.ToInt32($"{i}0"),
                    Foreground = /*(Brush)new BrushConverter().ConvertFrom("#00ff00")*/
                    Brushes.Green,
                    Height = 1,
                    Background = /*(Brush)converter.ConvertFrom("#453838")*/
                    Brushes.DarkGray,
                    BorderThickness = new Thickness(0)
                });
                #endregion

                ActiveTorrents.Add(new ListBoxItem
                {
                    DataContext = TorrentVMs[i],
                    Content = content
                });

            }
            #endregion


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
                    string dirPath = Path.GetDirectoryName(ofd.FileName) ?? "";

                    string SaveDirectory = CreateDorectoryByName(dirName, dirPath);

                    if (!Directory.Exists(SaveDirectory))
                        Directory.CreateDirectory(SaveDirectory);

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

        private string GetFileName(string path) =>
            path.Split('\\').Last().Split('.').First();

        private string CreateDorectoryByName(string name, string path) =>
            Path.Combine(path + "\\" + name);
    }
}
