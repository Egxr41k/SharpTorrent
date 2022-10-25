using Microsoft.Win32;
using SharpTorrent.MVVM.Model;
using SharpTorrent.MVVM.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class AddNewTorrentViewModel : Base.ViewModel
    {

        public Base.Command OKCommand { get; set; }
        public Base.Command OpenFileCommand { get; set; }

        public bool IsAdded = false;

        private bool isEnabled = false;
        public bool IsEnabled
        {
            get => isEnabled;
            set => Set(ref isEnabled, value);
        }

        private string downloadFile = "downloadFile";
        public string DownloadFile
        {
            get => downloadFile;
            set => Set(ref downloadFile, value);
        }

        private string saveDirectory = "saveDirectory";
        public string SaveDirectory
        {
            get => saveDirectory;
            set => Set(ref saveDirectory, value);
        }
        public AddNewTorrentViewModel()
        {
            OKCommand = new(o =>
            {
                IsAdded = true;
                try
                {
                    MainModel.DownloadAsync(downloadFile, saveDirectory);
                }
                catch (Exception ) { }
            });

            OpenFileCommand = new Base.Command(/*async*/ o =>
            {
                OpenFileDialog ofd = new()
                {
                    Filter = "Torrent Files(*.torrent)|*.torrent"
                };

                if (ofd.ShowDialog() == true)
                {
                    downloadFile = Path.GetFileName(ofd.FileName);
                    saveDirectory = Path.GetDirectoryName(ofd.FileName);
                    isEnabled = true;
                }


            });
        }
    }
}
