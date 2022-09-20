using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Linq;
using System.Text;

namespace SharpTorrent.MVVM.Model
{
    static class MainModel
    {
        public static Top10Listener Listener = new(10);
        public static ClientEngine Engine = new();
        public static StringBuilder Output = new(1024);


        static async void DownloadAsync(
            string TorrentPath, string DownloadPath)
        {
            try
            {
                var settingsBuilder = new TorrentSettingsBuilder();
                TorrentManager? manager = await Engine.AddAsync(
                    await Torrent.LoadAsync(
                        TorrentPath ?? "#"),
                        DownloadPath ?? "#",
                        settingsBuilder.ToSettings());

                manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
                {
                    lock (Listener)
                        Listener.WriteLine($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
                };
            }
            catch (Exception e) { Console.WriteLine("Couldn't decode file. " + e.Message); }


            var torrent = Engine.Torrents[0];


            torrent.PeerConnected += (o, e) => {
                lock (Listener)
                    Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
            };

            torrent.ConnectionAttemptFailed += (o, e) => {
                lock (Listener)
                    Listener.WriteLine(
                        $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
            };

            torrent.PieceHashed += delegate (object? o, PieceHashedEventArgs e) {
                lock (Listener)
                    Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
            };

            torrent.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e) {
                lock (Listener)
                    Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
            };

            torrent.TrackerManager.AnnounceComplete += (sender, e) => {
                Listener.WriteLine($"{e.Successful}: {e.Tracker}");
            };

            await torrent.StartAsync();



            do
            {
                Output.Remove(0, Output.Length);

                Output.AppendLine($"Transfer Rate:      {Engine.TotalDownloadRate / 1024.0:0.00}kB/sec ↓ / {Engine.TotalUploadRate / 1024.0:0.00}kB/sec ↑");
                Output.AppendLine($"Memory Cache:       {Engine.DiskManager.CacheBytesUsed / 1024.0:0.00}/{Engine.Settings.DiskCacheBytes / 1024.0:0.00} kB");
                Output.AppendLine($"Disk IO Rate:       {Engine.DiskManager.ReadRate / 1024.0:0.00} kB/s read / {Engine.DiskManager.WriteRate / 1024.0:0.00} kB/s write");
                Output.AppendLine($"Disk IO Total:      {Engine.DiskManager.TotalBytesRead / 1024.0:0.00} kB read / {Engine.DiskManager.TotalBytesWritten / 1024.0:0.00} kB written");
                Output.AppendLine($"Open Connections:   {Engine.ConnectionManager.OpenConnections}");

                // Print out the port mappings
                foreach (var mapping in Engine.PortMappings.Created)
                    Output.AppendLine($"Successful Mapping    {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
                foreach (var mapping in Engine.PortMappings.Failed)
                    Output.AppendLine($"Failed mapping:       {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");
                foreach (var mapping in Engine.PortMappings.Pending)
                    Output.AppendLine($"Pending mapping:      {mapping.PublicPort}:{mapping.PrivatePort} ({mapping.Protocol})");

                Output.AppendLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                Output.AppendLine($"State:              {torrent.State}");
                Output.AppendLine($"Name:               {(torrent.Torrent == null ? "MetaDataMode" : torrent.Torrent.Name)}");
                Output.AppendLine($"Progress:           {torrent.Progress:0.00}");
                Output.AppendLine($"Transferred:        {torrent.Monitor.DataBytesReceived / 1024.0 / 1024.0:0.00} MB ↓ / {torrent.Monitor.DataBytesSent / 1024.0 / 1024.0:0.00} MB ↑");
                Output.AppendLine($"Tracker Status");
                foreach (var tier in torrent.TrackerManager.Tiers)
                    Output.AppendLine($"\t{tier.ActiveTracker} : Announce Succeeded: {tier.LastAnnounceSucceeded}. Scrape Succeeded: {tier.LastScrapeSucceeded}.");

                if (torrent.PieceManager != null)
                    Output.AppendFormat("Current Requests:   {0}", await torrent.PieceManager.CurrentRequestCountAsync());

                var peers = await torrent.GetPeersAsync();

                Output.AppendLine("Outgoing:");
                foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Outgoing))
                {
                    Output.AppendFormat("\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                                p.Monitor.DownloadRate / 1024.0,
                                                                                p.AmRequestingPiecesCount,
                                                                                p.Monitor.UploadRate / 1024.0,
                                                                                p.EncryptionType,
                                                                                string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
                }
                Output.AppendLine();
                Output.AppendLine("Incoming:");
                foreach (PeerId p in peers.Where(t => t.ConnectionDirection == Direction.Incoming))
                {
                    Output.AppendFormat("\t{2} - {1:0.00}/{3:0.00}kB/sec - {0} - {4} ({5})", p.Uri,
                                                                                p.Monitor.DownloadRate / 1024.0,
                                                                                p.AmRequestingPiecesCount,
                                                                                p.Monitor.UploadRate / 1024.0,
                                                                                p.EncryptionType,
                                                                                string.Join("|", p.SupportedEncryptionTypes.Select(t => t.ToString()).ToArray()));
                }

                Output.AppendLine();

                Console.Clear();
                Console.WriteLine(Output.ToString());
                Listener.ExportTo(Console.Out);

            }
            while (torrent.Files[0].BitField.PercentComplete != 100.00);
        }

    }
}
