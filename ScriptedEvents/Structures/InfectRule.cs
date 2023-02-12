namespace ScriptedEvents.Structures
{
    using PlayerRoles;

    /// <summary>
    /// Represents a rule for an infection game.
    /// </summary>
    public readonly struct InfectRule
    {
        public InfectRule(RoleTypeId oldRole, RoleTypeId newRole, bool movePlayer)
        {
            OldRole = oldRole;
            NewRole = newRole;
            MovePlayer = movePlayer;
        }

        /// <summary>
        /// Gets the role a player must be in order to get infected.
        /// </summary>
        public RoleTypeId OldRole { get; }

        /// <summary>
        /// Gets the role a user will become if infected with this rule.
        /// </summary>
        public RoleTypeId NewRole { get; }

        /// <summary>
        /// Gets a value indicating whether or not the player will be moved from the new role's spawn location to their death position.
        /// </summary>
        public bool MovePlayer { get; }
    }
}
