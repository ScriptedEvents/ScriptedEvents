namespace ScriptedEvents.Structures
{
    /// <summary>
    /// Holds information about an SE error.
    /// </summary>
    public readonly struct ErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo"/> struct.
        /// </summary>
        /// <param name="id">The error code.</param>
        /// <param name="info">Short description.</param>
        /// <param name="longInfo">Long description.</param>
        public ErrorInfo(int id, string info, string longInfo)
        {
            Id = id;
            Info = info;
            LongDescription = longInfo;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the short description of the error.
        /// </summary>
        public string Info { get; }

        /// <summary>
        /// Gets the long description of the error.
        /// </summary>
        public string LongDescription { get; }

        /// <inheritdoc/>
        public override string ToString()
            => $"{Info} [Error Code: SE-{Id}] [Run 'shelp SE-{Id}' in server console for more details]";
    }
}
