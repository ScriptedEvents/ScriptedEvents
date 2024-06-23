namespace ScriptedEvents.Tutorials
{
    public interface IScriptExample : ITutorial
    {
        /// <summary>
        /// Gets the script purpose.
        /// </summary>
        public string Purpose { get; }

        /// <summary>
        /// Gets the script difficuty.
        /// </summary>
        public string Difficulty { get; }

        /// <summary>
        /// Gets the original script author.
        /// </summary>
        public string OriginalAuthor { get; }

        /// <summary>
        /// Gets an attached note.
        /// </summary>
        public string Note { get; }

        /// <summary>
        /// Gets the last modification date.
        /// </summary>
        public string LastModification { get; }

        /// <summary>
        /// Gets the version of SE in which the script was tested.
        /// </summary>
        public string RatedVersion { get; }
    }
}
