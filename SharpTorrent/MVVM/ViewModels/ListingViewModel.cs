using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpTorrent.MVVM.ViewModels;
internal class ListingViewModel : Base.ViewModel
{
    public Commands.Base.Command AddNewCommand { get; set; }

    private ObservableCollection<ListingItemViewModel> _activeTorrents;
    public IEnumerable<ListingItemViewModel> ActiveTorrents => _activeTorrents;

    public SelectedModelStore _selectedModelStore { get; }
    private readonly SharpTorrentStore _sharpTorrentStore;

    private ListingItemViewModel _selectedListingItemViewModel;
    public ListingItemViewModel SelectedListingItemViewModel
    {
        get => _selectedListingItemViewModel;
        set
        {
            Set(ref _selectedListingItemViewModel, value);
            _selectedModelStore.SelectedModel = _selectedListingItemViewModel?.SharpTorrentModel;
        }
    }

    public ListingViewModel(SharpTorrentStore sharpTorrentStore, SelectedModelStore selectedModelStore )
    {
        _sharpTorrentStore = sharpTorrentStore;
        _selectedModelStore = selectedModelStore;
        _activeTorrents = new ObservableCollection<ListingItemViewModel>();


        _sharpTorrentStore.TorrentAdded += _sharpTorrentStore_TorrentAdded;
        _sharpTorrentStore.TorrentUpdated += _sharpTorrentStore_TorrentUpdated;
        AddNewCommand = new AddTorrentCommand(_sharpTorrentStore, this);

    }

    private void _sharpTorrentStore_TorrentUpdated(SharpTorrentModel model)
    {
        ListingItemViewModel? listingItemViewModel =
            _activeTorrents.FirstOrDefault(y => y.SharpTorrentModel.Id == model.Id);
    }

    private void _sharpTorrentStore_TorrentAdded(SharpTorrentModel model)
    {
        AddListItem(model);
    }

    private void AddListItem(SharpTorrentModel model)
    {
        //ICommand editCommand = new OpenEditListItemCommand(model, _modalNavigationStore);
        _activeTorrents.Add(
            new ListingItemViewModel(model, _sharpTorrentStore));
    }
}
