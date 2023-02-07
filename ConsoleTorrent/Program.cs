using MonoTorrent;
using MonoTorrent.Client;
using System.Net;
using System.Text;

namespace ConsoleTorrent;

class Program
{
    static LinkedList<string> Last10Messages;
    static async Task Main(string[] args)
    {
        var Listener = new Top10Listener(10);
        string? TorrentPath, DownloadPath;
        //C:\Users\Egxr41k\Desktop\TorrentSamples\4471_rain_world.torrent
        //C:\Users\Egxr41k\Desktop\TorrentSamples\4471_rain_world
        do
        {
            Console.Write("Enter the torrent file path: ");
            TorrentPath = Console.ReadLine();
            Console.Write("Enter the download folder path: ");
            DownloadPath = Console.ReadLine();
        }
        while (!File.Exists(TorrentPath) &&
            !Directory.Exists(DownloadPath));

        TorrentManager torrent;

        Task.Run(async () =>
        {
            ClientEngine Engine = new();
            torrent = await Engine.AddAsync(
            await Torrent.LoadAsync(
                TorrentPath),
            DownloadPath,
            new TorrentSettingsBuilder()
                .ToSettings());

            //Manager = Engine.Torrents[0];

            // эту штуку нужно выполнять только один раз для каждого менеджера
            #region events

            torrent.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
            };

            torrent.PeerConnected += (o, e) =>
            {
                lock (Last10Messages)
                    AddMessage($"Connection succeeded: {e.Peer.Uri}");
            };

            torrent.ConnectionAttemptFailed += (o, e) =>
            {
                lock (Last10Messages)
                    AddMessage(
                        $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
            };

            torrent.PieceHashed += delegate (object? o, PieceHashedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
            };

            torrent.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e)
            {
                lock (Last10Messages)
                    AddMessage($"OldState: {e.OldState} NewState: {e.NewState}");
            };

            torrent.TrackerManager.AnnounceComplete += (sender, e) =>
            {
                AddMessage($"{e.Successful}: {e.Tracker}");
            };
            #endregion

            

            await torrent.StartAsync();


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
                sb.AppendLine();
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
                if (torrent.Torrent != null)
                    foreach (var file in torrent.Files)
                        sb.AppendFormat("{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);

                Console.Clear();
                Console.WriteLine(sb.ToString());
                Listener.ExportTo(Console.Out);

            }
            while (torrent.Files[0].BitField.PercentComplete != 100.00);

        }).Wait();
    }
    public static void AddMessage(string message)
    {
        lock (Last10Messages)
        {
            if (Last10Messages.Count >= 10)
                Last10Messages.RemoveFirst();

            Last10Messages.AddLast(message);
        }
    }
}