namespace ScriptedEvents.Structures
{
    using MEC;

    public struct CoroutineData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineData"/> struct.
        /// </summary>
        /// <param name="tag">The coroutine tag.</param>
        public CoroutineData(string tag)
        {
            Key = tag;
            Handle = null;
            IsKilled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineData"/> struct.
        /// </summary>
        /// <param name="handle">The <see cref="CoroutineHandle"/>.</param>
        public CoroutineData(CoroutineHandle handle)
        {
            Key = null;
            Handle = handle;
            IsKilled = false;
        }

        /// <summary>
        /// The coroutine's tag.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The coroutine's handle.
        /// </summary>
        public CoroutineHandle? Handle { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the coroutine has been killed.
        /// </summary>
        public bool IsKilled { get; private set; }

        /// <summary>
        /// Kills the coroutine.
        /// </summary>
        public void Kill()
        {
            if (IsKilled)
                return;

            if (Key != null)
                Timing.KillCoroutines(Key);
            else if (Handle != null && Handle.HasValue)
                Timing.KillCoroutines(Handle.Value);

            IsKilled = true;
        }
    }
}
