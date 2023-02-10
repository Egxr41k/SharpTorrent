using System;

namespace SharpTorrent.Stores;

internal class SharpTorrentStore
{
    public event Action<SharpTorrentModel>? TorrentAdded;
    public void Add(SharpTorrentModel model)
    {
        TorrentAdded?.Invoke(model);
    }

    public event Action<SharpTorrentModel>? TorrentUpdated;
    public void Update(SharpTorrentModel model)
    {
        TorrentUpdated?.Invoke(model);
    }

}
