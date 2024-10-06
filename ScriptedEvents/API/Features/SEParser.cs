namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.CustomItems.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    /// <summary>
    /// A class used to store and retrieve all variables.
    /// </summary>
    public static class SEParser
    {
        public delegate bool TryParseDelegate<T>(string input, out T result);

        public static bool Cast<T>(TryParseDelegate<T> tryParseFunc, string input, Script source, out T result)
        {
            return tryParseFunc(ReplaceContaminatedValueSyntax(input, source), out result);
        }

        /// <summary>
        /// Attempts to parse a string input into <typeparamref name="T"/>, where T is an <see cref="Enum"/>. Functionally similar to <see cref="Enum.TryParse{TEnum}(string, out TEnum)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <typeparam name="T">The Enum type to cast to.</typeparam>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParseEnum<T>(string input, out T result, Script source, bool requireBrackets = true)
            where T : struct, Enum
        {
            input = input.Trim();
            if (Enum.TryParse(input, true, out result))
            {
                return true;
            }

            if (VariableSystemV2.TryGetVariable(input, source, out VariableResult vresult, requireBrackets) && vresult.ProcessorSuccess)
            {
                return TryParseEnum(vresult.String(), out result, source, requireBrackets);
            }

            return false;
        }

        public static object? ParseEnum(string input, Type enumType, Script source, bool requireBrackets = true)
        {
            input = ReplaceContaminatedValueSyntax(input, source);
            var result = Enum.Parse(enumType, input, true);
            return result;
        }

        /// <summary>
        /// Try-get a <see cref="Door"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="doors">The doors.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetDoors(string input, out Door[] doors, Script source)
        {
            List<Door> doorList;
            if (input is "*" or "ALL")
            {
                doorList = Door.List.ToList();
            }
            else if (TryParseEnum(input, out ZoneType zt, source))
            {
                doorList = Door.List.Where(d => d.Zone.HasFlag(zt)).ToList();
            }
            else if (TryParseEnum(input, out DoorType dt, source))
            {
                doorList = Door.List.Where(d => d.Type == dt).ToList();
            }
            else if (TryParseEnum(input, out RoomType rt, source))
            {
                doorList = Door.List.Where(d => d.Room?.Type == rt).ToList();
            }
            else
            {
                doorList = Door.List.Where(d => string.Equals(d.Name, input, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            doors = doorList.Where(d => d.IsElevator is false && d.Type is not DoorType.Scp914Door && d.Type is not DoorType.Scp079First && d.Type is not DoorType.Scp079Second && AirlockController.Get(d) is null).ToArray();
            return doorList.Count > 0;
        }

        /// <summary>
        /// Try-get a <see cref="Lift"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="lifts">The lift objects.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetLifts(string input, out Lift[] lifts, Script source)
        {
            List<Lift> liftList = ListPool<Lift>.Pool.Get();
            if (input is "*" or "ALL")
            {
                liftList = Lift.List.ToList();
            }
            else if (TryParseEnum(input, out ElevatorType et, source))
            {
                liftList = Lift.List.Where(l => l.Type == et).ToList();
            }
            else
            {
                liftList = Lift.List.Where(l => l.Name.ToLower() == input.ToLower()).ToList();
            }

            lifts = ListPool<Lift>.Pool.ToArrayReturn(liftList);
            return lifts.Length > 0;
        }

        /// <summary>
        /// Try-get a <see cref="Room"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetRooms(string input, out Room[] rooms, Script source)
        {
            List<Room> roomList = ListPool<Room>.Pool.Get();
            if (input is "*" or "ALL")
            {
                roomList = Room.List.ToList();
            }
            else if (TryParseEnum(input, out ZoneType zt, source))
            {
                roomList = Room.List.Where(room => room.Zone.HasFlag(zt)).ToList();
            }
            else if (TryParseEnum(input, out RoomType rt, source))
            {
                roomList = Room.List.Where(d => d.Type == rt).ToList();
            }
            else
            {
                roomList = Room.List.Where(d => d.Name.ToLower() == input.ToLower()).ToList();
            }

            rooms = ListPool<Room>.Pool.ToArrayReturn(roomList);
            return rooms.Length > 0;
        }

        /// <summary>
        /// Converts a string input into a player collection.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="amount">The maximum amount of players to give back, or <see langword="null"/> for unlimited.</param>
        /// <param name="collection">A <see cref="PlayerCollection"/> representing the players.</param>
        /// <param name="source">The script using the API. Used for per-script variables.</param>
        /// <param name="brecketsRequired">Are brackets required.</param>
        /// <returns>Whether or not the process errored.</returns>
        /// <remarks>This method differs from <see cref="VariableSystemV2.TryGetPlayers(string, Script, out PlayerCollection, bool)"/></remarks>
        public static bool TryGetPlayers(string input, int? amount, out PlayerCollection collection, Script source, bool brecketsRequired = true)
        {
            void Log(string msg)
            {
                Logger.Debug($"[SEParser] [TryGetPlayers] {msg}", source);
            }

            input = input.RemoveWhitespace();
            List<Player> list = new();

            if (input.ToUpper() is "*" or "ALL")
            {
                Log($"Input {input.ToUpper()} specifies all players on the server.");
                list = Player.List.ToList();
            }
            else if (VariableSystemV2.TryGetPlayers(input, source, out PlayerCollection playersFromVariable, brecketsRequired))
            {
                list = playersFromVariable.GetInnerList();
                Log($"Input {input} is a valid variable. Fetch got {list.Count} players.");
            }
            else
            {
                Log($"Input {input} is not a valid variable nor does it specify all players, trying to match directly.");
                Player match = Player.Get(input);
                if (match is not null)
                    list.Add(match);
            }

            if (MainPlugin.Configs.IgnoreOverwatch)
                list.RemoveAll(p => p.Role.Type is RoleTypeId.Overwatch);

            if (amount.HasValue && amount.Value > 0 && list.Count > 0)
            {
                Log("Amount of fetched players bigger than limit, removing players.");
                while (list.Count > amount.Value)
                {
                    list.PullRandomItem();
                }
            }

            // Return
            Log($"Complete! Returning {list.Count} players.");
            collection = new(list);
            return true;
        }

        /// <summary>
        /// Isolates all values like.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The variables used within the string.</returns>
        public static (Match[] variables, Match[] dynamicActions, Match[] accessors) IsolateValueSyntax(string input, Script source, bool captureVariables = true, bool captureDynActs = true, bool captureAccessors = true)
        {
            var empty = Array.Empty<Match>();
            var variables = empty;
            var dynamicActions = empty;
            var accessors = empty;

            if (captureVariables && input.Length >= 2)
            {
                variables = Regex.Matches(input, @"@\w+").Cast<Match>().ToArray();
            }

            if (captureDynActs && input.Length >= 3)
            {
                dynamicActions = Regex.Matches(input, @"\{[^{}\s]*\}").Cast<Match>().ToArray();
            }

            if (captureAccessors && input.Length >= 3)
            {
                accessors = Regex.Matches(input, @"<[^<>\s]*>").Cast<Match>().ToArray();
            }

            Logger.Debug($"[SEParser] [IsolateValueSyntax] From '{input}' retreived: {variables.Length} VARS, {dynamicActions.Length} DNCTS, {accessors.Length} ACSRS", source);
            return (variables, dynamicActions, accessors);
        }

        public static bool TryGetAccessor(string input, Script source, out string result)
        {
            void Log(string msg)
            {
                Logger.Debug($"[SEParser] [TryGetAccessor] {msg}", source);
            }

            // "<PLR:ROLE>"
            // "<PLR>"
            result = string.Empty;

            if (!input.StartsWith("<") || !input.EndsWith(">"))
            {
                Log("Fail. Accessor definers not present.");
                return false;
            }

            // "PLR:ROLE"
            // "PLR"
            input = input.Substring(1, input.Length - 2);

            if (input.Contains(' ') || input.Contains('<') || input.Contains('>'))
            {
                Log("Fail. After processing, accessor contains whitespace or definers.");
                return false;
            }

            // ["PLR", "ROLE"]
            // ["PLR"]
            var parts = input.Split(':');

            switch (parts.Length)
            {
                case > 2:
                    Log("Fail. After splitting, more than 2 parts defined.");
                    return false;
                case 1:
                    // ["PLR"] -> ["PLR", "NAME"]
                    parts = parts.Append("NAME").ToArray();
                    break;
            }

            if (!VariableSystemV2.TryGetPlayers(parts[0], source, out PlayerCollection players, false))
            {
                Log("Fail. Player variable invalid.");
                return false;
            }

            if (players.Length != 1)
            {
                Log("Fail. Player amount not equal 1.");
                return false;
            }

            var ply = players[0];

            switch (parts[1])
            {
                case "NAME":
                    result = ply.Nickname;
                    break;

                case "DISPLAYNAME" or "DPNAME":
                    result = ply.DisplayNickname;
                    break;

                case "USERID" or "UID":
                    result = ply.UserId;
                    break;

                case "PLAYERID" or "PID":
                    result = ply.Id.ToString();
                    break;

                case "ROLE":
                    result = ply.Role.Type.ToString();
                    break;

                case "TEAM":
                    result = ply.Role.Team.ToString();
                    break;

                case "ROOM":
                    result = ply.CurrentRoom.Type.ToString();
                    break;

                case "ZONE":
                    result = ply.Zone.ToString();
                    break;

                case "HP" or "HEALTH":
                    result = ply.Health.ToString();
                    break;

                case "ITEMCOUNT":
                    result = ply.Items.Count.ToString();
                    break;

                case "ITEMS":
                    result = ply.Items.Count > 0
                        ? string.Join("|", ply.Items.Select(item => CustomItem.TryGet(item, out var ci1) ? ci1?.Name : item.Type.ToString()))
                        : "NONE";
                    break;

                case "HELDITEM":
                    result = (CustomItem.TryGet(ply.CurrentItem, out var ci)
                        ? ci?.Name ?? "NONE"
                        : ply.CurrentItem?.Type.ToString()) ?? "NONE";
                    break;

                case "GOD":
                    result = ply.IsGodModeEnabled.ToUpper();
                    break;

                case "POS":
                    result = $"{ply.Position.x} {ply.Position.y} {ply.Position.z}";
                    break;

                case "POSX":
                    result = ply.Position.x.ToString();
                    break;

                case "POSY":
                    result = ply.Position.y.ToString();
                    break;

                case "POSZ":
                    result = ply.Position.z.ToString();
                    break;

                case "TIER" when ply.Role is Scp079Role scp079Role:
                    result = scp079Role.Level.ToString();
                    break;

                case "TIER":
                    result = "NONE";
                    break;

                case "GROUP":
                    result = ply.GroupName ?? "NONE";
                    break;

                case "CUFFED":
                    result = ply.IsCuffed.ToUpper();
                    break;

                case "CUSTOMINFO" or "CINFO" or "CUSTOMI":
                    result = !string.IsNullOrEmpty(ply.CustomInfo) ? ply.CustomInfo : "NONE";
                    break;

                case "XSIZE":
                    result = ply.Scale.x.ToString();
                    break;

                case "YSIZE":
                    result = ply.Scale.y.ToString();
                    break;

                case "ZSIZE":
                    result = ply.Scale.z.ToString();
                    break;

                case "KILLS":
                    result = MainPlugin.Handlers.PlayerKills.TryGetValue(ply, out int v) ? v.ToString() : "0";
                    break;

                case "EFFECTS" when ply.ActiveEffects.Any():
                    result = string.Join(", ", ply.ActiveEffects.Select(eff => eff.name));
                    break;

                case "EFFECTS":
                    result = "NONE";
                    break;

                case "USINGNOCLIP" when ply.Role is FpcRole role:
                    result = role.IsNoclipEnabled.ToUpper();
                    break;

                case "USINGNOCLIP":
                    result = "FALSE";
                    break;

                case "CANNOCLIP":
                    result = ply.IsNoclipPermitted.ToUpper();
                    break;

                case "STAMINA":
                    result = ply.Stamina.ToString();
                    break;

                case "ISSTAFF":
                    result = ply.RemoteAdminAccess.ToUpper();
                    break;

                case "AHP":
                    result = ply.ArtificialHealth.ToString();
                    break;

                case "IS096TARGET":
                    result = "FALSE";
                    foreach (Player p in Player.Get(RoleTypeId.Scp096))
                    {
                        if ((p.Role as Scp096Role).Targets.Contains(ply))
                        {
                            result = "TRUE";
                            break;
                        }
                    }

                    break;

                case "ISWATCHING173":
                    result = "FALSE";
                    foreach (Player p in Player.Get(RoleTypeId.Scp173))
                    {
                        if ((p.Role as Scp173Role).ObservingPlayers.Contains(ply))
                        {
                            result = "TRUE";
                            break;
                        }
                    }

                    break;

                default:
                    result = ply.SessionVariables.ContainsKey(parts[1])
                        ? ply.SessionVariables[parts[1]].ToString()
                        : "UNDEFINED";
                    break;
            }

            Log("Success! Returning " + result);
            return true;
        }

        /// <summary>
        /// Replaces all the occurrences of a value syntax in a string with regular text.
        /// </summary>
        /// <param name="input">The string to perform the replacements on.</param>
        /// <param name="script">The script that is currently running to replace values.</param>
        /// <returns>The modified string.</returns>
        /// <remarks>This is intended for strings that contain both regular text and value syntax. Otherwise, see <see cref="ReplaceVariable(string, Script, bool)"/>.</remarks>
        public static string ReplaceContaminatedValueSyntax(string input, Script script)
        {
            void Log(string msg)
            {
                Logger.Debug($"[SEParser] [RCVS] {msg}", script);
            }

            var values = IsolateValueSyntax(input, script);
            StringBuilder output = new(input);

            foreach (Match accssr in values.accessors)
            {
                if (!TryGetAccessor(accssr.Value, script, out string res))
                {
                    continue;
                }

                output.Replace(accssr.Value, res, accssr.Index, accssr.Length);
            }

            foreach (Match dynact in values.dynamicActions)
            {
                if (!TryGetDynamicAction(dynact.Value, script, out string res))
                {
                    continue;
                }

                output.Replace(dynact.Value, res, dynact.Index, dynact.Length);
            }

            foreach (Match varbl in values.variables)
            {
                if (!VariableSystemV2.TryGetVariable(varbl.Value, script, out VariableResult res) || res.Variable is null)
                {
                    continue;
                }

                output.Replace(varbl.Value, res.Variable.String(), varbl.Index, varbl.Length);
            }

            return output.ToString();
        }

        private static bool TryGetDynamicAction<T>(string input, Script script, out T? output)
        {
            if (typeof(T) != typeof(string) && typeof(T) != typeof(Player[]))
            {
                throw new ArgumentException(typeof(T).FullName);
            }

            // "{LIMIT:@PLAYERS:2}"
            input = input.Trim();
            output = default;

            if (!input.StartsWith("{") || !input.EndsWith("}"))
            {
                return false;
            }

            // "LIMIT:@PLAYERS:2
            input = input.Substring(1, input.Length - 2);

            // ["LIMIT", "@PLAYERS", "2"]
            var parts = input.Split(':');

            // "LIMIT"
            var actionName = parts[0];

            // ["@PLAYERS", "2"]
            var arguments = parts.Skip(1).ToArray();

            var act = MainPlugin.ScriptModule.TryGetActionType(actionName);
            if (act == null)
            {
                return false;
            }

            var actionToExtract = Activator.CreateInstance(act) as IAction;

            if (actionToExtract is ITimingAction)
            {
                // Logger.Log($"{actionToExtract.Name} is a timing action, which cannot be used with smart extractors.", LogType.Warning, script, currentline + 1);
                return false;
            }

            if (actionToExtract is not IReturnValueAction)
            {
                // Logger.Log($"{actionToExtract.Name} action does not return any values, therefore can't be used with smart accessors.", LogType.Warning, script, currentline + 1);
                return false;
            }

            if (!ScriptModule.TryRunAction(script, actionToExtract, out var resp, out var _, arguments))
            {
                return false;
            }

            if (resp == null || resp.ResponseVariables.Length == 0)
            {
                return false;
            }

            if (resp.ResponseVariables.Length > 1)
            {
                // Log("Action returned more than 1 value. Using the first one as default.");
            }

            var value = resp.ResponseVariables[0];

            if (value is not string)
            {
                return false;
            }

            output = value switch
            {
                string s => (T)(object)s,
                Player[] players => (T)(object)players,
                _ => throw new InvalidOperationException("Unsupported type")
            };

            return true;
        }
    }
}