namespace ScriptedEvents.API.Features
{
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

                // Enumerations:
                case "CandyKindID":
                    if (!VariableSystem.TryParse(input, out CandyKindID candyRes, source))
                        return new(false, expected.ArgumentName, "Invalid candy type provided.");

                    success.NewParameters.Add(candyRes);
                    break;
                case "RoleTypeId":
                    // Todo: Better error message here
                    if (!VariableSystem.TryParse(input, out RoleTypeId roleTypeRes, source))
                        return new(false, expected.ArgumentName, MsgGen.Generate(MessageType.InvalidRole, action, expected.ArgumentName, input));

                    success.NewParameters.Add(roleTypeRes);
                    break;
                case "Team":
                    if (!VariableSystem.TryParse(input, out Team teamRes, source))
                        return new(false, expected.ArgumentName, "Invalid team type provided.");

                    success.NewParameters.Add(teamRes);
                    break;
                case "ItemType":
                    if (!VariableSystem.TryParse(input, out ItemType itemTypeRes, source))
                        return new(false, expected.ArgumentName, "Invalid ItemType or Custom Item name provided.");

                    success.NewParameters.Add(itemTypeRes);
                    break;
                case "EffectType":
                    if (!VariableSystem.TryParse(input, out EffectType effectRes, source))
                        return new(false, expected.ArgumentName, "Invalid effect type provided.");

                    success.NewParameters.Add(effectRes);
                    break;
                case "SpawnableTeamType":
                    if (!VariableSystem.TryParse(input, out SpawnableTeamType sttRes, source))
                        return new(false, expected.ArgumentName, "Invalid spawnable role provided. Must be ChaosInsurgency or NineTailedFox.");

                    success.NewParameters.Add(sttRes);
                    break;
                case "RadioRange":
                    if (!VariableSystem.TryParse(input, out RadioRange radioRangeRes, source))
                        return new(false, expected.ArgumentName, "Invalid radio range provided. Must be: Short, Medium, Long, Ultra.");

                    success.NewParameters.Add(radioRangeRes);
                    break;
                case "SpawnLocationType":
                    if (!VariableSystem.TryParse(input, out SpawnLocationType spltres, source))
                        return new(false, expected.ArgumentName, "Invalid SpawnLocation type provided. View all valid spawns at: https://exiled-team.github.io/EXILED/api/Exiled.API.Enums.SpawnLocationType.html");

                    success.NewParameters.Add(spltres);
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
                    success.NewParameters.Add(input);
                    break;
            }

            return success;
        }
    }
}
