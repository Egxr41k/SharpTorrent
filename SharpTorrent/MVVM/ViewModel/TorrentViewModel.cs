using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class TorrentViewModel : Base.ViewModel
    {
        public bool IsActive { get; set; }

        public string TorrentName { get; set; }
        public string TorrentPath { get; set; }

        public int ProgressBarValue
        {
            get => progrssBarValue;
            set => Set(ref progrssBarValue, value);
        }
        public int progrssBarValue;

    }
}
