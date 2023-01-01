using SharpTorrent.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class TorrentViewModel : Base.ViewModel
    {
        public TextRange Text
        {
            get => text;
            set => Set(ref text, value);
        }
        private TextRange text;

        public bool IsActive { get; set; }

        public string TorrentName
        {
            get => torrentName;
            set => Set(ref torrentName, value);
        }
        private string torrentName = "1";
        public string TorrentPath { get; set; }

        public int ProgressBarValue
        {
            get => progrssBarValue;
            set => Set(ref progrssBarValue, value);
        }
        private int progrssBarValue;

        public TorrentViewModel()
        {
            var manager = MainModel.Manager;
            //TorrentName = MainModel.Manager.Name;
            //ProgrssBarValue = MainModel.Manager.BitField.PercentComplete;
        }

    }
}
