namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Items;
    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// Contains methods to process action or variable arguments.
    /// </summary>
    public static class ArgumentProcessor
    {
        /// <summary>
        /// Processes arguments.
        /// </summary>
        /// <param name="expectedArguments">The expected arguments.</param>
        /// <param name="args">The provided arguments.</param>
        /// <param name="action">The action or variable performing the process.</param>
        /// <param name="source">The script source.</param>
        /// <param name="requireBrackets">If brackets are required to convert variables.</param>
        /// <returns>The result of the process.</returns>
        public static ArgumentProcessResult Process(Argument[] expectedArguments, string[] args, IScriptComponent action, Script source, bool requireBrackets = true)
        {
            Log.Info("called");
            if (expectedArguments is null || expectedArguments.Length == 0)
                return new(true);

            int required = expectedArguments.Count(arg => arg.Required);

            ArgumentProcessResult processedForLoop = HandlePlayerListComprehension(args, source, out string[] strippedArgs);
            Log.Info($"success: {processedForLoop.Success} | errored: {processedForLoop.Errored} | message: {processedForLoop.Message}");
            if (!processedForLoop.Success)
            {
                Log.Warn("skipped by for loop");
                return processedForLoop;
            }

            args = strippedArgs;

            int conditionSectionKeyword = args.IndexOf("$IF");
            if (conditionSectionKeyword != -1)
            {
                string[] conditionArgs = args.Skip(conditionSectionKeyword + 1).ToArray();
                args = args.Take(conditionSectionKeyword).ToArray();
                source.DebugLog($"args = {string.Join(",", args)} | conditionArgs = {string.Join(",", conditionArgs)}");
                if (!ConditionHelperV2.Evaluate(string.Join(" ", conditionArgs), source).Passed)
                {
                    return new(false);
                }
            }

            if (args.Length < required)
            {
                IEnumerable<string> args2 = expectedArguments.Select(arg => $"{(arg.Required ? "<" : "[")}{arg.ArgumentName}{(arg.Required ? ">" : "]")}");
                return new(false, true, string.Empty, ErrorGen.Get(ErrorCode.MissingArguments, action.Name, action is IAction ? "action" : "variable", required, string.Join(", ", args2)));
            }

            ArgumentProcessResult success = new(true);

            for (int i = 0; i < expectedArguments.Length; i++)
            {
                Argument expect = expectedArguments[i];
                string input = string.Empty;

                if (args.Length > i)
                    input = args[i];
                else
                    continue;

                ArgumentProcessResult res = ProcessIndividualParameter(expect, input, action, source, requireBrackets);
                if (!res.Success) return res; // Throw issue to end-user

                success.NewParameters.AddRange(res.NewParameters);
            }

            // If the raw argument list is larger than the expected list, do not process any extra arguments
            // Edge-cases with long strings being the last parameter
            if (args.Length > expectedArguments.Length)
            {
                success.NewParameters.AddRange(args.Skip(expectedArguments.Length));
            }

            success.NewParameters.RemoveAll(o => o is string st && string.IsNullOrWhiteSpace(st));

            return success;
        }

        /// <summary>
        /// Processes an individual argument.
        /// </summary>
        /// <param name="expected">The expected argument.</param>
        /// <param name="input">The provided input.</param>
        /// <param name="action">The action or variable performing the process.</param>
        /// <param name="source">The script source.</param>
        /// <param name="requireBrackets">If brackets are required to convert variables.</param>
        /// <returns>The output of the process.</returns>
        public static ArgumentProcessResult ProcessIndividualParameter(Argument expected, string input, IScriptComponent action, Script source, bool requireBrackets = true)
        {
            ArgumentProcessResult success = new(true);

            source.DebugLog($"[C: {action.Name}] Param {expected.ArgumentName} needs a {expected.Type.Name}");

            // Extra magic for options
            if (expected is OptionsArgument options)
            {
                if (!options.Options.Any(o => o.Name.ToUpper() == input.ToUpper()))
                    return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.ParameterError_Option, input, expected.ArgumentName, action.Name, string.Join(", ", options.Options.Select(x => x.Name))));

                success.NewParameters.Add(input);
                source?.DebugLog($"[OPTION ARG] [C: {action.Name}] Param {expected.ArgumentName} has a processed value '{success.NewParameters.Last()}' and raw value '{input}'");
                return success;
            }

            switch (expected.Type.Name)
            {
                // Number Types:
                case "Boolean":
                    if (!input.IsBool(out bool result, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidBoolean, input));

                    success.NewParameters.Add(result);
                    break;
                case "Int32": // int
                    if (!SEParser.TryParse(input, out int intRes, source, requireBrackets))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidInteger, input));

                    success.NewParameters.Add(intRes);
                    break;
                case "Int64": // long
                    if (!SEParser.TryParse(input, out long longRes, source, requireBrackets))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidInteger, input));

                    success.NewParameters.Add(longRes);
                    break;
                case "Single": // float
                    if (!SEParser.TryParse(input, out float floatRes, source, requireBrackets))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidNumber, input));

                    success.NewParameters.Add(floatRes);
                    break;
                case "Char":
                    if (!char.TryParse(input, out char charRes))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidCharacter, input));

                    success.NewParameters.Add(charRes);
                    break;

                // Variable Interfaces
                case "IConditionVariable":
                    if (!VariableSystemV2.TryGetVariable(input, source, out VariableResult variable, requireBrackets))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidVariable, input));

                    success.NewParameters.Add(variable.Variable);
                    break;
                case "IStringVariable":
                    if (!VariableSystemV2.TryGetVariable(input, source, out VariableResult variable2, requireBrackets))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidVariable, input));
                    if (variable2.Variable is not IStringVariable strVar)
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidStringVariable, input));

                    success.NewParameters.Add(strVar);
                    break;
                case "IPlayerVariable":
                    if (!VariableSystemV2.TryGetVariable(input, source, out VariableResult variable3, requireBrackets))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidVariable, input));
                    if (variable3.Variable is not IPlayerVariable playerVar)
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidPlayerVariable, input));

                    success.NewParameters.Add(playerVar);
                    break;

                case "IItemVariable":
                    if (!VariableSystemV2.TryGetVariable(input, source, out VariableResult variable4, requireBrackets))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidVariable, input));
                    if (variable4.Variable is not IItemVariable itemVar)

                        // TODO: ???
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidVariable, input));
                    if (Item.Get(itemVar.Value) is null)
                        return new(false, true, expected.ArgumentName, "The provided item variable is not valid.");

                    success.NewParameters.Add(itemVar);
                    break;

                // Array Types:
                case "Room[]":
                    if (!ScriptModule.TryGetRooms(input, out Room[] rooms, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.ParameterError_Rooms, input, expected.ArgumentName));

                    success.NewParameters.Add(rooms);
                    break;
                case "Door[]":
                    if (!ScriptModule.TryGetDoors(input, out Door[] doors, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidDoor, input));

                    success.NewParameters.Add(doors);
                    break;
                case "Lift[]":
                    if (!ScriptModule.TryGetLifts(input, out Lift[] lifts, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidLift, input));

                    success.NewParameters.Add(lifts);
                    break;

                // Special
                case "PlayerCollection":
                    if (!ScriptModule.TryGetPlayers(input, null, out PlayerCollection players, source))
                        return new(false, true, expected.ArgumentName, players.Message);

                    success.NewParameters.Add(players);
                    break;
                case "RoleTypeIdOrTeam":
                    if (SEParser.TryParse(input, out RoleTypeId rtResult, source, requireBrackets))
                        success.NewParameters.Add(rtResult);
                    else if (SEParser.TryParse(input, out Team teamResult, source, requireBrackets))
                        success.NewParameters.Add(teamResult);
                    else
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidRoleTypeOrTeam, input));

                    break;

                default:
                    // Handle all enum types
                    if (expected.Type.BaseType == typeof(Enum))
                    {
                        object res = SEParser.Parse(input, expected.Type, source);
                        if (res is null)
                            return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidEnumGeneric, input, expected.Type.Name));

                        success.NewParameters.Add(res);
                        break;
                    }

                    // Unsupported types: Add the string input
                    success.NewParameters.Add(input);
                    break;
            }

            source?.DebugLog($"[C: {action.Name}] Param {expected.ArgumentName} has a processed value '{success.NewParameters.Last()}' and raw value '{input}'");

            return success;
        }

        private static ArgumentProcessResult HandlePlayerListComprehension(string[] inArgs, Script source, out string[] args)
        {
            args = inArgs;
            int loopSyntaxIndex = inArgs.IndexOf("$FOR");

            if (loopSyntaxIndex == -1)
                return new(true);

            string[] loopArgs = inArgs.Skip(loopSyntaxIndex + 1).ToArray();
            args = inArgs.Take(loopSyntaxIndex).ToArray();

            string newPlayerVarName = loopArgs[0];
            string inKeyword = loopArgs[1];
            string playerVarNameLoopingThrough = loopArgs[2];

            if (inKeyword != "IN")
                Log.Warn($"[LINE {source.CurrentLine + 1}] $FOR statement requires 'IN' keyword, provided '{inKeyword}'.");

            List<Player> playersToLoop;

            Log.Info($"curent line is {source.CurrentLine}, for loop is at {source.CurrentLine}");

            if (source.PlayerLoopInfo is not null && source.PlayerLoopInfo.Line == source.CurrentLine)
            {
                Log.Info("this is not the first time");
                playersToLoop = source.PlayerLoopInfo.PlayersToLoopThrough;
            }
            else
            {
                Log.Info("this is the first time");
                if (!VariableSystemV2.TryGetPlayers(playerVarNameLoopingThrough, source, out PlayerCollection outPlayers))
                    return new(false, true, playerVarNameLoopingThrough, ErrorGen.Get(ErrorCode.InvalidPlayerVariable, playerVarNameLoopingThrough));
                playersToLoop = outPlayers.GetInnerList();
            }

            Log.Info($"there are {playersToLoop.Count} players in {playerVarNameLoopingThrough}");

            if (playersToLoop.Count == 0)
            {
                Log.Info($"there are 0 players, returning");
                return new(false);
            }

            Player player = playersToLoop.FirstOrDefault();
            playersToLoop.Remove(player);

            Log.Info($"adding variavle {newPlayerVarName}");

            source.AddPlayerVariable(newPlayerVarName, string.Empty, new[] { player });

            source.PlayerLoopInfo = new(source.CurrentLine, playersToLoop);

            Log.Info($"now there are {source.PlayerLoopInfo.PlayersToLoopThrough.Count} left to loop");
            Log.Info($"jumping to line {source.CurrentLine}");
            source.Jump(source.CurrentLine);

            return new(true);
        }
    }
}
