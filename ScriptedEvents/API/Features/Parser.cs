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
    using Exiled.API.Features.Roles;
    using Exiled.CustomItems.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// A class used to store and retrieve all variables.
    /// </summary>
    public static class Parser
    {
        public delegate bool TryParseDelegate<T>(string input, out T result);

        public static bool TryCast<T>(TryParseDelegate<T> tryParseFunc, string input, Script source, out T result, out ErrorInfo? errorInfo)
        {
            var success = tryParseFunc(ReplaceContaminatedValueSyntax(input, source), out result);

            errorInfo = success
                ? null
                : Error(
                    $"Invalid cast attempt for type {typeof(T).Name}'",
                    $"Value '{input}' is not convertable to a value of type '{typeof(T).Name}'");

            return success;
        }

        /// <summary>
        /// Attempts to parse a string input into <typeparamref name="T"/>, where T is an <see cref="Enum"/>. Functionally similar to <see cref="Enum.TryParse{TEnum}(string, out TEnum)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <typeparam name="T">The Enum type to cast to.</typeparam>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryGetEnum<T>(string input, out T result, Script source, out ErrorInfo? errorInfo)
            where T : struct, Enum
        {
            input = input.Trim();
            input = ReplaceContaminatedValueSyntax(input, source);
            var success = Enum.TryParse(input, true, out result);

            errorInfo = success
                ? null
                : Error(
                    $"Invalid input for enum '{typeof(T).Name}'",
                    $"Provided input '{input}' could not be converted to the enum '{typeof(T).Name}'");

            return success;
        }

        public static bool TryGetEnumArray<T>(string input, out T[] result, Script source)
            where T : struct, Enum
        {
            input = input.Trim();
            input = ReplaceContaminatedValueSyntax(input, source);
            IEnumerable<T> resInIEnumerable = Array.Empty<T>();

            foreach (string item in input.Split('|'))
            {
                if (!Enum.TryParse(item, true, out T value))
                {
                    result = Array.Empty<T>();
                    return false;
                }

                resInIEnumerable = resInIEnumerable.Append(value);
            }

            result = resInIEnumerable.ToArray();
            return true;
        }

        /// <summary>
        /// Try-get a <see cref="Door"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="doors">The doors.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetDoors(string input, out Door[] doors, Script source, out ErrorInfo? errorInfo)
        {
            IEnumerable<Door> doorList;
            if (input is "*" or "ALL")
            {
                doorList = Door.List;
            }
            else if (TryGetEnum(input, out ZoneType zt, source, out _))
            {
                doorList = Door.List.Where(d => d.Zone.HasFlag(zt));
            }
            else if (TryGetEnum(input, out DoorType dt, source, out _))
            {
                doorList = Door.List.Where(d => d.Type == dt);
            }
            else if (TryGetEnum(input, out RoomType rt, source, out _))
            {
                doorList = Door.List.Where(d => d.Room?.Type == rt);
            }
            else
            {
                doorList = Door.List.Where(d => string.Equals(d.Name, input, StringComparison.CurrentCultureIgnoreCase));
            }

            doors = doorList.ToArray().Where(d => d.IsElevator is false && AirlockController.Get(d) is null).ToArray();

            var found = doors.Length > 0;
            errorInfo = found
                ? null
                : Error("Invalid door list input", $"Specified input '{input}' is not a valid representation of a list of doors.");

            return found;
        }

        /// <summary>
        /// Try-get a <see cref="Lift"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="lifts">The lift objects.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetLifts(string input, out Lift[] lifts, Script source, out ErrorInfo? errorInfo)
        {
            IEnumerable<Lift> liftList;
            if (input is "*" or "ALL")
            {
                liftList = Lift.List;
            }
            else if (TryGetEnum(input, out ElevatorType et, source, out _))
            {
                liftList = Lift.List.Where(l => l.Type == et);
            }
            else
            {
                liftList = Lift.List.Where(l => string.Equals(l.Name, input, StringComparison.CurrentCultureIgnoreCase));
            }

            lifts = liftList.ToArray();

            var found = lifts.Length > 0;
            errorInfo = found
                ? null
                : Error("Invalid lift list input", $"Specified input '{input}' is not a valid representation of a list of lifts.");

            return found;
        }

        /// <summary>
        /// Try-get a <see cref="Room"/> array given an input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="source">The script source.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetRooms(string input, out Room[] rooms, Script source, out ErrorInfo? errorInfo)
        {
            IEnumerable<Room> roomList;
            if (input is "*" or "ALL")
            {
                roomList = Room.List;
            }
            else if (TryGetEnum(input, out ZoneType zt, source, out _))
            {
                roomList = Room.List.Where(room => room.Zone.HasFlag(zt));
            }
            else if (TryGetEnum(input, out RoomType rt, source, out _))
            {
                roomList = Room.List.Where(d => d.Type == rt);
            }
            else
            {
                roomList = Room.List.Where(d => string.Equals(d.Name, input, StringComparison.CurrentCultureIgnoreCase));
            }

            rooms = roomList.ToArray();

            var found = rooms.Length > 0;
            errorInfo = found
                ? null
                : Error("Invalid room list input", $"Specified input '{input}' is not a valid representation of a list of rooms.");

            return found;
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
        public static bool TryGetPlayers(string input, int? amount, out IEnumerable<Player> players, Script source, out ErrorTrace? trace)
        {
            input = input.RemoveWhitespace();
            List<Player> list;

            if (input.ToUpper() is "*" or "ALL")
            {
                Log($"Input {input.ToUpper()} specifies all players on the server.");
                list = Player.List.ToList();
            }
            else if (VariableSystem.TryGetPlayersFromVariable(input, source, out var result, false, out trace))
            {
                list = result.ToList();
                Log($"Input {input} is a valid variable. Fetch got {list.Count} players.");
            }
            else
            {
                trace!.Append(Error("Invalid player reference", $"Input '{input}' is not a valid reference to a list of players (like '*') and is not a variable."));
                players = Array.Empty<Player>();
                return false;
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
            players = list;
            trace = default;
            return true;

            void Log(string msg)
            {
                Logger.Debug($"[SEParser] [TryGetPlayers] {msg}", source);
            }
        }

        /// <summary>
        /// Isolates all values like.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <param name="captureVariables">Should capture variables.</param>
        /// <param name="captureDynActs">Should capture dynamic action results.</param>
        /// <param name="captureAccessors">Should capture accessors.</param>
        /// <returns>The variables used within the string.</returns>
        public static (Match[] variables, Match[] dynamicActions, Match[] accessors) IsolateValueSyntax(string input, Script source, bool captureVariables = true, bool captureDynActs = true, bool captureAccessors = true)
        {
            var empty = Array.Empty<Match>();
            var variables = empty;
            var dynamicActions = empty;
            var accessors = empty;

            if (captureVariables && input.Length >= 2)
            {
                variables = Regex.Matches(input, @"@\w+").Cast<Match>()
                    .Concat(Regex.Matches(input, @"\$\w+").Cast<Match>())
                    .ToArray();
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

        public static bool TryGetAccessor(string initialInput, Script source, out string result, out ErrorTrace? error)
        {
            void Log(string msg)
            {
                Logger.Debug($"[SEParser] [TryGetAccessor] {msg}", source);
            }

            // "<PLR:ROLE>"
            // "<PLR>"
            result = string.Empty;

            // "PLR:ROLE"
            // "PLR"
            string input = initialInput.Substring(1, initialInput.Length - 2);

            // ["PLR", "ROLE"]
            // ["PLR"]
            var parts = input.Split(':');

            switch (parts.Length)
            {
                case > 2:
                    error = Error(
                        $"Failed parsing the '{initialInput}' accessor",
                        $"Accessor structure looks like '<PlayerVariable:Property>', where parts of it are 'PlayerVariable' and 'Property', but provided input '{initialInput}' defines an accessor with more than 2 parts ('{string.Join("', '", parts)}').")
                        .ToTrace();
                    return false;
                case 1:
                    // ["PLR"] -> ["PLR", "NAME"]
                    parts = parts.Append("NAME").ToArray();
                    break;
            }

            if (!VariableSystem.TryGetPlayersFromVariable(parts[0], source, out var plrRes, true, out var trace))
            {
                trace!.Append(Error($"Failed parsing the '{initialInput}' accessor", $"Can't retreive a player from variable {parts[0]}."));
                error = trace;
                return false;
            }

            var players = plrRes.ToArray();

            if (players.Length != 1)
            {
                error = Error(
                    $"Failed parsing the '{initialInput}' accessor",
                    $"Provided player variable '{parts[0]}' contains {players.Length} players, but accessors require a variable with exactly 1 player.")
                    .ToTrace();
                return false;
            }

            var ply = players.First();

            if (ply is null)
                throw new ArgumentException("Invalid player variable", nameof(initialInput));

            try
            {
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
                            ? string.Join(
                                "|",
                                ply.Items.Select(item =>
                                    CustomItem.TryGet(item, out var ci1) ? ci1?.Name : item.Type.ToString()))
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
                            if (p.Role is not Scp096Role scp096 || !scp096.Targets.Contains(ply)) continue;
                            result = "TRUE";
                            break;
                        }

                        break;

                    case "ISWATCHING173":
                        result = "FALSE";
                        foreach (Player p in Player.Get(RoleTypeId.Scp173))
                        {
                            if (p.Role is not Scp173Role scp173 || !scp173.ObservingPlayers.Contains(ply)) continue;
                            result = "TRUE";
                            break;
                        }

                        break;

                    default:
                        result = ply.SessionVariables.ContainsKey(parts[1])
                            ? ply.SessionVariables[parts[1]].ToString()
                            : "UNDEFINED";
                        break;
                }
            }
            catch (NullReferenceException)
            {
                result = "NONE";
            }

            Log("Success! Returning " + result);
            error = default;
            return true;
        }

        /// <summary>
        /// Replaces all the occurrences of a value syntax in a string with regular text.
        /// </summary>
        /// <param name="input">The string to perform the replacements on.</param>
        /// <param name="script">The script that is currently running to replace values.</param>
        /// <returns>The modified string.</returns>
        public static string ReplaceContaminatedValueSyntax(string input, Script script)
        {
            void Log(string msg)
            {
                Logger.Debug($"[SEParser] [RCVS] {msg}", script);
            }

            var values = IsolateValueSyntax(input, script);
            StringBuilder output = new(input);

            // variables can appear in dynActs and it can do some wonky stuff
            // so remove variables if they are fully encapsulated in a dynact
            List<Match> filteredVariables = new();
            foreach (var variable in values.variables)
            {
                bool isEnclosed = false;

                foreach (var dynAct in values.dynamicActions)
                {
                    if (dynAct.Index > variable.Index || dynAct.Index + dynAct.Length < variable.Index + variable.Length)
                        continue;

                    isEnclosed = true;
                    break;
                }

                if (!isEnclosed)
                {
                    filteredVariables.Add(variable);
                }
            }

            // Handle accessors in reverse order
            for (int i = values.accessors.Length - 1; i >= 0; i--)
            {
                var accssr = values.accessors[i];

                if (!TryGetAccessor(accssr.Value, script, out var res, out var errorTrace))
                {
                    Logger.ScriptError(errorTrace!, script);
                    continue;
                }

                // Perform the replacement if the index/length is valid
                if (accssr.Index >= 0 && accssr.Index < output.Length &&
                    accssr.Index + accssr.Length <= output.Length)
                {
                    output.Replace(accssr.Value, res, accssr.Index, accssr.Length);
                }
                else
                {
                    Logger.Warn($"Invalid index/length for accessor: Index={accssr.Index}, Length={accssr.Length}, OutputLength={output.Length}");
                }
            }

            // Handle dynamic actions in reverse order
            for (int i = values.dynamicActions.Length - 1; i >= 0; i--)
            {
                var dynact = values.dynamicActions[i];

                if (!TryGetDynamicActionResult<string>(dynact.Value, script, out var res, out var error))
                {
                    Logger.ScriptError($"[Dynamic action] {error}", script);
                    continue;
                }

                // Perform the replacement if the index/length is valid
                if (dynact.Index >= 0 && dynact.Index < output.Length &&
                    dynact.Index + dynact.Length <= output.Length)
                {
                    output.Replace(dynact.Value, res, dynact.Index, dynact.Length);
                }
                else
                {
                    Logger.Warn($"Invalid index/length for dynamic action: Index={dynact.Index}, Length={dynact.Length}, OutputLength={output.Length}");
                }
            }

            // Handle filtered variables in reverse order
            for (int i = filteredVariables.Count - 1; i >= 0; i--)
            {
                var varbl = filteredVariables[i];

                if (!VariableSystem.TryGetVariable<ILiteralVariable>(varbl.Value, script, out var res, false, out var errorTrace) || res is null)
                {
                    Logger.ScriptError(errorTrace!, script);
                    continue;
                }

                // Perform the replacement if the index/length is valid
                if (varbl.Index >= 0 && varbl.Index < output.Length &&
                    varbl.Index + varbl.Length <= output.Length)
                {
                    output.Replace(varbl.Value, res.String(), varbl.Index, varbl.Length);
                }
                else
                {
                    Logger.Warn($"Invalid index/length for variable: Index={varbl.Index}, Length={varbl.Length}, OutputLength={output.Length}");
                }
            }

            return output.ToString();
        }

        private static bool TryGetDynamicActionResult<T>(string rawInput, Script script, out T output, out ErrorTrace? errorTrace)
        {
            // "{LIMIT:@PLAYERS:2}"
            var input = rawInput.Trim();
            if (typeof(T) == typeof(string))
            {
                output = (T)(object)string.Empty;
            }
            else if (typeof(T) == typeof(Player[]))
            {
                output = (T)(object)Array.Empty<Player[]>();
            }
            else
            {
                throw new ArgumentException(typeof(T).FullName);
            }

            if (!input.StartsWith("{") || !input.EndsWith("}"))
            {
                errorTrace = Error(
                    "Dynamic action parsing error",
                    $"Provided dynamic action '{rawInput}' is not surrounded by '{{}}' brackets.")
                    .ToTrace();
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

            if (!MainPlugin.ScriptModule.TryGetActionType(actionName, out var actionToExtract, out var error))
            {
                errorTrace = Error(
                        "Dynamic action parsing error",
                        $"Provided action '{actionName}' is not a valid action.")
                    .ToTrace();
                return false;
            }

            switch (actionToExtract)
            {
                case null:
                    throw new ArgumentNullException(nameof(actionToExtract));

                case ITimingAction:
                    errorTrace = Error(
                        "Dynamic action logic error",
                        $"Provided action '{actionName}' is a timing action, which is not supported.")
                        .ToTrace();
                    return false;
            }

            if (actionToExtract is not IReturnValueAction)
            {
                errorTrace = Error(
                    $"Dynamic action '{actionToExtract.Name}' does not return any values",
                    $"Provided action is not designated as an action returning values, which is required for dynamic actions.")
                    .ToTrace();
                return false;
            }

            if (!ScriptModule.TryRunAction(script, actionToExtract, out var resp, out _, arguments))
            {
                resp!.ErrorTrace!.Append(Error(
                    $"Dynamic action '{actionToExtract.Name}' failed while running",
                    "Provided action failed while running, see inner exception for details."));

                errorTrace = resp.ErrorTrace;
                return false;
            }

            if (resp == null || resp.ResponseVariables.Length == 0)
            {
                errorTrace = Error(
                        $"Dynamic action '{actionToExtract.Name}' did not return any values",
                        "Provied action did not return any values, which is required for dynamic actions.")
                    .ToTrace();
                return false;
            }

            if (resp.ResponseVariables.Length > 1)
            {
                // Log("Action returned more than 1 value. Using the first one as default.");
            }

            var response = resp.ResponseVariables[0];

            if (response is not T value)
            {
                errorTrace = Error(
                        $"Dynamic action '{actionToExtract}' returned an invalid value",
                        $"Action returned a value of type {response.GetType().Name}, which does not match the expected type {typeof(T).Name}.")
                    .ToTrace();
                return false;
            }

            output = value;
            errorTrace = null;
            return true;
        }

        private static ErrorInfo Error(string name, string description)
        {
            return new(name, description, "Parser");
        }
    }
}