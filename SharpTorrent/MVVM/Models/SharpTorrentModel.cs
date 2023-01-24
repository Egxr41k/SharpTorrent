using Microsoft.Win32;
using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Connections;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTorrent.MVVM.Models;

internal class SharpTorrentModel
{
    public static ClientEngine Engine = new();

    public Guid Id { get; }
    public string TorrentName { get; private set; }
    //public bool IsActive { get; }

    public StringBuilder Output;
    public TorrentManager Manager;

    public string TorrentPath { get; private set; }
    public string DownloadPath { get; private set; }

    
    //public SharpTorrentModel()
    //{
    //    Id = Guid.NewGuid();
    //    //TorrentName = torrentName;
    //    //IsActive = isActive;
    //}

    public SharpTorrentModel(Guid id, string torrentName, bool v2)
    {
        Id = id;
        TorrentName = torrentName;
        Output = new StringBuilder().Append($"{TorrentName} id is {id}");
    }

    private void PathsInit()
    {
        OpenFileDialog ofd = new()
        {
            Filter = "Torrent Files(*.torrent)|*.torrent"
        };

        if (ofd.ShowDialog() == true)
        {
            string DownloadFile = ofd.FileName;

            string dirName = DownloadFile.Split('\\').Last().Split('.').First();

            TorrentName = dirName;


            string dirPath = Path.GetDirectoryName(ofd.FileName) ??
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string SaveDirectory = Path.Combine(dirPath + "\\" + dirName);

            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
        }
        
    }

    private async Task<TorrentManager> ManagerInit()
    {
        TorrentManager Manager;

        try
        {
            var settingsBuilder = new TorrentSettingsBuilder();

            Manager = await Engine.AddAsync(
                await Torrent.LoadAsync(TorrentPath),
                DownloadPath,
                new TorrentSettingsBuilder().ToSettings());
        }
        catch (Exception e) { Console.WriteLine("Couldn't decode file. " + e.Message); }

        Manager = Engine.Torrents[0];

        #region events
        //Manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
        //{
        //    lock (Listener)
        //        Listener.WriteLine($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
        //};

        //Manager.PeerConnected += (o, e) =>
        //{
        //    lock (Listener)
        //        Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
        //};

        //Manager.ConnectionAttemptFailed += (o, e) =>
        //{
        //    lock (Listener)
        //        Listener.WriteLine(
        //            $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
        //};

        //Manager.PieceHashed += delegate (object? o, PieceHashedEventArgs e)
        //{
        //    lock (Listener)
        //        Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
        //};

        //Manager.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e)
        //{
        //    lock (Listener)
        //        Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
        //};

        //Manager.TrackerManager.AnnounceComplete += (sender, e) =>
        //{
        //    Listener.WriteLine($"{e.Successful}: {e.Tracker}");
        //};
        #endregion

        //await Manager.StartAsync();
        return Manager;
    }
}
