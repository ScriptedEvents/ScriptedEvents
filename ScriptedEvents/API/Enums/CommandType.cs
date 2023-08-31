namespace ScriptedEvents.API.Enums
{
    /// <summary>
    /// Indicates the type of a command. Used for <see cref="Structures.CustomCommand"/> and <see cref="Commands.CustomCommand"/>.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Indicates a player console command.
        /// </summary>
        PlayerConsole,

        /// <summary>
        /// Indicates a remote admin command.
        /// </summary>
        RemoteAdmin,

        /// <summary>
        /// Indicates a server console command.
        /// </summary>
        ServerConsole,
    }
}
