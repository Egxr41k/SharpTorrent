using Microsoft.Win32;
using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SharpTorrent.MVVM.Models;

internal class SharpTorrentModel
{
    public static ClientEngine Engine = new();

    public Task? Task { get; set; }
    public TorrentManager? Manager { get; private set; }
    public Guid Id { get; private set; }

    public SharpTorrentModel() => ManagerInit();

    //ADD BUGS-PROTECTION
    private void ManagerInit() 
    {
        OpenFileDialog ofd = new()
        {
            Filter = "Torrent Files(*.torrent)|*.torrent"
        };

        if (ofd.ShowDialog() == true)
        {
            string downloadFile = ofd.FileName;

            string saveDirectory = Path.GetDirectoryName(ofd.FileName) ??
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            
            Task.Run(async () =>
            {
                Manager = await Engine.AddAsync(
                    await Torrent.LoadAsync
                        (downloadFile),
                    saveDirectory,
                    new TorrentSettingsBuilder()
                        .ToSettings());
            }).Wait();
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
