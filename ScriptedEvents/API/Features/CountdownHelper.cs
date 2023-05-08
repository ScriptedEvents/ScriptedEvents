namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Structures;
    using UnityEngine;

    public static class CountdownHelper
    {
        public static CoroutineHandle? MainHandle { get; set; }

        public static Dictionary<Player, Countdown> Countdowns { get; } = new();

        public static string BroadcastString => MainPlugin.Singleton.Config.CountdownString ?? "Countdown";

        public static void Start()
        {
            MainHandle = Timing.RunCoroutine(InternalCoroutine());
        }

        public static string Display(TimeSpan time)
        {
            int seconds = Mathf.RoundToInt((float)time.TotalSeconds);

            if (seconds <= 60)
                return Mathf.RoundToInt((float)time.TotalSeconds).ToString();

            string secondsStr = time.Seconds.ToString();
            if (secondsStr.Length == 1)
            {
                secondsStr = $"0{secondsStr}";
            }

            return $"{time.Minutes}:{secondsStr}";
        }

        private static IEnumerator<float> InternalCoroutine()
        {
            while (true)
            {
                if (Round.IsEnded)
                    break;

                foreach (Player ply in Player.List)
                {
                    if (!Countdowns.TryGetValue(ply, out Countdown ct))
                        continue;

                    if (!ct.Expired)
                    {
                        ply.ClearBroadcasts();
                        ply.Broadcast(1, BroadcastString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Display(ct.TimeLeft)));
                    }
                }

                yield return Timing.WaitForSeconds(1);
            }

            MainHandle = null;
        }

        public static void AddCountdown(Player player, string text, TimeSpan ts)
        {
            if (Countdowns.ContainsKey(player))
                Countdowns.Remove(player);

            Countdown ct = new(player, text, (int)ts.TotalSeconds);

            Countdowns.Add(player, ct);
            player.ClearBroadcasts();
            player.Broadcast(1, BroadcastString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Display(ts)));
        }
    }
}
