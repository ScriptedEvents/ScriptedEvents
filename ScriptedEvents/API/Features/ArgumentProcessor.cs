namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
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
        /// <param name="script">The script source.</param>
        /// <returns>The result of the process.</returns>
        public static ArgumentProcessResult Process(Argument[] expectedArguments, string[] args, IScriptComponent action, Script script)
        {
            void Log(string message)
            {
                if (!script.IsDebug) return;
                Logger.Debug($"[ArgumentProcessor] [Process] [{action.Name}] {message}", script);
            }

            if (expectedArguments.Length == 0)
            {
                Log("This action doesnt use arguments. Ending processing.");
                return new(true);
            }

            int requiredArguments = expectedArguments.Count(arg => arg.Required);
            if (args.Length < requiredArguments)
            {
                IEnumerable<string> args2 = expectedArguments.Select(arg => $"{(arg.Required ? "<" : "[")}{arg.ArgumentName}{(arg.Required ? ">" : "]")}");
                return new(false, true, string.Empty, ErrorGen.Get(ErrorCode.MissingArguments, action.Name, action is IAction ? "action" : "variable", requiredArguments, string.Join(", ", args2)));
            }

            ArgumentProcessResult success = new(true)
            {
                StrippedRawParameters = args.ToArray(),
            };

            for (int i = 0; i < expectedArguments.Length; i++)
            {
                // break when we run out of args
                if (args.Length <= i) break;

                Argument argument = expectedArguments[i];
                string input = args[i];

                ArgumentProcessResult res = ProcessIndividualParameter(argument, input, action, script);
                if (!res.Success) return res; // Throw issue to end-user

                success.NewParameters.AddRange(res.NewParameters);
            }

            success.NewParameters.AddRange(args.Skip(expectedArguments.Length).Select(arg =>
            {
                if (TryProcessSmartArgument(arg, action, script, out string saRes))
                {
                    return saRes;
                }
                else
                {
                    return Parser.ReplaceContaminatedValueSyntax(arg, script);
                }
            }).ToArray());

            Log($"Processed action parameters: '{string.Join(", ", success.NewParameters.Select(x => x.ToString()))}'");

            return success;
        }

        /// <summary>
        /// Tries to process the argument for a quick argument.
        /// </summary>
        /// <param name="input">The provided input.</param>
        /// <param name="action">The action or variable performing the process.</param>
        /// <param name="source">The script source.</param>
        /// <param name="result">The resulting string. Empty if method returns false.</param>
        /// <returns>The output of the process.</returns>
        public static bool TryProcessSmartArgument(string input, IScriptComponent action, Script source, out string result)
        {
            result = string.Empty;
            bool didSomething = false;

            if (action is not IAction actualAction)
            {
                return false;
            }

            // Regex pattern to match '#' followed by a digit
            Regex regex = new(@"#(\d)");
            result = input; // Start with input as the base result

            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                int index = match.Index;

                // Try to parse the number after '#'
                if (!int.TryParse(match.Groups[1].Value, out int lastNum) || lastNum < 1)
                {
                    continue;
                }

                Logger.Debug($"[SMART ARG PROC] Found '#' syntax with index '{lastNum}' at position {index}", source);

                string argument;
                try
                {
                    // Fetch smart argument based on index
                    var res = source.SmartArguments[actualAction][lastNum - 1]();
                    if (!res.Item1)
                    {
                        Logger.ScriptError($"[Smart argument] " + res.Item2, source, true);
                        continue;
                    }

                    argument = res.Item2;
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }

                Logger.Debug($"[SMART ARG PROC] Index '{lastNum}' is valid", source);

                // Replace the '#<number>' with the processed argument
                result = result.Replace(match.Value, argument);
                didSomething = true;

                Logger.Debug($"[SMART ARG PROC] Success! Smart arg used correctly. Result: {result}", source);
            }

            return didSomething;
        }

        /// <summary>
        /// Processes an individual argument.
        /// </summary>
        /// <param name="expected">The expected argument.</param>
        /// <param name="input">The provided input.</param>
        /// <param name="action">The action or variable performing the process.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The output of the process.</returns>
        public static ArgumentProcessResult ProcessIndividualParameter(Argument expected, string input, IScriptComponent action, Script source)
        {
            void Log(string message)
            {
                if (!source.IsDebug) return;
                Logger.Debug($"[ArgumentProcessor] [PIP] [{action.Name}] " + message, source);
            }

            ArgumentProcessResult success = new(true);

            Log($"Parameter '{expected.ArgumentName}' needs a '{expected.Type.Name}' type.");

            // Extra magic for options
            if (expected is OptionsArgument options)
            {
                if (options.Options.All(o => !string.Equals(o.Name, input, StringComparison.CurrentCultureIgnoreCase)) && options is not SuggestedOptionsArgument)
                {
                    return new(
                        false,
                        true,
                        expected.ArgumentName,
                        ErrorGen.Get(
                            ErrorCode.ParameterError_Option,
                            input,
                            expected.ArgumentName,
                            action.Name,
                            string.Join(
                                ", ",
                                options.Options.Select(
                                    x => x.Name))));
                }

                success.NewParameters.Add(input);
                Log($"[OPTION ARG] Parameter '{expected.ArgumentName}' now has a processed value '{success.NewParameters.Last()}' and raw value '{input}'");
                return success;
            }

            // smart action arguments
            if (TryProcessSmartArgument(input, action, source, out var saResult))
            {
                input = saResult;
            }

            switch (expected.Type.Name)
            {
                // Number Types:
                case "Boolean":
                    if (!input.IsBool(out var result, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidBoolean, input));

                    success.NewParameters.Add(result);
                    break;
                case "Int32": // int
                    if (!Parser.Cast<int>(int.TryParse, input, source, out var intRes))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidInteger, input));

                    success.NewParameters.Add(intRes);
                    break;
                case "Int64": // long
                    if (!Parser.Cast<long>(long.TryParse, input, source, out var longRes))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidInteger, input));

                    success.NewParameters.Add(longRes);
                    break;
                case "Single": // float
                    if (!Parser.Cast<float>(float.TryParse, input, source, out var floatRes))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidNumber, input));

                    success.NewParameters.Add(floatRes);
                    break;

                case "UInt16": // ushort
                    if (!Parser.Cast<ushort>(ushort.TryParse, input, source, out var ushortRes))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidNumber, input));

                    success.NewParameters.Add(ushortRes);
                    break;
                case "Char":
                    if (!Parser.Cast<char>(char.TryParse, input, source, out var charRes))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidCharacter, input));

                    success.NewParameters.Add(charRes);
                    break;

                case "IVariable":
                    if (!VariableSystem.TryGetVariable<IVariable>(input, source, out var someVar, false)
                        || someVar is null)
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidVariable, input));

                    success.NewParameters.Add(someVar);
                    break;
                case "ILiteralVariable":
                    if (!VariableSystem.TryGetVariable<ILiteralVariable>(input, source, out var strVar, false)
                        || strVar is null)
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidStringVariable, input));

                    success.NewParameters.Add(strVar);
                    break;
                case "IPlayerVariable":
                    if (!VariableSystem.TryGetVariable<IPlayerVariable>(input, source, out var plrVar, false)
                        || plrVar is null)
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidPlayerVariable, input));

                    success.NewParameters.Add(plrVar);
                    break;

                // Array Types:
                case "Room[]":
                    if (!Parser.TryGetRooms(input, out var rooms, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.ParameterError_Rooms, input, expected.ArgumentName));

                    success.NewParameters.Add(rooms);
                    break;
                case "Door[]":
                    if (!Parser.TryGetDoors(input, out var doors, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidDoor, input));

                    success.NewParameters.Add(doors);
                    break;
                case "Lift[]":
                    if (!Parser.TryGetLifts(input, out var lifts, source))
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidLift, input));

                    success.NewParameters.Add(lifts);
                    break;

                // Special
                case "PlayerCollection":
                    if (!Parser.TryGetPlayers(input, null, out var players, source))
                        return new(false, true, expected.ArgumentName, players.Message);

                    success.NewParameters.Add(players);
                    break;

                case "Player":
                    if (!Parser.TryGetPlayers(input, null, out var players1, source))
                        return new(false, true, expected.ArgumentName, players1.Message);

                    switch (players1.Length)
                    {
                        case 0:
                            return new(false, true, expected.ArgumentName, $"One player is required, but value '{input}' holds no players.");
                        case > 1:
                            return new(false, true, expected.ArgumentName, $"One player is required, but value '{input}' holds more than one player ({players1.Length} players).");
                    }

                    success.NewParameters.Add(players1.FirstOrDefault());
                    break;

                case "RoleTypeIdOrTeam":
                    if (Parser.TryParseEnum(input, out RoleTypeId rtResult, source))
                        success.NewParameters.Add(rtResult);
                    else if (Parser.TryParseEnum(input, out Team teamResult, source))
                        success.NewParameters.Add(teamResult);
                    else
                        return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidRoleTypeOrTeam, input));

                    break;

                default:
                    // Handle all enum types
                    if (expected.Type.BaseType == typeof(Enum))
                    {
                        object res = Parser.ParseEnum(input, expected.Type, source);
                        if (res is null)
                            return new(false, true, expected.ArgumentName, ErrorGen.Get(ErrorCode.InvalidEnumGeneric, input, expected.Type.Name));

                        success.NewParameters.Add(res);
                        break;
                    }

                    success.NewParameters.Add(Parser.ReplaceContaminatedValueSyntax(input, source));
                    break;
            }

            Log($"Param '{expected.ArgumentName}' processed! STD value: '{success.NewParameters.Last()}' RAW value: '{input}'");

            return success;
        }

        private static ArgumentProcessResult HandleFORDecorator(string[] inArgs, Script script, out string[] args)
        {
            void Log(string message)
            {
                if (!script.IsDebug) return;
                Logger.Debug("[ArgumentProcessor] [$FOR] " + message, script);
            }

            args = inArgs;
            int loopSyntaxIndex = inArgs.IndexOf("$FOR");

            if (loopSyntaxIndex == -1)
            {
                return new(true);
            }

            Log("Syntax found, continuing.");

            string[] loopArgs = inArgs.Skip(loopSyntaxIndex + 1).ToArray();
            args = inArgs.Take(loopSyntaxIndex).ToArray();

            string newPlayerVarName = loopArgs[0];
            string inKeyword = loopArgs[1];
            string playerVarNameLoopingThrough = loopArgs[2];

            if (inKeyword != "IN")
                Logger.Warn($"[$FOR DECORATOR] $FOR statement requires the 'IN' keyword (e.g. $FOR {{PLR}} IN {{PLAYERS}}). Instead of 'IN', '{inKeyword}' was provided.", script);

            List<Player> playersToLoop;

            if (script.PlayerLoopInfo is not null && script.PlayerLoopInfo.Line == script.CurrentLine)
            {
                playersToLoop = script.PlayerLoopInfo.PlayersToLoopThrough;
                Log("A loop for this action has already been initialized.");
            }
            else
            {
                if (!Parser.TryGetPlayers(playerVarNameLoopingThrough, null, out PlayerCollection outPlayers, script))
                {
                    return new(false, true, ErrorGenV2.InvalidPlayerVariable(playerVarNameLoopingThrough));
                }

                playersToLoop = outPlayers.GetInnerList();
                Log($"A loop for this action has not yet been initalized. Saved '{playerVarNameLoopingThrough}' as the players to loop through.");
            }

            if (playersToLoop.Count == 0)
            {
                Log("No more players to loop through. Action will be skipped.");
                script.PlayerLoopInfo = null;
                return new(false);
            }

            Player player = playersToLoop.First();
            playersToLoop.Remove(player);

            script.AddPlayerVariable(newPlayerVarName, new[] { player }, false);

            script.PlayerLoopInfo = new(script.CurrentLine, playersToLoop);

            script.Jump(script.CurrentLine);

            return new(true);
        }

        private static ArgumentProcessResult HandleIFDecorator(string[] inArgs, Script script, out string[] outArgs)
        {
            void Log(string message)
            {
                if (!script.IsDebug) return;
                Logger.Debug("[ArgumentProcessor] [$IF] " + message, script);
            }

            outArgs = inArgs;

            int conditionSectionKeyword = inArgs.IndexOf("$IF");
            if (conditionSectionKeyword == -1)
            {
                return new(true);
            }

            Log($"Decorator was detected, continuing...");

            string[] conditionArgs = inArgs.Skip(conditionSectionKeyword + 1).ToArray();
            outArgs = outArgs.Take(conditionSectionKeyword).ToArray();

            ConditionResponse resp = ConditionHelper.Evaluate(string.Join(" ", conditionArgs), script);

            if (!resp.Success)
            {
                Log("Evaluation resulted in an error.");
                return new(false, true, string.Empty, resp.Message);
            }

            if (!resp.Passed)
            {
                Log("Evaluation resulted in FALSE. Action will be skipped.");
                return new(false, false);
            }

            Log($"Evaluation resulted in TRUE. Continuing...");
            return new(true);
        }
    }
}
