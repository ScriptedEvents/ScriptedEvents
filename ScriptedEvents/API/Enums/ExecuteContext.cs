namespace ScriptedEvents.API.Enums
{
    /// <summary>
    /// Represents where the script was executed.
    /// </summary>
    public enum ExecuteContext
    {
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
    }
}
