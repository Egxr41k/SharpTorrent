using SharpTorrent.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class TorrentModel
    {
        public bool IsActive { get; set; }
        public string TorrentName { get; set; }
        public int ProgressBarValue { get; set; }
    }

    internal class TorrentViewModel : Base.ViewModel
    {
        TorrentModel model;
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

        public void SetData(string name, int precentComplete, bool isActive)
        {
            IsActive = isActive;
            TorrentName = name;
            ProgressBarValue = precentComplete;
        }

        public TorrentViewModel()
        {
            model = TorrentsMenuViewModel.CurrentModel;

            IsActive = model.IsActive;
            TorrentName = model.TorrentName;
            ProgressBarValue = model.ProgressBarValue;
            
        }

    }
}
