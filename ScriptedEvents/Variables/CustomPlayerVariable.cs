namespace ScriptedEvents.Variables
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.CustomItems.API.Features;
    using ScriptedEvents.Variables.Interfaces;

    public class CustomPlayerVariable : IFloatVariable, IPlayerVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPlayerVariable"/> class.
        /// </summary>
        public CustomPlayerVariable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPlayerVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="description">The description of the variable.</param>
        /// <param name="value">The players to add to the variable.</param>
        public CustomPlayerVariable(string name, string description, List<Player> value)
        {
            Name = name;
            Description = description;
            PlayerList = value;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => PlayerList;

        private List<Player> PlayerList { get; }

        /// <summary>
        /// Adds a range of players to the variable. Will ignore duplicates.
        /// </summary>
        /// <param name="player">The players to add.</param>
        public void Add(params Player[] player)
        {
            foreach (Player plr in player)
            {
                if (!PlayerList.Contains(plr))
                    PlayerList.Add(plr);
            }
        }

        /// <summary>
        /// Removes a range of players from the variable. Will ignore duplicates.
        /// </summary>
        /// <param name="player">The players to remove.</param>
        public void Remove(params Player[] player)
        {
            foreach (Player plr in player)
            {
                if (PlayerList.Contains(plr))
                    PlayerList.Remove(plr);
            }
        }

        /// <summary>
        /// Removes a range of players from the variable. Will ignore duplicates.
        /// </summary>
        /// <param name="selector">The selector by which user gets info.</param>
        /// <returns>Information according to the selector.</returns>
        public string Show(string selector = "NAME")
        {
            IOrderedEnumerable<string> display = PlayerList.Select(ply =>
            {
                return selector switch
                {
                    "NAME" => ply.Nickname,
                    "DISPLAYNAME" or "DPNAME" => ply.DisplayNickname,
                    "USERID" or "UID" => ply.UserId,
                    "PLAYERID" or "PID" => ply.Id.ToString(),
                    "ROLE" => ply.Role.Type.ToString(),
                    "TEAM" => ply.Role.Team.ToString(),
                    "ROOM" => ply.CurrentRoom.Type.ToString(),
                    "ZONE" => ply.Zone.ToString(),
                    "HP" or "HEALTH" => ply.Health.ToString(),
                    "INVCOUNT" => ply.Items.Count.ToString(),
                    "INV" => string.Join("|", ply.Items.Select(item => CustomItem.TryGet(item, out CustomItem ci) ? ci.Name : item.Type.ToString())),
                    "HELDITEM" => (CustomItem.TryGet(ply.CurrentItem, out CustomItem ci) ? ci.Name : ply.CurrentItem?.Type.ToString()) ?? ItemType.None.ToString(),
                    "GOD" => ply.IsGodModeEnabled.ToString().ToUpper(),
                    "POS" => $"{ply.Position.x} {ply.Position.y} {ply.Position.z}",
                    "POSX" => ply.Position.x.ToString(),
                    "POSY" => ply.Position.y.ToString(),
                    "POSZ" => ply.Position.z.ToString(),
                    "TIER" when ply.Role is Scp079Role scp079role => scp079role.Level.ToString(),
                    "TIER" => "0",
                    "GROUP" => ply.GroupName,
                    "CUFFED" => ply.IsCuffed.ToString().ToUpper(),
                    "CUSTOMINFO" or "CINFO" or "CUSTOMI" => ply.CustomInfo.ToString(),
                    "XSIZE" => ply.Scale.x.ToString(),
                    "YSIZE" => ply.Scale.y.ToString(),
                    "ZSIZE" => ply.Scale.z.ToString(),
                    "KILLS" => MainPlugin.Handlers.PlayerKills.TryGetValue(ply, out int v) ? v.ToString() : "0",
                    "EFFECTS" when ply.ActiveEffects.Count() != 0 => string.Join("|", ply.ActiveEffects.Select(eff => eff.name)),
                    "EFFECTS" => "NONE",
                    _ => $"Invalid property '{selector}'.",
                };
            }).OrderBy(s => s);
            return string.Join(", ", display).Trim();
        }
    }
}
