namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using InventorySystem.Items.Usables.Scp330;
    using PlayerRoles;
    using Respawning;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public static class ArgumentProcessor
    {
        public static ArgumentProcessResult Process(Argument[] expected, string[] args, IScriptComponent action, Script source, bool requireBrackets = true)
        {
            if (expected is null || expected.Length == 0)
                return new(true);

            int required = expected.Count(arg => arg.Required);

            if (args.Length < required)
            {
                IEnumerable<string> args2 = expected.Select(arg => $"{(arg.Required ? "<" : "[")}{arg.ArgumentName}{(arg.Required ? ">" : "]")}");
                return new(false, string.Empty, ErrorGen.Get(130, action.Name, action is IAction ? "action" : "variable", required, string.Join(", ", args2)));
            }

            ArgumentProcessResult success = new(true);

            for (int i = 0; i < expected.Length; i++)
            {
                Argument expect = expected[i];
                string input = string.Empty;

                if (args.Length > i)
                    input = args[i];
                else
                    continue;

                ArgumentProcessResult res = ProcessIndividualParameter(expect, input, action, source, requireBrackets);
                if (!res.Success)
                    return res; // Throw issue to end-user

                success.NewParameters.AddRange(res.NewParameters);
            }

            // If the raw argument list is larger than the expected list, do not process any extra arguments
            // Edge-cases with long strings being the last parameter
            if (args.Length > expected.Length)
            {
                success.NewParameters.AddRange(args.Skip(expected.Length));
            }

            success.NewParameters.RemoveAll(o => o is string st && string.IsNullOrWhiteSpace(st));

            return success;
        }

        public static ArgumentProcessResult ProcessIndividualParameter(Argument expected, string input, IScriptComponent action, Script source, bool requireBrackets = true)
        {
            ArgumentProcessResult success = new(true);

            source.DebugLog($"Param {expected.ArgumentName} needs a {expected.Type.Name}");
            switch (expected.Type.Name)
            {
                // Number Types:
                case "Boolean":
                    success.NewParameters.Add(input.AsBool());
                    break;
                case "Int32": // int
                    if (!VariableSystem.TryParse(input, out int intRes, source, requireBrackets))
                        return new(false, expected.ArgumentName, ErrorGen.Get(134, input));

                    success.NewParameters.Add(intRes);
                    break;
                case "Int64": // long
                    if (!VariableSystem.TryParse(input, out long longRes, source, requireBrackets))
                        return new(false, expected.ArgumentName, ErrorGen.Get(134, input));

                    success.NewParameters.Add(longRes);
                    break;
                case "Single": // float
                    if (!VariableSystem.TryParse(input, out float floatRes, source, requireBrackets))
                        return new(false, expected.ArgumentName, ErrorGen.Get(137, input));

                    success.NewParameters.Add(floatRes);
                    break;
                case "Char":
                    if (!char.TryParse(input, out char charRes))
                        return new(false, expected.ArgumentName, ErrorGen.Get(146, input));

                    success.NewParameters.Add(charRes);
                    break;

                // Variable Interfaces
                case "IConditionVariable":
                    if (!VariableSystem.TryGetVariable(input, out IConditionVariable variable, out _, source, requireBrackets))
                        return new(false, expected.ArgumentName, ErrorGen.Get(132, input));

                    success.NewParameters.Add(variable);
                    break;
                case "IStringVariable":
                    if (!VariableSystem.TryGetVariable(input, out IConditionVariable variable2, out _, source, requireBrackets))
                        return new(false, expected.ArgumentName, ErrorGen.Get(132, input));
                    if (variable2 is not IStringVariable strVar)
                        return new(false, expected.ArgumentName, ErrorGen.Get(145, input));

                    success.NewParameters.Add(strVar);
                    break;
                case "IPlayerVariable":
                    if (!VariableSystem.TryGetVariable(input, out IConditionVariable variable3, out _, source, requireBrackets))
                        return new(false, expected.ArgumentName, ErrorGen.Get(132, input));
                    if (variable3 is not IPlayerVariable playerVar)
                        return new(false, expected.ArgumentName, ErrorGen.Get(133, input));

                    success.NewParameters.Add(playerVar);
                    break;

                // Array Types:
                case "Player[]":
                    if (!ScriptHelper.TryGetPlayers(input, null, out PlayerCollection players, source))
                        return new(false, expected.ArgumentName, players.Message);

                    success.NewParameters.Add(players);
                    break;
                case "Room[]":
                    if (!ScriptHelper.TryGetRooms(input, out Room[] rooms, source))
                        return new(false, expected.ArgumentName, MsgGen.Generate(MessageType.NoRoomsFound, action, expected.ArgumentName, input));

                    success.NewParameters.Add(rooms);
                    break;
                case "Door[]":
                    if (!ScriptHelper.TryGetDoors(input, out Door[] doors, source))
                        return new(false, expected.ArgumentName, ErrorGen.Get(142, input));

                    success.NewParameters.Add(doors);
                    break;
                case "Lift[]":
                    if (!ScriptHelper.TryGetLifts(input, out Lift[] lifts, source))
                        return new(false, expected.ArgumentName, ErrorGen.Get(143, input));

                    success.NewParameters.Add(lifts);
                    break;

                // Special
                case "RoleTypeIdOrTeam":
                    if (VariableSystem.TryParse(input, out RoleTypeId rtResult, source, requireBrackets))
                        success.NewParameters.Add(rtResult);
                    else if (VariableSystem.TryParse(input, out Team teamResult, source, requireBrackets))
                        success.NewParameters.Add(teamResult);
                    else
                        return new(false, expected.ArgumentName, ErrorGen.Get(147, input));

                    break;

                default:
                    // Handle all enum types
                    if (expected.Type.BaseType == typeof(Enum))
                    {
                        object res = VariableSystem.Parse(input, expected.Type, source);
                        if (res is null)
                        {
                            return new(false, expected.ArgumentName, ErrorGen.Get(144, input, expected.Type.Name));
                        }

                        success.NewParameters.Add(res);
                        break;
                    }

                    // Unsupported types: Add the string input
                    success.NewParameters.Add(input);
                    break;
            }

            return success;
        }
    }
}
