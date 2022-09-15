using Microsoft.Win32;
using SharpTorrent.MVVM.Model;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

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
