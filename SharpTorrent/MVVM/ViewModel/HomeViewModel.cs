using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class HomeViewModel : Base.ViewModel
    {
        public Base.Command AddNewCommand { get; set; }

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
    }
}
