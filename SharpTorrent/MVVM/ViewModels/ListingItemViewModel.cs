using SharpTorrent.MVVM.Models;
using SharpTorrent.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpTorrent.MVVM.ViewModels
{
    internal class ListingItemViewModel : Base.ViewModel
    {
        public SharpTorrentModel SharpTorrentModel { get; private set; }

        public string TorrentName => SharpTorrentModel.TorrentName;

        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ListingItemViewModel(SharpTorrentModel model, SharpTorrentStore sharpTorrentStore)
        {
            SharpTorrentModel = model;
        }


        internal void Update(SharpTorrentModel model)
        {
            SharpTorrentModel = model;

            OnpropertyChanged(nameof(SharpTorrentModel));
        }
    }
}
