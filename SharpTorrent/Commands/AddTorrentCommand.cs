using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpTorrent.Commands;

internal class AddTorrentCommand : Base.Command
{
    private readonly SharpTorrentStore _sharpTorrentStore;
    private readonly ListingViewModel _listingViewModel;

    public AddTorrentCommand(SharpTorrentStore sharpTorrentStore, ListingViewModel listingViewModel)
    {
        _sharpTorrentStore = sharpTorrentStore;
        _listingViewModel = listingViewModel;
    }

    public override void Execute(object? parameter)
    {
        SharpTorrentModel newtorrent = new();
        //var manager = newtorrent.ManagerInit().Result;

        try { _sharpTorrentStore.Add(newtorrent); }
        catch (Exception) { }

        _listingViewModel.SelectedListingItemViewModel =
            _listingViewModel.ActiveTorrents.Last();
    }
}
