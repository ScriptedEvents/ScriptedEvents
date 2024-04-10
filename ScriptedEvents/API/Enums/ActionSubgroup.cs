namespace ScriptedEvents.API.Enums
{
    /// <summary>
    /// Represents a group to give to each action.
    /// </summary>
    public enum ActionSubgroup
    {
        /// <summary>
        /// Broadcast-related action.
        /// </summary>
        Broadcast,

        /// <summary>
        /// Cassie-related action.
        /// </summary>
        Cassie,

        /// <summary>
        /// Debugging-related action.
        /// </summary>
        Debug,

        /// <summary>
        /// Health-related action.
        /// </summary>
        Health,

        /// <summary>
        /// Item-related action.
        /// </summary>
        Item,

        /// <summary>
        /// Facility lighting related action.
        /// </summary>
        Lights,

        /// <summary>
        /// Logic action, such as IFs and STOPs.
        /// </summary>
        Logic,

        /// <summary>
        /// Map-related action.
        /// </summary>
        Map,

        /// <summary>
        /// Uncategorized action.
        /// </summary>
        Misc,

        /// <summary>
        /// Player-related action.
        /// </summary>
        Player,

        /// <summary>
        /// Round-related action.
        /// </summary>
        Round,

        /// <summary>
        /// Round Rule related action.
        /// </summary>
        RoundRule,

        /// <summary>
        /// Variable-related action.
        /// </summary>
        Variable,

        /// <summary>
        /// Yielding action.
        /// </summary>
        Yielding,
    }
}
