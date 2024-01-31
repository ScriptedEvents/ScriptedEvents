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

    public static class ArgumentProcessor
    {
        public static ArgumentProcessResult Process(Argument[] expected, string[] args, IAction action, Script source)
        {
            if (expected is null || expected.Length == 0)
                return new(true);

            int required = expected.Count(arg => arg.Required);

            // Todo: Better error message here
            if (args.Length < required)
                return new(false, string.Empty, MsgGen.Generate(MessageType.InvalidUsage, action, string.Empty, (object)expected));

            ArgumentProcessResult success = new(true);

            for (int i = 0; i < expected.Length; i++)
            {
                Argument expect = expected[i];
                string input = string.Empty;

                if (args.Length > i)
                    input = args[i];
                else
                    continue;

                ArgumentProcessResult res = ProcessIndividualParameter(expect, input, action, source);
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

        public static ArgumentProcessResult ProcessIndividualParameter(Argument expected, string input, IAction action, Script source)
        {
            ArgumentProcessResult success = new(true);

            source.DebugLog($"Param {expected.ArgumentName} needs a {expected.Type.Name}");
            switch (expected.Type.Name)
            {
                // Todo: Add enumerations (RoomType, ZoneType, DamageType)
                // Number Types:
                case "Boolean":
                    success.NewParameters.Add(input.AsBool());
                    break;
                case "Int32": // int
                    if (!VariableSystem.TryParse(input, out int intRes, source))
                        return new(false, expected.ArgumentName, ErrorGen.Get(134, input));

                    success.NewParameters.Add(intRes);
                    break;
                case "Int64": // long
                    if (!VariableSystem.TryParse(input, out long longRes, source))
                        return new(false, expected.ArgumentName, ErrorGen.Get(134, input));

                    success.NewParameters.Add(longRes);
                    break;
                case "Single": // float
                    if (!VariableSystem.TryParse(input, out float floatRes, source))
                        return new(false, expected.ArgumentName, ErrorGen.Get(137, input));

                    success.NewParameters.Add(floatRes);
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
                        return new(false, expected.ArgumentName, "Invalid door(s) provided!");

                    success.NewParameters.Add(doors);
                    break;
                case "Lift[]":
                    if (!ScriptHelper.TryGetLifts(input, out Lift[] lifts, source))
                        return new(false, expected.ArgumentName, "Invalid lift(s) provided!");

                    success.NewParameters.Add(lifts);
                    break;
                default:
                    // Handle all enum types
                    if (expected.Type.BaseType == typeof(Enum))
                    {
                        object res = VariableSystem.Parse(input, expected.Type, source);
                        if (res is null)
                        {
                            return new(false, expected.ArgumentName, $"Invalid {expected.Type.Name} provided. See all options by running 'shelp {expected.Type.Name}' in the server console.");
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
