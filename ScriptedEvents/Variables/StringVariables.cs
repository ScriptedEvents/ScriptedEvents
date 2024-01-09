namespace ScriptedEvents.Variables.Strings
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.CustomItems.API.Features;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
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
            new PlayerData(),

            new Len(),
            new Command(),
            new Show(),

            new RandomRoom(),
            new Log(),
        };
    }

    public class PlayerData : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERDATA}";

        /// <inheritdoc/>
        public string Description => "Retrieves the value of a key from a player's player data.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("player", typeof(string), "The player to get the key from. MUST BE ONLY ONE PLAYER", true),
            new Argument("keyName", typeof(string), "The name of the key.", true),
        };

        /// <inheritdoc/>
        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length < 2)
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "player", "keyName " }));

                if (VariableSystem.TryGetPlayers(Arguments[0], out IEnumerable<Player> players, Source, false))
                {
                    List<Player> playerList = players.ToList();
                    if (playerList.Count > 1)
                        throw new ArgumentException("The 'PLAYERDATA' variable only works with one player!");
                    if (playerList[0].SessionVariables.ContainsKey(Arguments[1]))
                        return playerList[0].SessionVariables[Arguments[1]].ToString();
                }

                return "NONE";
            }
        }
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
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "name" }));
                }

                var conditionVariable = VariableSystem.GetVariable(Arguments[0], Source, false);
                if (conditionVariable.Item1 is not null)
                {
                    if (conditionVariable.Item1 is not IPlayerVariable variable)
                    {
                        throw new ArgumentException($"The provided value '{conditionVariable.Item1.Name}' has no associated players. [Error Code: SE-133]");
                    }

                    return variable.Players.Count();
                }

                throw new ArgumentException($"The provided value '{Arguments[0]}' is not a valid variable. [Error Code: SE-132]");
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
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "name" }));
                }

                if (VariableSystem.TryGetVariable(Arguments[0], out IConditionVariable variable, out _, Source, false))
                {
                    if (variable is IArgumentVariable)
                    {
                        return "ERROR: ARGUMENT VARIABLE NOT SUPPORTED IN 'C'. PLEASE USE CUSTOM VARIABLE INSTEAD.";
                    }

                    if (variable is not IPlayerVariable plrVar)
                    {
                        throw new ArgumentException($"The provided value '{variable.Name}' has no associated players. [Error Code: SE-133]");
                    }

                    if (plrVar.Players.Count() == 0)
                        return string.Empty;

                    return string.Join(".", plrVar.Players.Select(plr => plr.Id.ToString()));
                }

                throw new ArgumentException($"The provided value '{Arguments[0]}' is not a valid variable. [Error Code: SE-132]");
            }
        }
    }

    public class Show : IStringVariable, IArgumentVariable, INeedSourceVariable, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "{SHOW}";

        /// <inheritdoc/>
        public string Description => "Reveal certain properties about the players in a player variable.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name of the player variable to show.", true),
            new Argument("selector", typeof(string), "The type to show. Defaults to \"NAME\".", false),
        };

        /// <inheritdoc/>
        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "name" }));
                }

                string selector = "NAME";

                if (Arguments.Length > 1)
                    selector = Arguments[1].ToUpper();

                if (VariableSystem.TryGetPlayers(Arguments[0], out IEnumerable<Player> players, Source, false))
                {
                    IOrderedEnumerable<string> display = players.Select(ply =>
                    {
                        return selector switch
                        {
                            "NAME" => ply.Nickname,
                            "DISPLAYNAME" => ply.DisplayNickname,
                            "USERID" => ply.UserId,
                            "PLAYERID" => ply.Id.ToString(),
                            "ROLE" => ply.Role.Type.ToString(),
                            "TEAM" => ply.Role.Team.ToString(),
                            "ROOM" => ply.CurrentRoom.Type.ToString(),
                            "ZONE" => ply.Zone.ToString(),
                            "HP" or "HEALTH" => ply.Health.ToString(),
                            "INVCOUNT" => ply.Items.Count.ToString(),
                            "INV" => string.Join(", ", ply.Items.Select(item => CustomItem.TryGet(item, out CustomItem ci) ? ci.Name : item.Type.ToString())),
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
                            "ONSERVER" => ply.IsVerified.ToString().ToUpper(),
                            _ => ply.Nickname,
                        };
                    }).OrderBy(s => s);
                    return string.Join(", ", display).Trim();
                }

                throw new ArgumentException($"The provided value '{Arguments[0]}' is not a valid variable or has no associated players. [Error Code: SE-131]");
            }
        }

        /// <inheritdoc/>
        public string LongDescription => @"This variable is designed to only be used with a player variable containing one player. However, it CAN be used with multiple players, and will list the display in the form of a comma-separated list.
Do not use this variable for using player variables in commands. Use the 'C' variable for this.
The following options are valid selector options:
- NAME
- DISPLAYNAME
- USERID
- PLAYERID
- ROLE
- TEAM
- ROOM
- ZONE
- HP / HEALTH
- INVCOUNT
- INV
- HELDITEM
- GOD
- POS
- POSX
- POSY
- POSZ
- TIER
- GROUP
- CUFFED
- ONSERVER
Invalid options will default to the 'NAME' selector.";
    }

    public class RandomRoom : IStringVariable, IArgumentVariable, INeedSourceVariable
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
        public Script Source { get; set; }

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                ZoneType filter = ZoneType.Unspecified;

                if (Arguments.Length > 0 && !VariableSystem.TryParse(Arguments[0], out filter, Source, false))
                {
                    throw new ArgumentException($"Provided value '{Arguments[0]}' is not a valid ZoneType.");
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


    public class Log : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{LOG}";

        /// <inheritdoc/>
        public string Description => "Shows the name of the variable with its value. Useful for quick debugging.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variable", typeof(string), "The name of the variable.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, new[] { "variable" }));
                }

                if (!VariableSystem.TryGetVariable(Arguments[0], out IConditionVariable variable, out _, Source, false))
                {
                    throw new ArgumentException("bruh");
                }
 
                if (variable is not IStringVariable value)
                {
                    throw new ArgumentException($"Provided variable '{Arguments[0]}' is not a valid variable.");
                }

                return $"{variable.Name} = {value.Value}";
            }
        }
    }
}
