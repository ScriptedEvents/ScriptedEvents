namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using MEC;
    using ScriptedEvents.Structures;

    using UnityEngine;

    /// <summary>
    /// Module that controls showing countdowns to players.
    /// </summary>
    public class CountdownModule : SEModule
    {
        public override string Name => "CountdownModule";
        
        public static CountdownModule? Singleton { get; private set; }

        /// <summary>
        /// Gets or sets the main coroutine handle.
        /// </summary>
        private CoroutineHandle? MainHandle { get; set; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of active countdowns.
        /// </summary>
        private Dictionary<Player, Countdown> Countdowns { get; } = new();

        /// <summary>
        /// Gets the default countdown text.
        /// </summary>
        private static string BroadcastString => MainPlugin.Singleton.Config.CountdownString;

        /// <summary>
        /// Begins the countdown coroutine.
        /// </summary>
        public override void Init()
        {
            base.Init();
            Singleton = this;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestarting;
            Exiled.Events.Handlers.Server.WaitingForPlayers += WaitingForPlayers;
        }

        public override void Kill()
        {
            base.Kill();
            Singleton = null;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRestarting;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
        }

        private void OnRestarting()
        {
            if (MainHandle is null || !MainHandle.Value.IsRunning) return;
            Timing.KillCoroutines(MainHandle.Value);
            MainHandle = null;
            Countdowns.Clear();
        }

        private void WaitingForPlayers()
        {
            MainHandle = Timing.RunCoroutine(InternalCoroutine());
        }

        /// <summary>
        /// Given a <see cref="TimeSpan"/>, returns a pretty string format.
        /// </summary>
        /// <param name="time">The time span.</param>
        /// <returns>String format.</returns>
        private static string Display(TimeSpan time)
        {
            int seconds = Mathf.RoundToInt((float)time.TotalSeconds);

            if (seconds <= 60)
                return Mathf.RoundToInt((float)time.TotalSeconds).ToString();

            string secondsStr = time.Seconds.ToString();
            if (secondsStr.Length == 1)
            {
                secondsStr = $"0{secondsStr}";
            }

            if (time.TotalHours < 1)
                return $"{time.Minutes}:{secondsStr}";

            string minStr = time.Minutes.ToString();
            if (minStr.Length == 1)
            {
                minStr = $"0{minStr}";
            }

            if (time.TotalDays < 1)
                return $"{time.Hours}:{minStr}:{secondsStr}";

            string hourStr = time.Hours.ToString();
            if (hourStr.Length == 1)
            {
                hourStr = $"0{hourStr}";
            }

            return $"{time.Days}:{hourStr}:{minStr}:{secondsStr}";
        }

        /// <summary>
        /// Adds a countdown to a player.
        /// </summary>
        /// <param name="player">The player to count down for.</param>
        /// <param name="text">The text to show.</param>
        /// <param name="ts">The time on the countdown.</param>
        /// <param name="source">The script that started the countdown.</param>
        public void AddCountdown(Player player, string text, TimeSpan ts, Script? source = null)
        {
            if (Countdowns.ContainsKey(player))
                Countdowns.Remove(player);

            Countdown ct = new(player, text, (int)ts.TotalSeconds, source);

            Countdowns.Add(player, ct);
            player.ClearBroadcasts();
            player.Broadcast(1, BroadcastString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Display(ts)));
        }

        private IEnumerator<float> InternalCoroutine()
        {
            while (true)
            {
                if (Round.IsEnded)
                    break;

                foreach (Player ply in Player.List)
                {
                    if (!Countdowns.TryGetValue(ply, out Countdown ct))
                        continue;

                    if (ct.Expired) continue;
                    ply.ClearBroadcasts();
                    ply.Broadcast(1, BroadcastString.Replace("{TEXT}", ct.Text).Replace("{TIME}", Display(ct.TimeLeft)));
                }

                yield return Timing.WaitForSeconds(1);
            }

            MainHandle = null;
        }
    }
}
