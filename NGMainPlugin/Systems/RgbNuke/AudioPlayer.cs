namespace NGMainPlugin.Systems.RGBNuke
{
    using System;
    using System.IO;
    using Mirror;
    using PluginAPI.Helpers;
    using SCPSLAudioApi.AudioCore;
    using UnityEngine;
    using VoiceChat;

    internal class FakeConnection : NetworkConnectionToClient
    {
        public FakeConnection(int connectionId) : base(connectionId)
        {

        }

        public override string address => "localhost";

        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }

        public override void Disconnect()
        {
        }
    }

    internal static class AudioPlayer
    {
        private static ReferenceHub _audioBot;

        private static string AudioPath => RGBNuke.Config.MusicDirectory.Replace("{global}", Paths.GlobalPlugins.Plugins);

        private static int Volume => RGBNuke.Config.Volume;

        public static void PlayAudio()
        {
            if (_audioBot == null) AddDummy();

            StopAudio();

            var path = Path.Combine(AudioPath, "warhead.ogg");
            var audioPlayer = AudioPlayerBase.Get(_audioBot);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
            audioPlayer.Volume = Volume / 100f;
            audioPlayer.Loop = true;
            audioPlayer.Play(0);
        }

        public static void StopAudio()
        {
            if (_audioBot == null) return;

            var audioPlayer = AudioPlayerBase.Get(_audioBot);
            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }
        }

        private static void AddDummy()
        {
            var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeConnection = new FakeConnection(0);
            _audioBot = newPlayer.GetComponent<ReferenceHub>();

            NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
            _audioBot.authManager.InstanceMode = CentralAuth.ClientInstanceMode.Unverified;

            try
            {
                _audioBot.nicknameSync.SetNick(RGBNuke.Config.DisplayName);
            }
            catch (Exception)
            {
            }
        }

        public static void RemoveDummy()
        {
            if (_audioBot == null) return;

            var audioPlayer = AudioPlayerBase.Get(_audioBot);
            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }

            _audioBot.OnDestroy();
            CustomNetworkManager.TypedSingleton.OnServerDisconnect(_audioBot.connectionToClient);
            UnityEngine.Object.Destroy(_audioBot.gameObject);
        }
    }
}