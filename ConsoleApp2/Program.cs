using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var Listener = new Top10Listener(10);
            string TorrentPath, DownloadPath;
            do
            {
                Console.Write("Enter the torrent file path: ");
                TorrentPath = Console.ReadLine();

                Console.Write("Enter the download folder path: ");
                DownloadPath = Console.ReadLine();
            }
            while (!File.Exists(TorrentPath) &&
                !Directory.Exists(DownloadPath));

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
                    TorrentManager manager = await Engine.AddAsync(
                        await Torrent.LoadAsync(
                            TorrentPath ?? "#"),
                            DownloadPath ?? "#",
                            settingsBuilder.ToSettings());

                    manager.PeersFound += delegate (object sender, PeersAddedEventArgs e)
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

                torrent.PieceHashed += delegate (object o, PieceHashedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
                };

                torrent.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e) {
                    lock (Listener)
                        Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
                };

                torrent.TrackerManager.AnnounceComplete += (sender, e) => {
                    Listener.WriteLine($"{e.Successful}: {e.Tracker}");
                };

                await torrent.StartAsync();


                StringBuilder sb = new StringBuilder(1024);

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
                    sb.AppendLine($"State:              {torrent.State}");
                    sb.AppendLine($"Name:               {(torrent.Torrent == null ? "MetaDataMode" : torrent.Torrent.Name)}");
                    sb.AppendLine($"Progress:           {torrent.Progress:0.00}");
                    sb.AppendLine($"Transferred:        {torrent.Monitor.DataBytesReceived / 1024.0 / 1024.0:0.00} MB ↓ / {torrent.Monitor.DataBytesSent / 1024.0 / 1024.0:0.00} MB ↑");
                    sb.AppendLine($"Tracker Status");
                    foreach (var tier in torrent.TrackerManager.Tiers)
                        sb.AppendLine($"\t{tier.ActiveTracker} : Announce Succeeded: {tier.LastAnnounceSucceeded}. Scrape Succeeded: {tier.LastScrapeSucceeded}.");

                    if (torrent.PieceManager != null)
                        sb.AppendFormat("Current Requests:   {0}", await torrent.PieceManager.CurrentRequestCountAsync());

                    var peers = await torrent.GetPeersAsync();

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

                    Console.Clear();
                    Console.WriteLine(sb.ToString());
                    Listener.ExportTo(Console.Out);

                }
                while (torrent.Files[0].BitField.PercentComplete != 100.00);

            }).Wait();
        }
    }
}
