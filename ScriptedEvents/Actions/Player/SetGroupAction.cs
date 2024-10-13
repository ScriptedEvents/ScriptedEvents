namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class SetGroupAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SETGROUP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets a player's group. Can remove the group of needed.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to set the group for.", true),
            new Argument("group", typeof(string), "The group to set. Use 'NONE' to remove the group.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string group = (string)Arguments[1] != "NONE" ? (string)Arguments[1] : null;

            foreach (Player player in (PlayerCollection)Arguments[0])
            {
                player.Group = ServerStatic.PermissionsHandler.GetGroup(group);
            }

            return new(true);
        }
    }
}