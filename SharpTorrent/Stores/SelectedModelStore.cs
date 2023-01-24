using System;

namespace SharpTorrent.Stores;

internal class SelectedModelStore
{
    private readonly SharpTorrentStore _sharpTorrentStore;

    private SharpTorrentModel _selectedModel;
    public SharpTorrentModel SelectedModel
    {
        get => _selectedModel;
        set
        {
            _selectedModel = value;
            SelectedModelChanged?.Invoke();
        }
    }

    public Action SelectedModelChanged;

    public SelectedModelStore(SharpTorrentStore sharpTorrentStore)
    {
        _sharpTorrentStore = sharpTorrentStore;
        _sharpTorrentStore.TorrentUpdated += _sharpTorrentStore_TorrentUpdated;
    }

    private void _sharpTorrentStore_TorrentUpdated(SharpTorrentModel model)
    {
        if (model.Id == SelectedModel?.Id)
        {
            SelectedModel = model;
        }
    }
}
