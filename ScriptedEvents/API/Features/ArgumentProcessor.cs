namespace ScriptedEvents.API.Features
{
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using InventorySystem.Items.Usables.Scp330;
    using PlayerRoles;
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
                case "ItemType":
                    if (!VariableSystem.TryParse(input, out ItemType itemTypeRes, source))
                        return new(false, expected.ArgumentName, "Invalid ItemType or Custom Item name provided.");

                    success.NewParameters.Add(itemTypeRes);
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
                default:
                    success.NewParameters.Add(input);
                    break;
            }

            return success;
        }
    }
}
