namespace ScriptedEvents.Structures
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Represents an active countdown, used in the <see cref="API.Features.CountdownModule"/>.
    /// </summary>
    public readonly struct Countdown
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown"/> struct.
        /// </summary>
        /// <param name="target">The player to show the countdown to.</param>
        /// <param name="text">The text of the countdown.</param>
        /// <param name="time">The time of the countdown, in seconds.</param>
        /// <param name="source">The script that executed the countdown.</param>
        public Countdown(Player target, string text, int time, Script source = null)
        {
            Target = target;
            Text = string.IsNullOrWhiteSpace(text) ? "Countdown" : text;
            StartTime = DateTime.UtcNow;
            EndTime = StartTime.AddSeconds(time);
            Source = source;
        }

        /// <summary>
        /// Gets the player seeing the countdown.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the text of the countdown.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the start time of the countdown.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// Gets the end time of the countdown.
        /// </summary>
        public DateTime EndTime { get; }

        /// <summary>
        /// Gets a value indicating whether or not the countdown has ended.
        /// </summary>
        public bool Expired => DateTime.UtcNow > EndTime;

        /// <summary>
        /// Gets a <see cref="TimeSpan"/> representing the amount of time left on the countdown.
        /// </summary>
        public TimeSpan TimeLeft => EndTime - DateTime.UtcNow;

        /// <summary>
        /// Gets the source script that executed the countdown timer.
        /// </summary>
        public Script Source { get; }
    }
}
