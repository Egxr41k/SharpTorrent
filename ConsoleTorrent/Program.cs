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
            Console.WriteLine("Enter th torrent path:");
            TorrentPath = Console.ReadLine();
        } while (File.Exists(TorrentPath));


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
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"\Downloads"),
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
                //112 - 171
            }

        }).Wait();

    }
}