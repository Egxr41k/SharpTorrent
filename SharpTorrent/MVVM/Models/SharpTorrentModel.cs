using System;

namespace SharpTorrent.MVVM.Models;

internal class SharpTorrentModel
{
    public Guid Id { get; }
    public string TorrentName { get; }
    public bool IsActive { get; }
    public SharpTorrentModel
        (Guid id, string torrentName, bool isActive)
    {
        Id = id;
        TorrentName = torrentName;
        IsActive = isActive;
    }
}
