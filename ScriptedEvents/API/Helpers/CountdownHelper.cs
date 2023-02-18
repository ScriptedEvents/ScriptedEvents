namespace ScriptedEvents.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public static class CountdownHelper
    {
        public static CoroutineHandle? MainHandle { get; set; }

        public static Dictionary<Player, Countdown> Countdowns { get; } = new();

        public static string BroadcastString => MainPlugin.Singleton.Config.CountdownString;

        public static void Start()
        {
            MainHandle = Timing.RunCoroutine(InternalCoroutine());
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
                        ply.Broadcast(1, BroadcastString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Mathf.RoundToInt((float)ct.TimeLeft.TotalSeconds)));
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
            player.Broadcast(1, BroadcastString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Mathf.RoundToInt((float)ts.TotalSeconds)));
        }
    }
}
