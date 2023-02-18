namespace ScriptedEvents.Structures
{
    using System;
    using Exiled.API.Features;

    public readonly struct Countdown
    {
        public Player Target { get; }

        public string Text { get; }

        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

        public bool Expired => DateTime.UtcNow > EndTime;

        public Countdown(Player target, string text, int time)
        {
            Target = target;
            Text = text;
            StartTime = DateTime.UtcNow;
            EndTime = StartTime.AddSeconds(time);
        }
    }
}
