namespace ScriptedEvents.API.Enums
{
    /// <summary>
    /// Represents where the script was executed.
    /// </summary>
    public enum ExecuteContext
    {
        /// <summary>
        /// N/A.
        /// </summary>
        None,

        /// <summary>
        /// Executed via Remote Admin.
        /// </summary>
        RemoteAdmin,

        /// <summary>
        /// Executed via Server Console.
        /// </summary>
        ServerConsole,

        /// <summary>
        /// Executed automatically by plugin.
        /// </summary>
        Automatic,

        /// <summary>
        /// Executed via Player Console command.
        /// </summary>
        PlayerConsole,
    }
}
