namespace ScriptedEvents.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Structures;

    public static class CountdownHelper
    {
        public static CoroutineHandle? MainHandle { get; set; }

        public static Dictionary<Player, Countdown> Countdowns { get; } = new();

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
                        TimeSpan timeLeft = ct.EndTime - DateTime.UtcNow;
                        ply.Broadcast(1, $"<size=26><color=#5EB3FF><b>{ct.Text}</b></color></size>\n{(int)timeLeft.TotalSeconds}");
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

            Countdowns.Add(player, new(player, text, (int)ts.TotalSeconds));
        }
    }
}
