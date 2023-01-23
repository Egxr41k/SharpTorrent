using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTorrent.MVVM.ViewModels
{
    internal class SharpTorrentViewModel : Base.ViewModel
    {
        public HomeViewModel HomeVM { get; set; }
        public TorrentsMenuViewModel TorrentsMenuVM { get; set; }

        private object currentView;
        public object CurrentView
        {
            get => currentView;
            set => Set(ref currentView, value);
        }
        public SharpTorrentViewModel()
        {
            TorrentsMenuVM = new TorrentsMenuViewModel();
            CurrentView = new HomeViewModel();
        }
    }
}
