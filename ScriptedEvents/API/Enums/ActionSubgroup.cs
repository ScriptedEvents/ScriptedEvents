namespace ScriptedEvents.API.Enums
{
    /// <summary>
    /// Represents a group to give to each action.
    /// </summary>
    public enum ActionSubgroup
    {
        /// <summary>
        /// Broadcast related action.
        /// </summary>
        Broadcast,

        /// <summary>
        /// Cassie related action.
        /// </summary>
        Cassie,

        /// <summary>
        /// Debugging related action.
        /// </summary>
        Debug,

        /// <summary>
        /// Health related action.
        /// </summary>
        Health,

        /// <summary>
        /// Item related action.
        /// </summary>
        Item,

        /// <summary>
        /// Facility lighting related action.
        /// </summary>
        Lights,

        /// <summary>
        /// Logic related action, such as IFs and STOPs.
        /// </summary>
        Logic,

        /// <summary>
        /// Map related action.
        /// </summary>
        Map,

        /// <summary>
        /// Uncategorized action.
        /// </summary>
        Misc,

        /// <summary>
        /// Player management related action.
        /// </summary>
        Player,

        /// <summary>
        /// Round related action.
        /// </summary>
        Round,

        /// <summary>
        /// Round Rule related action.
        /// </summary>
        RoundRule,

        /// <summary>
        /// Variable related action.
        /// </summary>
        Variable,

        /// <summary>
        /// Yielding related action.
        /// </summary>
        Yielding,

        /// <summary>
        /// Server related action.
        /// </summary>
        Server,

        /// <summary>
        /// Teleportation related action.
        /// </summary>
        Teleportation,

        /// <summary>
        /// Math related action.
        /// </summary>
        Math,

        /// <summary>
        /// Map info related action.
        /// </summary>
        MapInfo,

        /// <summary>
        /// "All In One" info related action.
        /// </summary>
        AllInOneInfo,

        /// <summary>
        /// Player fetching action.
        /// </summary>
        PlayerFetch,

        /// <summary>
        /// String modification related action.
        /// </summary>
        String,

        /// <summary>
        /// Script info related action.
        /// </summary>
        ScriptInfo,
    }
}
