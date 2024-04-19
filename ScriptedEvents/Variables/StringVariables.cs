namespace ScriptedEvents.Variables.Strings
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;
    using Exiled.CustomItems.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
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
            new Index(),
            new RandomDoor(),
            new RandomItem(),
            new ReplaceVariable(),
        };
    }

    public class PlayerData : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERDATA}";

        /// <inheritdoc/>
        public string Description => "Retrieves the value of a key from a player's player data.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("player", typeof(IPlayerVariable), "The player to get the key from. MUST BE ONLY ONE PLAYER", true),
            new Argument("keyName", typeof(string), "The name of the key.", true),
        };

        /// <inheritdoc/>
        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                List<Player> players = ((IPlayerVariable)Arguments[0]).Players.ToList();
                string key = (string)Arguments[1];

                if (players.Count > 1)
                    throw new ArgumentException("The 'PLAYERDATA' variable only works with one player!");
                if (players[0].SessionVariables.ContainsKey(key))
                    return players[0].SessionVariables[key].ToString();

                return "NONE";
            }
        }
    }

    public class Len : IFloatVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{LEN}";

        /// <inheritdoc/>
        public string Description => "[OBSOLETE] Reveals the length of a player variable.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(IPlayerVariable), "The name of the player variable to retrieve the length of.", true),
        };

        /// <inheritdoc/>
        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                IPlayerVariable variable = (IPlayerVariable)Arguments[0];
                return variable.Players.Count();
            }
        }
    }

    public class Command : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{C}";

        /// <inheritdoc/>
        public string Description => "Converts a player variable into a format to use with commands.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(IPlayerVariable), "The name of the player variable.", true),
        };

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IPlayerVariable variable = (IPlayerVariable)Arguments[0];

                if (variable is IArgumentVariable)
                {
                    throw new ArgumentException(ErrorGen.Get(ErrorCode.UnsupportedArgumentVariables, Name));
                }

                if (variable.Players.Count() == 0)
                    return string.Empty;

                return string.Join(".", variable.Players.Select(plr => plr.Id.ToString()));
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
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(IPlayerVariable), "The name of the player variable to show.", true),
            new Argument("selector", typeof(string), "The type to show. Defaults to \"NAME\".", false),
        };

        /// <inheritdoc/>
        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                string selector = "NAME";

                if (Arguments.Length > 1)
                    selector = Arguments[1].ToUpper();

                IEnumerable<Player> players = ((IPlayerVariable)Arguments[0]).Players;
                IOrderedEnumerable<string> display = players.Select(ply =>
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
                        "KILLS" => MainPlugin.Handlers.PlayerKills.TryGetValue(ply, out int v) ? v.ToString() : "-1",
                        "EFFECTS" when ply.ActiveEffects.Count() != 0 => string.Join("|", ply.ActiveEffects.Select(eff => eff.name)),
                        "EFFECTS" => "None",
                        "NOCLIP" => ply.Role is FpcRole role ? role.IsNoclipEnabled.ToUpper() : "FALSE",
                        _ => ply.Nickname,
                    };
                    }).OrderBy(s => s);
                return string.Join(", ", display).Trim();
            }
        }

        /// <inheritdoc/>
        public string LongDescription => @"This variable is designed to only be used with a player variable containing one player. However, it CAN be used with multiple players, and will list the display in the form of a comma-separated list.
Do not use this variable for using player variables in commands. Use the 'C' variable for this.
The following options are valid selector options:
- NAME
- DISPLAYNAME / DPNAME
- USERID / UID
- PLAYERID / PID
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
- CUSTOMINFO / CINFO / CUSTOMI
- XSIZE 
- YSIZE
- ZSIZE
- KILLS
- EFFECTS
- NOCLIP
Invalid options will default to the 'NAME' selector.";
    }

    public class RandomRoom : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{RANDOMROOM}";

        /// <inheritdoc/>
        public string Description => "Gets the RoomType of a random room. Can be filtered by zone.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

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

                if (Arguments.Length > 0)
                    filter = (ZoneType)Arguments[0];

                IEnumerable<Room> validRooms = Room.List.Where(room => room.Type != RoomType.Pocket);

                if (filter is not ZoneType.Unspecified)
                    validRooms = validRooms.Where(room => room.Zone.HasFlag(filter));

                List<Room> newList = validRooms.ToList();
                return newList[UnityEngine.Random.Range(0, newList.Count)].Type.ToString();
            }
        }
    }

    public class ReplaceVariable : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{REPLACE}";

        /// <inheritdoc/>
        public string Description => "Replaces character sequneces.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("variableName", typeof(IStringVariable), "The variable on which the operation will be performed.", true),
            new Argument("targetSequence", typeof(string), "The sequence which will be replaced.", true),
            new Argument("replacingSequence", typeof(string), "The value to replace with.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IStringVariable processedVar = (IStringVariable)Arguments[0];
                return processedVar.String().Replace(Arguments[1].ToString(), Arguments[2].ToString());
            }
        }

        public Script Source { get; set; }
    }

    public class RandomDoor : IStringVariable, INeedSourceVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{RANDOMDOOR}";

        /// <inheritdoc/>
        public string Description => "Gets the DoorType of a random door. Can be filtered by zone.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

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

                if (Arguments.Length > 0)
                    filter = (ZoneType)Arguments[0];

                IEnumerable<Door> validDoors = Door.List;

                if (filter is not ZoneType.Unspecified)
                {
                    validDoors = validDoors.Where(door => door.Zone.HasFlag(filter));
                }

                List<Door> newList = validDoors.ToList();
                return newList[UnityEngine.Random.Range(0, newList.Count)].Type.ToString();
            }
        }
    }

    public class RandomItem : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{RANDOMITEM}";

        /// <inheritdoc/>
        public string Description => "Gets the ItemType of a random item.";

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                ItemType[] items = (ItemType[])Enum.GetValues(typeof(ItemType));
                return items[UnityEngine.Random.Range(0, items.Length)].ToString();
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
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variable", typeof(IConditionVariable), "The name of the variable.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IConditionVariable variable = (IConditionVariable)Arguments[0];

                return variable switch
                {
                    IStringVariable strV => $"{strV.Name} = {strV.Value}",
                    IFloatVariable floatV => $"{floatV.Name} = {floatV.Value}",
                    ILongVariable longV => $"{longV.Name} = {longV.Value}",
                    IBoolVariable boolV => $"{boolV.Name} = {boolV.Value}",
                    _ => $"{variable.Name} = UNKNOWN VALUE",
                };
            }
        }
    }

    public class Index : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{INDEX}";

        /// <inheritdoc/>
        public string Description => "Extract a certain part of a variables value using an index.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variable", typeof(IStringVariable), "The name of the variable.", true),
             new Argument("index", typeof(int), "The place from which the value should be taken.", true),
             new Argument("listSplitChar", typeof(char), "The character that will split the variable into a list. Must be a 1 character only.", false),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IStringVariable value = (IStringVariable)Arguments[0];
                int index = (int)Arguments[1];
                string result;

                if (Arguments.Length >= 3)
                {
                    char delimiter = (char)Arguments[2];

                    string[] resultList = value.Value.Split(delimiter);

                    result = resultList[index].Trim();
                }
                else
                {
                    result = value.Value[index].ToString();
                }

                return result;
            }
        }
    }
}
