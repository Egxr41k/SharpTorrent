using System;
using System.Threading.Tasks;

namespace SharpTorrent.Stores;

internal class SharpTorrentStore
{
    public event Action<SharpTorrentModel> TorrentAdded;
    public async Task Add(SharpTorrentModel model)
    {
        TorrentAdded?.Invoke(model);
    }

    public event Action<SharpTorrentModel> TorrentUpdated;
    public async Task Update(SharpTorrentModel model)
    {
        TorrentUpdated?.Invoke(model);
    }

}
