using Microsoft.Win32;
using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;


namespace SharpTorrent.MVVM.Models;

internal class SharpTorrentModel
{
    public static ClientEngine Engine = new();

    public Guid Id { get; }
    public string TorrentName { get; private set; }
    //public Task? Task { get; set; }
    public Task? Task { get; set; }
    public LinkedList<string> Last10Messages { get; private set; }

    public TorrentManager? Manager { get; private set; }

    public string DownloadFile { get; private set; }
    public string SaveDirectory { get; private set; }

    public SharpTorrentModel(Guid id)
    {
        Id = id;

        Last10Messages = new();
        DownloadFile = @"C:\Users\Egxr41k\Desktop\TorrentSamples\Anno-1404-RePack-ot-xatab (1).torrent";
        SaveDirectory = @"C:\Users\Egxr41k\Desktop\TorrentSamples\Anno-1404-RePack-ot-xatab (1)";
        TorrentName = "Anno 1404 Gold Edition by xatab";
        var files = new DirectoryInfo(SaveDirectory + "\\" + TorrentName).GetFiles();

        foreach (FileInfo file in files)
        {
            file.Delete();
        }

        //not final version of ManagerInitAsync
        Task.Run(async () =>
        {
            Manager = await Engine.AddAsync(
                await Torrent.LoadAsync(
                    DownloadFile),
                SaveDirectory,
                new TorrentSettingsBuilder()
                    .ToSettings());

            #region events
            Manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
            };

            Manager.PeerConnected += (o, e) =>
            {
                lock (Last10Messages)
                    AddMessage($"Connection succeeded: {e.Peer.Uri}");
            };

            Manager.ConnectionAttemptFailed += (o, e) =>
            {
                lock (Last10Messages)
                    AddMessage(
                        $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
            };

            Manager.PieceHashed += delegate (object? o, PieceHashedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
            };

            Manager.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"OldState: {e.OldState} NewState: {e.NewState}");
            };

            Manager.TrackerManager.AnnounceComplete += (sender, e) =>
            {
                AddMessage($"{e.Successful}: {e.Tracker}");
            };
            #endregion
        }).Wait();

    }
    public SharpTorrentModel()
    {
        Id = Guid.NewGuid();
        Last10Messages = new();

        PathsInit();

        //not final version of ManagerInitAsync
        Task.Run(async () =>
        {
            Manager = await Engine.AddAsync(
                await Torrent.LoadAsync(
                    DownloadFile),
                SaveDirectory,
                new TorrentSettingsBuilder()
                    .ToSettings());

            #region events
            Manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
            };

            Manager.PeerConnected += (o, e) =>
            {
                lock (Last10Messages)
                    AddMessage($"Connection succeeded: {e.Peer.Uri}");
            };

            Manager.ConnectionAttemptFailed += (o, e) =>
            {
                lock (Last10Messages)
                    AddMessage(
                        $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
            };

            Manager.PieceHashed += delegate (object? o, PieceHashedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
            };

            Manager.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"OldState: {e.OldState} NewState: {e.NewState}");
            };

            Manager.TrackerManager.AnnounceComplete += (sender, e) =>
            {
                AddMessage($"{e.Successful}: {e.Tracker}");
            };
            #endregion
        }).Wait();
        
    }

    //ADD BUGS-PROTECTION
    private void PathsInit() 
    {
        OpenFileDialog ofd = new()
        {
            Filter = "Torrent Files(*.torrent)|*.torrent"
        };

        if (ofd.ShowDialog() == true)
        {
            DownloadFile = ofd.FileName;

            TorrentName = DownloadFile.Split('\\').Last().Split('.').First();


            string dirPath = Path.GetDirectoryName(ofd.FileName) ??
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            SaveDirectory = Path.Combine(dirPath + "\\" + TorrentName);

            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
        }
        
    }

    // NEED FOR DEBUGING
    public async Task<TorrentManager> ManagerInitAsync()
    {
        Manager = await Engine.AddAsync(
            await Torrent.LoadAsync(
                DownloadFile),
            SaveDirectory,
            new TorrentSettingsBuilder()
                .ToSettings());


        
        #region events

        Manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
        {
            lock (Last10Messages)
                AddMessage($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
        };

        Manager.PeerConnected += (o, e) =>
        {
            lock (Last10Messages)
                AddMessage($"Connection succeeded: {e.Peer.Uri}");
        };

        Manager.ConnectionAttemptFailed += (o, e) =>
        {
            lock (Last10Messages)
                AddMessage(
                    $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
        };

        Manager.PieceHashed += delegate (object? o, PieceHashedEventArgs e)
        {
            lock (Last10Messages)
                AddMessage($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
        };

        Manager.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e)
        {
            lock (Last10Messages)
                AddMessage($"OldState: {e.OldState} NewState: {e.NewState}");
        };

        Manager.TrackerManager.AnnounceComplete += (sender, e) =>
        {
            AddMessage($"{e.Successful}: {e.Tracker}");
        };
        #endregion

        return Manager;
    }

    public void AddMessage(string message)
    {
        lock (Last10Messages)
        {
            if (Last10Messages.Count >= 10)
                Last10Messages.RemoveFirst();

            Last10Messages.AddLast(message);
        }
    }

    public async Task<string> GetCurrentInfo(StringBuilder output)
    {
        output.Remove(0, output.Length);
        output.AppendLine($"Transfer Rate:      {Engine.TotalDownloadRate / 1024.0:0.00}kB/sec ↓ / {Engine.TotalUploadRate / 1024.0:0.00}kB/sec ↑");
        output.AppendLine($"Memory Cache:       {Engine.DiskManager.CacheBytesUsed / 1024.0:0.00}/{Engine.Settings.DiskCacheBytes / 1024.0:0.00} kB");
        output.AppendLine($"Disk IO Rate:       {Engine.DiskManager.ReadRate / 1024.0:0.00} kB/s read / {Engine.DiskManager.WriteRate / 1024.0:0.00} kB/s write");
        output.AppendLine($"Disk IO Total:      {Engine.DiskManager.TotalBytesRead / 1024.0:0.00} kB read / {Engine.DiskManager.TotalBytesWritten / 1024.0:0.00} kB written");
        output.AppendLine($"Open Connections:   {Engine.ConnectionManager.OpenConnections}");

        // Print out the port mappings
        foreach (var mapping in Engine.PortMappings.Created)
            output.AppendLine($"Successful Mapping    {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
        foreach (var mapping in Engine.PortMappings.Failed)
            output.AppendLine($"Failed mapping:       {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
        foreach (var mapping in Engine.PortMappings.Pending)
            output.AppendLine($"Pending mapping:      {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
        output.AppendLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
        output.AppendLine($"State:              {Manager.State}");
        output.AppendLine($"Name:               {(Manager.Torrent == null ? "MetaDataMode" : Manager.Torrent.Name)}");
        output.AppendLine($"Progress:           {Manager.Progress:0.00}%");
        output.AppendLine($"Transferred:        {Manager.Monitor.DataBytesReceived / 1024.0 / 1024.0:0.00} MB ↓ / {Manager.Monitor.DataBytesSent / 1024.0 / 1024.0:0.00} MB ↑");
        output.AppendLine($"Tracker Status");
        foreach (var tier in Manager.TrackerManager.Tiers)
            output.AppendLine($"\t{tier.ActiveTracker} : Announce Succeeded: {tier.LastAnnounceSucceeded}. Scrape Succeeded: {tier.LastScrapeSucceeded}.");

        if (Manager.PieceManager != null)
            output.AppendFormat("Current Requests:   {0}", await Manager.PieceManager.CurrentRequestCountAsync());

        var peers = await Manager.GetPeersAsync();
        output.AppendLine();
        output.AppendLine("Outgoing:");
        foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Outgoing))
        {
            output.AppendFormat("\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                        p.Monitor.DownloadRate / 1024.0,
                                                                        p.AmRequestingPiecesCount,
                                                                        p.Monitor.UploadRate / 1024.0,
                                                                        p.EncryptionType,
                                                                        string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
        }
        output.AppendLine();
        output.AppendLine("Incoming:");
        foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Incoming))
        {
            output.AppendFormat("\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                        p.Monitor.DownloadRate / 1024.0,
                                                                        p.AmRequestingPiecesCount,
                                                                        p.Monitor.UploadRate / 1024.0,
                                                                        p.EncryptionType,
                                                                        string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
        }

        output.AppendLine();
        if (Manager.Torrent != null)
            foreach (var file in Manager.Files)
                output.AppendFormat("{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);

        return output.ToString();
    }



}
