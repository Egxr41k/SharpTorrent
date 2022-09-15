using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class MainModel
    {
        Top10Listener Listener { get; }
        public static ClientEngine Engine;
        public static string downloadsPath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile),
            @"\Downloads");
        public void EngineInit(out ClientEngine engine)
        {
            var settingBuilder = new EngineSettingsBuilder
            {
                AllowPortForwarding = true,

                AutoSaveLoadDhtCache = true,

                AutoSaveLoadFastResume = true,

                AutoSaveLoadMagnetLinkMetadata = true,

                ListenEndPoint = new IPEndPoint(IPAddress.Any, 55123),

                DhtEndPoint = new IPEndPoint(IPAddress.Any, 55123),

                HttpStreamingPrefix = $"http://127.0.0.1:55125/",
            };
            engine = new ClientEngine(settingBuilder.ToSettings());
        }
        public async Task DownloadAsync(string torrentPath)
        {
            if (torrentPath.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var settingsBuilder = new TorrentSettingsBuilder();
                    var torrent = await Torrent.LoadAsync(torrentPath);
                    var manager = await Engine.AddAsync(
                        torrent,
                        MainModel.downloadsPath,
                        settingsBuilder.ToSettings());

                    manager.PeersFound += Manager_PeersFound;
                    Console.WriteLine(manager.InfoHashes.V1OrV2.ToHex());
                }
                catch (Exception) { }
            }


            // For each torrent manager we loaded and stored in our list, hook into the events
            // in the torrent manager and start the engine.
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
                // Every time a piece is hashed, this is fired.
                manager.PieceHashed += delegate (object o, PieceHashedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
                };

                // Every time the state changes (Stopped -> Seeding -> Downloading -> Hashing) this is fired
                manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
                };

                // Every time the tracker's state changes, this is fired
                manager.TrackerManager.AnnounceComplete += (sender, e) => {
                    Listener.WriteLine($"{e.Successful}: {e.Tracker}");
                };

                // Start the torrentmanager. The file will then hash (if required) and begin downloading/seeding.
                // As EngineSettings.AutoSaveLoadDhtCache is enabled, any cached data will be loaded into the
                // Dht engine when the first torrent is started, enabling it to bootstrap more rapidly.
                await manager.StartAsync();
            }

            // While the torrents are still running, print out some stats to the screen.
            // Details for all the loaded torrent managers are shown.
            StringBuilder sb = new StringBuilder(1024);
            while (Engine.IsRunning)
            {
                sb.Remove(0, sb.Length);

                AppendFormat(sb, $"Transfer Rate:      {Engine.TotalDownloadRate / 1024.0:0.00}kB/sec ↓ / {Engine.TotalUploadRate / 1024.0:0.00}kB/sec ↑");
                AppendFormat(sb, $"Memory Cache:       {Engine.DiskManager.CacheBytesUsed / 1024.0:0.00}/{Engine.Settings.DiskCacheBytes / 1024.0:0.00} kB");
                AppendFormat(sb, $"Disk IO Rate:       {Engine.DiskManager.ReadRate / 1024.0:0.00} kB/s read / {Engine.DiskManager.WriteRate / 1024.0:0.00} kB/s write");
                AppendFormat(sb, $"Disk IO Total:      {Engine.DiskManager.TotalBytesRead / 1024.0:0.00} kB read / {Engine.DiskManager.TotalBytesWritten / 1024.0:0.00} kB written");
                AppendFormat(sb, $"Open Connections:   {Engine.ConnectionManager.OpenConnections}");

                // Print out the port mappings
                foreach (var mapping in Engine.PortMappings.Created)
                    AppendFormat(sb, $"Successful Mapping    {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
                foreach (var mapping in Engine.PortMappings.Failed)
                    AppendFormat(sb, $"Failed mapping:       {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
                foreach (var mapping in Engine.PortMappings.Pending)
                    AppendFormat(sb, $"Pending mapping:      {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");

                foreach (TorrentManager manager in Engine.Torrents)
                {
                    AppendSeparator(sb);
                    AppendFormat(sb, $"State:              {manager.State}");
                    AppendFormat(sb, $"Name:               {(manager.Torrent == null ? "MetaDataMode" : manager.Torrent.Name)}");
                    AppendFormat(sb, $"Progress:           {manager.Progress:0.00}");
                    AppendFormat(sb, $"Transferred:        {manager.Monitor.DataBytesReceived / 1024.0 / 1024.0:0.00} MB ↓ / {manager.Monitor.DataBytesSent / 1024.0 / 1024.0:0.00} MB ↑");
                    AppendFormat(sb, $"Tracker Status");
                    foreach (var tier in manager.TrackerManager.Tiers)
                        AppendFormat(sb, $"\t{tier.ActiveTracker} : Announce Succeeded: {tier.LastAnnounceSucceeded}. Scrape Succeeded: {tier.LastScrapeSucceeded}.");

                    if (manager.PieceManager != null)
                        AppendFormat(sb, "Current Requests:   {0}", await manager.PieceManager.CurrentRequestCountAsync());

                    var peers = await manager.GetPeersAsync();
                    AppendFormat(sb, "Outgoing:");
                    foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Outgoing))
                    {
                        AppendFormat(sb, "\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                                    p.Monitor.DownloadRate / 1024.0,
                                                                                    p.AmRequestingPiecesCount,
                                                                                    p.Monitor.UploadRate / 1024.0,
                                                                                    p.EncryptionType,
                                                                                    string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
                    }
                    AppendFormat(sb, "");
                    AppendFormat(sb, "Incoming:");
                    foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Incoming))
                    {
                        AppendFormat(sb, "\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                                    p.Monitor.DownloadRate / 1024.0,
                                                                                    p.AmRequestingPiecesCount,
                                                                                    p.Monitor.UploadRate / 1024.0,
                                                                                    p.EncryptionType,
                                                                                    string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
                    }

                    AppendFormat(sb, "", null);
                    if (manager.Torrent != null)
                        foreach (var file in manager.Files)
                            AppendFormat(sb, "{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);
                }
                Console.Clear();
                Console.WriteLine(sb.ToString());
                Listener.ExportTo(Console.Out);

                await Task.Delay(5000);
            }
        }

        void Manager_PeersFound(object sender, PeersAddedEventArgs e)
        {
            lock (Listener)
                Listener.WriteLine($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");//throw new Exception("The method or operation is not implemented.");
        }

        void AppendSeparator(StringBuilder sb)
        {
            AppendFormat(sb, "");
            AppendFormat(sb, "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            AppendFormat(sb, "");
        }

        void AppendFormat(StringBuilder sb, string str, params object[] formatting)
        {
            if (formatting != null && formatting.Length > 0)
                sb.AppendFormat(str, formatting);
            else
                sb.Append(str);
            sb.AppendLine();
        }
    }
}
