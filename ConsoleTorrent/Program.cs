using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Net;
using System.Text;

namespace ConsoleTorrent;

class Program
{
    static void Main(string[] args)
    {
        var Listener = new Top10Listener(10);
        string? TorrentPath;
        do
        {
            Console.Write("Enter th torrent path: ");
            TorrentPath = Console.ReadLine();
        }
        while (!File.Exists(TorrentPath));
        var downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"\Downloads");

        Task.Run(async () =>
        {

            var Engine = new ClientEngine(new EngineSettingsBuilder
            {
                AllowPortForwarding = true,
                AutoSaveLoadDhtCache = true,
                AutoSaveLoadFastResume = true,
                AutoSaveLoadMagnetLinkMetadata = true,
                DhtEndPoint = new IPEndPoint(IPAddress.Any, 55123),
                HttpStreamingPrefix = $"http://127.0.0.1:{55125}/"
            }.ToSettings());


            try
            {
                var settingsBuilder = new TorrentSettingsBuilder();
                var torrent = await Torrent.LoadAsync(TorrentPath ?? "#");
                var manager = await Engine.AddAsync(
                    torrent,
                    downloadPath,
                    settingsBuilder.ToSettings());

                manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
                {
                    lock (Listener)
                        Listener.WriteLine($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
                };

                Console.WriteLine(manager.InfoHashes.V1OrV2.ToHex());
            }
            catch (Exception e)
            {
                Console.Write("Couldn't decode {0}: ", TorrentPath);
                Console.WriteLine(e.Message);
            }

            foreach (TorrentManager manager in Engine.Torrents)
            {
                manager.PeerConnected += (o, e) => {
                    lock (Listener)
                        Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
                };
                manager.ConnectionAttemptFailed += (o, e) => {
                    lock (Listener)
                        Listener.WriteLine(
                            $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
                };

                manager.PieceHashed += delegate (object? o, PieceHashedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
                };

                manager.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
                };

                manager.TrackerManager.AnnounceComplete += (sender, e) => {
                    Listener.WriteLine($"{e.Successful}: {e.Tracker}");
                };

                await manager.StartAsync();
            }

            StringBuilder sb = new StringBuilder(1024);

            while (Engine.IsRunning)
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

                foreach (TorrentManager manager in Engine.Torrents)
                {
                    sb.AppendLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    sb.AppendLine($"State:              {manager.State}");
                    sb.AppendLine($"Name:               {(manager.Torrent == null ? "MetaDataMode" : manager.Torrent.Name)}");
                    sb.AppendLine($"Progress:           {manager.Progress:0.00}");
                    sb.AppendLine($"Transferred:        {manager.Monitor.DataBytesReceived / 1024.0 / 1024.0:0.00} MB ↓ / {manager.Monitor.DataBytesSent / 1024.0 / 1024.0:0.00} MB ↑");
                    sb.AppendLine($"Tracker Status");
                    foreach (var tier in manager.TrackerManager.Tiers)
                        sb.AppendLine($"\t{tier.ActiveTracker} : Announce Succeeded: {tier.LastAnnounceSucceeded}. Scrape Succeeded: {tier.LastScrapeSucceeded}.");

                    if (manager.PieceManager != null)
                        sb.AppendFormat("Current Requests:   {0}", await manager.PieceManager.CurrentRequestCountAsync());

                    var peers = await manager.GetPeersAsync();

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
                    if (manager.Torrent != null)
                        foreach (var file in manager.Files)
                            sb.AppendFormat("{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);
                }
                Console.Clear ();
                Console.WriteLine (sb.ToString ());
                Listener.ExportTo (Console.Out);

                await Task.Delay(500);
            }
        }).Wait();

    }
}