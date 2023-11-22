namespace ScriptedEvents.Structures
{
    using Exiled.API.Features;
    using MEC;

    /// <summary>
    /// Holds a value referencing a coroutine - either its handle or its tag.
    /// </summary>
    public class CoroutineData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineData"/> class.
        /// </summary>
        /// <param name="tag">The coroutine tag.</param>
        public CoroutineData(string tag)
        {
            Key = tag;
            Handle = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineData"/> class.
        /// </summary>
        /// <param name="handle">The <see cref="CoroutineHandle"/>.</param>
        public CoroutineData(CoroutineHandle handle)
        {
            Key = null;
            Handle = handle;
        }

        /// <summary>
        /// Gets or sets the coroutine's tag.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the coroutine's handle.
        /// </summary>
        public CoroutineHandle? Handle { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the coroutine has been killed.
        /// </summary>
        public bool IsKilled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the coroutine is running.
        /// </summary>
        public bool IsRunning => !Handle.HasValue || Handle.Value.IsRunning;

        /// <summary>
        /// Kills the coroutine.
        /// </summary>
        public void Kill()
        {
            if (IsKilled)
                return;

            if (Key != null)
            {
                Timing.KillCoroutines(Key);
                Log.Debug($"Stopped coroutine with tag '{Key}'");
            }
            else if (Handle != null && Handle.HasValue)
            {
                Timing.KillCoroutines(Handle.Value);
                Log.Debug($"Stopped coroutine with tag '{Handle.Value.Tag ?? "UN-TAGGED-COROUTINE"}'");
            }

            IsKilled = true;
        }
    }
}
