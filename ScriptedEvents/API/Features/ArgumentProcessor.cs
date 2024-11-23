using ScriptedEvents.Interfaces;

namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using PlayerRoles;
    using ScriptedEvents.API.Extensions;
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
        public static ArgumentProcessResult ProcessActionArguments(Argument[] expectedArguments, string[] args, IAction action, Script script)
        {
            void Log(string message)
            {
                if (!script.IsDebug) return;
                Logger.Debug($"[ArgumentProcessor] [ProcessActionArguments] [{action.Name}] {message}", script);
            }

            if (expectedArguments.Length == 0)
            {
                Log("This action doesnt use arguments. Ending processing.");
                return new(true);
            }

            int requiredArguments = expectedArguments.Count(arg => arg.Required);
            if (args.Length < requiredArguments)
            {
                IEnumerable<string> labeledArgs = expectedArguments.Select(arg => $"[{(arg.Required ? "Required" : "Optional")} argument '{arg.ArgumentName}']");
                return new(
                    false,
                    true,
                    Error(
                        $"Action '{action.Name}' is missing {requiredArguments - args.Length} arguments.",
                        $"Action defines these arguments: {string.Join(", ", labeledArgs)}, of which {requiredArguments - args.Length} required arguments were not provided.")
                        .ToTrace());
            }

            ArgumentProcessResult success = new(true)
            {
                StrippedRawParameters = args.ToArray(),
            };

            for (int i = 0; i < expectedArguments.Length; i++)
            {
                // assign null to the expected argument if there are no more raw arguments
                if (args.Length <= i)
                {
                    success.NewParameters.Add(null);
                    continue;
                }

                Argument argument = expectedArguments[i];
                string input = args[i];

                ArgumentProcessResult res = ProcessIndividualParameter(argument, input, action, script);
                if (!res.ShouldExecute)
                {
                    return res; // Throw issue to end-user
                }

                success.NewParameters.Add(res.NewParameters.First());
            }

            success.NewParameters.AddRange(args.Skip(expectedArguments.Length).Select(arg =>
            {
                if (TryProcessSmartArgumentsInContaminatedString(arg, action, script, out string saRes))
                {
                    return saRes;
                }

                return Parser.ReplaceContaminatedValueSyntax(arg, script);
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
        public static bool TryProcessSmartArgumentsInContaminatedString(string input, IAction action, Script source, out string result)
        {
            bool didSomething = false;

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
                    var res = source.SmartArguments[action][lastNum - 1]();
                    if (res.Item1 is not null)
                    {
                        Logger.ScriptError(res.Item1!, source);
                        continue;
                    }

                    if (res.Item3 != typeof(string))
                    {
                        var trace = Error(
                            "Invalid type returned from a smart argument",
                            $"The value under the smart argument '{match.Value}' is not a literal value, but value of type '{res.Item3!.Name}'.")
                            .ToTrace();
                        Logger.ScriptError(trace, source);
                    }

                    argument = (string)res.Item2!;
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
        /// Tries to process the argument for a quick argument.
        /// </summary>
        /// <param name="input">The provided input.</param>
        /// <param name="action">The action or variable performing the process.</param>
        /// <param name="source">The script source.</param>
        /// <param name="result">The resulting string. Empty if method returns false.</param>
        /// <returns>The output of the process.</returns>
        public static bool TryProcessSmartArgument(string input, IAction action, Script source, out object? result, out Type? type)
        {
            result = null;
            type = null;

            // Regex pattern to match '#' followed by a digit
            Regex regex = new(@"#(\d)");
            result = input; // Start with input as the base result

            var matches = regex.Matches(input);

            if (matches.Count != 1)
            {
                return false;
            }

            var match = matches[0];

            if (match.Length != input.Length)
            {
                return false;
            }

            // Try to parse the number after '#'
            if (!int.TryParse(match.Groups[1].Value, out int lastNum) || lastNum < 1)
            {
                return false;
            }

            try
            {
                // Fetch smart argument based on index
                var res = source.SmartArguments[action][lastNum - 1]();
                if (res.Item1 is not null)
                {
                    Logger.ScriptError(res.Item1, source);
                    return false;
                }

                result = res.Item2!;
                type = res.Item3!;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Processes an individual argument.
        /// </summary>
        /// <param name="expected">The expected argument.</param>
        /// <param name="input">The provided input.</param>
        /// <param name="action">The action or variable performing the process.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The output of the process.</returns>
        public static ArgumentProcessResult ProcessIndividualParameter(Argument expected, string input, IAction action, Script source)
        {
            ArgumentProcessResult success = new(true);

            Log($"Parameter '{expected.ArgumentName}' needs a '{expected.Type}' type.");

            // Extra magic for options
            if (expected is OptionsArgument options)
            {
                if (options.Options.All(o => !string.Equals(o.Name, input, StringComparison.CurrentCultureIgnoreCase))
                    && options is not SuggestedOptionsArgument)
                {
                    return new(
                        false,
                        true,
                        Error(
                            $"Input '{input}' is not recongnized by option argument '{options.ArgumentName}' of action '{action.Name}'",
                            $"This argument only supports one of the following: '{string.Join("', '", options.Options.Select(o => o.Name))}'.")
                            .ToTrace());
                }

                success.NewParameters.Add(input);
                Log($"[OPTION ARG] Parameter '{expected.ArgumentName}' now has a value '{input}'");
                return success;
            }

            if (TryProcessSmartArgument(input, action, source, out var smartArgRes, out var type))
            {
                if (expected.Type == type)
                {
                    success.NewParameters.Add(smartArgRes!);
                }
            }

            // smart action arguments
            if (TryProcessSmartArgumentsInContaminatedString(input, action, source, out var saResult))
            {
                input = saResult;
            }

            switch (expected.Type.Name)
            {
                case "Boolean":
                    if (!input.IsBool(out var result, out var boolErr, source))
                    {
                        return ErrorByInfo(boolErr!);
                    }

                    success.NewParameters.Add(result);
                    break;

                case "Int32": // int
                    if (!Parser.TryCast<int>(int.TryParse, input, source, out var intRes, out var intErr))
                    {
                        ErrorByInfo(intErr!);
                    }

                    success.NewParameters.Add(intRes);
                    break;

                case "Int64": // long
                    if (!Parser.TryCast<long>(long.TryParse, input, source, out var longRes, out var longErr))
                    {
                        ErrorByInfo(longErr!);
                    }

                    success.NewParameters.Add(longRes);
                    break;

                case "Single": // float
                    if (!Parser.TryCast<float>(float.TryParse, input, source, out var floatRes, out var floatErr))
                    {
                        ErrorByInfo(floatErr!);
                    }

                    success.NewParameters.Add(floatRes);
                    break;

                case "UInt16": // ushort
                    if (!Parser.TryCast<ushort>(ushort.TryParse, input, source, out var ushortRes, out var ushortErr))
                    {
                        ErrorByInfo(ushortErr!);
                    }

                    success.NewParameters.Add(ushortRes);
                    break;

                case "Char":
                    if (!Parser.TryCast<char>(char.TryParse, input, source, out var charRes, out var charErr))
                    {
                        ErrorByInfo(charErr!);
                    }

                    success.NewParameters.Add(charRes);
                    break;

                case "Item": throw new NotImplementedException();

                case "TimeSpan":
                    if (!Parser.TryGetTimeSpan(input, out TimeSpan timeSpan, out ErrorInfo? timeSpanErr))
                    {
                        return ErrorByInfo(timeSpanErr!);
                    }
                    
                    success.NewParameters.Add(timeSpan);
                    break;
                
                case "Script": throw new NotImplementedException();
                
                case "ItemType[]": throw new NotImplementedException();

                case "IVariable":
                    if (!VariableSystem.TryGetVariable<IVariable>(input, source, out var someVar, false, out var someVarTrace))
                    {
                        return ErrorByTrace(someVarTrace!);
                    }

                    success.NewParameters.Add(someVar!);
                    break;

                case "ILiteralVariable":
                    if (!VariableSystem.TryGetVariable<ILiteralVariable>(input, source, out var strVar, false,
                            out var strVarTrace))
                    {
                        return ErrorByTrace(strVarTrace!);
                    }

                    success.NewParameters.Add(strVar!);
                    break;

                case "IPlayerVariable":
                    if (!VariableSystem.TryGetVariable<IPlayerVariable>(input, source, out var plrVar, false,
                            out var plrVarTrace))
                    {
                        return ErrorByTrace(plrVarTrace!);
                    }

                    success.NewParameters.Add(plrVar!);
                    break;

                case "Room[]":
                    if (!Parser.TryGetRooms(input, out var rooms, source, out var roomError))
                    {
                        return ErrorByInfo(roomError!);
                    }

                    success.NewParameters.Add(rooms);
                    break;

                case "Door[]":
                    if (!Parser.TryGetDoors(input, out var doors, source, out var doorError))
                    {
                        return ErrorByInfo(doorError!);
                    }

                    success.NewParameters.Add(doors);
                    break;

                case "Lift[]":
                    if (!Parser.TryGetLifts(input, out var lifts, source, out var liftError))
                    {
                        return ErrorByInfo(liftError!);
                    }

                    success.NewParameters.Add(lifts);
                    break;
                
                case "Player[]":
                    if (!Parser.TryGetPlayers(input, null, out var players, source, out var collectionError))
                    {
                        return ErrorByTrace(collectionError!);
                    }

                    success.NewParameters.Add(players.ToArray());
                    break;

                case "Player":
                    if (!Parser.TryGetPlayers(input, null, out var players1, source, out var playerError))
                    {
                        return ErrorByTrace(playerError!);
                    }

                    var enumerable = players1 as Player[] ?? players1.ToArray();
                    switch (enumerable.Length)
                    {
                        case 0:
                            return ErrorByInfo(Error(
                                $"Provided variable '{input}' has no players.",
                                $"There was one player expected, but no player is present."));
                        case > 1:
                            return ErrorByInfo(Error(
                                $"Provided variable '{input}' has too many players.",
                                $"There was one player expected, but {enumerable.Length} players are present."));
                    }

                    success.NewParameters.Add(enumerable.First());
                    break;

                default:
                    // Handle all enum types
                    if (expected.Type.BaseType == typeof(Enum))
                    {
                        var genericMethod = typeof(ArgumentProcessor)
                            .GetMethod("TryGetEnum")!
                            .MakeGenericMethod(expected.Type);

                        object?[] arguments = { input, null, source, null };

                        genericMethod.Invoke(null, arguments);

                        if (arguments[3] is ErrorInfo errorInfo)
                        {
                            return ErrorByInfo(errorInfo);
                        }

                        success.NewParameters.Add((arguments[1] as Enum)!);
                    }

                    success.NewParameters.Add(Parser.ReplaceContaminatedValueSyntax(input, source));
                    break;
            }

            Log($"Param '{expected.ArgumentName}' processed! STD value: '{success.NewParameters.Last()}' RAW value: '{input}'");

            return success;

            ArgumentProcessResult ErrorByTrace(ErrorTrace trace)
            {
                trace.Append(Error(
                    $"Failed to process argument '{expected.ArgumentName}' for action '{action.Name}'",
                    $"Provided input '{input}' is not possible to be interpreted as value of type '{expected.Type.MemberType}'."));
                return new(false, true, trace);
            }

            ArgumentProcessResult ErrorByInfo(ErrorInfo error)
            {
                var trace = error.ToTrace();
                trace.Append(Error(
                    $"Failed to process argument '{expected.ArgumentName}' for action '{action.Name}'",
                    $"Provided input '{input}' is not possible to be interpreted as value of type '{expected.Type.Name}'."));
                return new(false, true, trace);
            }

            void Log(string message)
            {
                if (!source.IsDebug) return;
                Logger.Debug($"[ArgumentProcessor] [PIP] [{action.Name}] " + message, source);
            }
        }

        private static ErrorInfo Error(string name, string desc)
        {
            return new ErrorInfo(name, desc, "ArgumentProcessor");
        }
    }
}
