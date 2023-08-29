namespace ScriptedEvents.Variables.Strings
{
    using System;
    using System.Collections.Generic;
#pragma warning disable SA1402 // File may only contain a single type
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class StringVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Strings";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Len(),
            new Command(),
            new Show(),

            new RandomRoom(),
        };
    }

    public class Len : IFloatVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{LEN}";

        /// <inheritdoc/>
        public string Description => "Reveals the length of a player variable.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name of the player variable to retrieve the length of.", true),
        };

        /// <inheritdoc/>
        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Arguments.Length == 0)
                    return -1f;

                var conditionVariable = VariableSystem.GetVariable(Arguments[0], Source, false);
                if (conditionVariable.Item1 is not null)
                {
                    if (conditionVariable.Item1 is not IPlayerVariable variable)
                    {
                        return -1f;
                    }

                    return variable.Players.Count();
                }

                return -1f;
            }
        }
    }

    public class Command : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{C}";

        /// <inheritdoc/>
        public string Description => "Convert a player variable into a format to use with commands.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(string), "The name of the player variable.", true),
        };

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length == 0)
                {
                    return "ERROR: MISSING PLAYER VARIABLE";
                }

                if (VariableSystem.TryGetVariable(Arguments[0], out IConditionVariable variable, out _, Source, false))
                {
                    if (variable is IArgumentVariable)
                    {
                        return "ERROR: ARGUMENT VARIABLE NOT SUPPORTED IN 'C'. PLEASE USE CUSTOM VARIABLE INSTEAD.";
                    }

                    if (variable is not IPlayerVariable plrVar)
                    {
                        return $"ERROR: No players associated with {variable.Name} variable";
                    }

                    if (plrVar.Players.Count() == 0)
                        return string.Empty;

                    return string.Join(".", plrVar.Players.Select(plr => plr.Id.ToString()));
                }

                return "ERROR: INVALID VARIABLE";
            }
        }
    }

    public class Show : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{SHOW}";

        /// <inheritdoc/>
        public string Description => "Reveal certain properties about the players in a player variable. This variable is designed to only be used with a player variable containing one player. However, it CAN be used with multiple players, and will list the display in the form of a comma-separated list.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name of the player variable to show.", true),
            new Argument("selector", typeof(string), "The type to show. Defaults to \"NAME\" Options: NAME, USERID, PLAYERID, ROLE, TEAM, ROOM, ZONE, HP, HEALTH, INV, INVCOUNT.", false),
        };

        /// <inheritdoc/>
        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length == 0)
                    return "ERROR: MISSING VARIABLE NAME";

                string selector = "NAME";

                if (Arguments.Length > 1)
                    selector = Arguments[1].ToUpper();

                var conditionVariable = VariableSystem.GetVariable(Arguments[0], Source, false);
                if (conditionVariable.Item1 is not null)
                {
                    if (conditionVariable.Item1 is not IPlayerVariable variable)
                    {
                        return $"ERROR: No players associated with {conditionVariable.Item1.Name} variable";
                    }

                    IOrderedEnumerable<string> display = variable.Players.Select(ply =>
                    {
                        return selector switch
                        {
                            "NAME" => ply.Nickname,
                            "USERID" => ply.UserId,
                            "PLAYERID" => ply.Id.ToString(),
                            "ROLE" => ply.Role.Type.ToString(),
                            "TEAM" => ply.Role.Team.ToString(),
                            "ROOM" => ply.CurrentRoom.Type.ToString(),
                            "ZONE" => ply.Zone.ToString(),
                            "HP" or "HEALTH" => ply.Health.ToString(),
                            "INVCOUNT" => ply.Items.Count.ToString(),
                            "INV" => string.Join(", ", ply.Items.Select(item => CustomItem.TryGet(item, out CustomItem ci) ? ci.Name : item.Type.ToString())),
                            "GOD" => ply.IsGodModeEnabled.ToString().ToUpper(),
                            "POS" => $"{ply.Position.x} {ply.Position.y} {ply.Position.z}",
                            "POSX" => ply.Position.x.ToString(),
                            "POSY" => ply.Position.y.ToString(),
                            "POSZ" => ply.Position.z.ToString(),
                            _ => ply.Nickname,
                        };
                    }).OrderBy(s => s);
                    return string.Join(", ", display);
                }

                return "ERROR: INVALID VARIABLE";
            }
        }
    }

    public class RandomRoom : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{RANDOMROOM}";

        /// <inheritdoc/>
        public string Description => "Gets the RoomType of a random room. Can be filtered by zone.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("zone", typeof(ZoneType), "A zone to filter by (optional).", false),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                ZoneType filter = ZoneType.Unspecified;

                if (Arguments.Length > 0 && !Enum.TryParse(Arguments[0], out filter))
                {
                    return "INVALID ZONETYPE FILTER PROVIDED";
                }

                IEnumerable<Room> validRooms = Room.List.Where(room => room.Type != RoomType.Pocket);

                if (filter is not ZoneType.Unspecified)
                {
                    validRooms = validRooms.Where(room => room.Zone.HasFlag(filter));
                }

                List<Room> newList = validRooms.ToList();
                return newList[UnityEngine.Random.Range(0, newList.Count)].Type.ToString();
            }
        }
    }
}
