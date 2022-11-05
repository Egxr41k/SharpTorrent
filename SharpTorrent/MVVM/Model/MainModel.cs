using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace SharpTorrent.MVVM.Model;
static class MainModel
{
    public static string RESOURCEPATH =
        Path.GetFullPath(
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\Resources"));

    public static Top10Listener Listener = new(10);
    public static ClientEngine Engine = new();
    public static StringBuilder Output = new(1024);
    public static TorrentManager Manager;


    public static async void DownloadAsync(
        string TorrentPath, string DownloadPath)

    {
        //TorrentManager Manager;
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
            Manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
            {
                lock (Listener)
                    Listener.WriteLine($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
            };

            Manager.PeerConnected += (o, e) =>
            {
                lock (Listener)
                    Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
            };

            Manager.ConnectionAttemptFailed += (o, e) =>
            {
                lock (Listener)
                    Listener.WriteLine(
                        $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
            };

            Manager.PieceHashed += delegate (object? o, PieceHashedEventArgs e)
            {
                lock (Listener)
                    Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
            };

            Manager.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e)
            {
                lock (Listener)
                    Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
            };

            Manager.TrackerManager.AnnounceComplete += (sender, e) =>
            {
                Listener.WriteLine($"{e.Successful}: {e.Tracker}");
            };

            #endregion

        await Manager.StartAsync();
        //return Manager;
    }

    public static async void OutputInit()
    {
        StringBuilder sb = new(1024);

        do
        {
            sb.Remove(0, sb.Length);

            sb.AppendLine($"Transfer Rate:      {Engine.TotalDownloadRate / 1024.0:0.00}kB/sec ↓ / {Engine.TotalUploadRate / 1024.0:0.00}kB/sec ↑");
            sb.AppendLine($"Memory Cache:       {Engine.DiskManager.CacheBytesUsed / 1024.0:0.00}/{Engine.Settings.DiskCacheBytes / 1024.0:0.00} kB");
            sb.AppendLine($"Disk IO Rate:       {Engine.DiskManager.ReadRate / 1024.0:0.00} kB/s read / {Engine.DiskManager.WriteRate / 1024.0:0.00} kB/s write");
            sb.AppendLine($"Disk IO Total:      {Engine.DiskManager.TotalBytesRead / 1024.0:0.00} kB read / {Engine.DiskManager.TotalBytesWritten / 1024.0:0.00} kB written");
            sb.AppendLine($"Open Connections:   {Engine.ConnectionManager.OpenConnections}");

            // Print out the port mappings
            foreach (var mapping in Engine.PortMappings.Created)
                sb.AppendLine($"Successful Mapping    {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
            foreach (var mapping in Engine.PortMappings.Failed)
                sb.AppendLine($"Failed mapping:       {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
            foreach (var mapping in Engine.PortMappings.Pending)
                sb.AppendLine($"Pending mapping:      {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");

            sb.AppendLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            sb.AppendLine($"State:              {Manager.State}");
            sb.AppendLine($"Name:               {(Manager.Torrent == null ? "MetaDataMode" : Manager.Torrent.Name)}");
            sb.AppendLine($"Progress:           {Manager.Progress:0.00}");
            sb.AppendLine($"Transferred:        {Manager.Monitor.DataBytesReceived / 1024.0 / 1024.0:0.00} MB ↓ / {Manager.Monitor.DataBytesSent / 1024.0 / 1024.0:0.00} MB ↑");
            sb.AppendLine($"Tracker Status");
            foreach (var tier in Manager.TrackerManager.Tiers)
                sb.AppendLine($"\t{tier.ActiveTracker} : Announce Succeeded: {tier.LastAnnounceSucceeded}. Scrape Succeeded: {tier.LastScrapeSucceeded}.");

            if (Manager.PieceManager != null)
                sb.AppendFormat("Current Requests:   {0}", await Manager.PieceManager.CurrentRequestCountAsync());

            var peers = await Manager.GetPeersAsync();

            sb.AppendLine("Outgoing:");
            foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Outgoing))
            {
                sb.AppendFormat("\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                            p.Monitor.DownloadRate / 1024.0,
                                                                            p.AmRequestingPiecesCount,
                                                                            p.Monitor.UploadRate / 1024.0,
                                                                            p.EncryptionType,
                                                                            string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
            }
            sb.AppendLine();
            sb.AppendLine("Incoming:");
            foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Incoming))
            {
                sb.AppendFormat("\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                            p.Monitor.DownloadRate / 1024.0,
                                                                            p.AmRequestingPiecesCount,
                                                                            p.Monitor.UploadRate / 1024.0,
                                                                            p.EncryptionType,
                                                                            string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
            }

            sb.AppendLine();
            if (Manager.Torrent != null)
                foreach (var file in Manager.Files)
                    sb.AppendFormat("{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);

            Console.Clear();
            Console.WriteLine(sb.ToString());
            Listener.ExportTo(Console.Out);

        }
        while (Manager.Files[0].BitField.PercentComplete != 100.00);
    }
}