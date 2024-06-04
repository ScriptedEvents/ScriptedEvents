namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class VariableSystemV2 : SEModule
    {
        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="IVariableGroup"/> representing all the valid condition variables.
        /// </summary>
        public static List<IVariableGroup> Groups { get; } = new();

        /// <inheritdoc/>
        public override string Name => "VariableSystemV2";

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, CustomVariable> DefinedVariables { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of player variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, CustomPlayerVariable> DefinedPlayerVariables { get; } = new();

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="desc">A description of the variable.</param>
        /// <param name="input">The value of the variable.</param>
        /// <param name="source">The script source.</param>
        /// <param name="local">If should be this script exclusive.</param>
        /// <remarks>Curly braces will be added automatically if they are not present already.</remarks>
        public static void DefineVariable(string name, string desc, string input, Script source, bool local = false)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name += "}";

            if (!local)
            {
                DefinedVariables[name] = new(name, desc, input);
            }
            else if (source is not null && local)
            {
                source.AddVariable(name, desc, input);
            }
            else
            {
                throw new Exception($"Cannot save the {name} variable.");
            }

            source?.DebugLog($"Defined variable {name} with value {input}");
        }

        /// <summary>
        /// Defines a player variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="desc">A description of the variable.</param>
        /// <param name="players">The players.</param>
        /// <param name="source">The script source.</param>
        /// <param name="local">If should be this script exclusive.</param>
        /// <remarks>Curly braces will be added automatically if they are not present already.</remarks>
        public static void DefineVariable(string name, string desc, List<Player> players, Script source, bool local = false)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name += "}";

            if (!local)
            {
                DefinedPlayerVariables[name] = new(name, desc, players);
            }
            else if (source is not null && local)
            {
                source.AddPlayerVariable(name, desc, players);
            }
            else
            {
                throw new Exception($"Cannot save the {name} variable.");
            }

            source?.DebugLog($"Defined player variable {name}");
        }

        /// <summary>
        /// Removes all defined variables.
        /// </summary>
        public static void ClearVariables()
        {
            DefinedVariables.Clear();
            DefinedPlayerVariables.Clear();
        }

        /// <summary>
        /// Gets a variable.
        /// </summary>
        /// <param name="name">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <param name="requireBrackets">If brackets are required to parse the variable.</param>
        /// <param name="skipProcessing">If processing is to be skipped.</param>
        /// <returns>A tuple containing the variable and whether or not it's a reversed boolean value.</returns>
        public static VariableResult GetVariable(string name, Script source, bool requireBrackets = true, bool skipProcessing = false)
        {
            DebugLog($"[GetVariable] Getting the '{name}' variable.", source);

            // Do this here so individual files dont have to do it anymore
            if (!requireBrackets)
            {
                name = name.Replace("{", string.Empty).Replace("}", string.Empty);
                name = $"{{{name}}}";
            }

            string variableName;
            List<string> argList = ListPool<string>.Pool.Get();

            string[] arguments = name.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (arguments.Length == 1)
            {
                variableName = arguments[0];
            }
            else
            {
                variableName = arguments[0] + "}";
                foreach (string argument in arguments.Skip(1))
                {
                    string arg = argument;
                    if (arg.EndsWith("}")) arg = arg.Replace("}", string.Empty);
                    argList.Add(arg);
                    DebugLog($"[GetVariable] Formatted argument '{argument} to '{arg}'", source);
                }
            }

            DebugLog($"[GetVariable] Attempting to retrieve variable '{variableName}' with args '{string.Join(", ", argList)}'", source);

            Tuple<IConditionVariable, bool> result = new(null, false);

            bool foundVar = false;
            foreach (IVariableGroup group in Groups)
            {
                foreach (IVariable variable in group.Variables)
                {
                    if (variable.Name == variableName && variable is IConditionVariable condition)
                    {
                        result = new(condition, false);
                        foundVar = true;
                    }
                    else if (variable is IBoolVariable boolVariable && boolVariable.ReversedName == variableName)
                    {
                        result = new(boolVariable, true);
                        foundVar = true;
                    }
                }
            }

            if (!foundVar)
                DebugLog("[GetVariable] The variable provided is not a variable predefined by ScriptedEvents.", source);
            else
                DebugLog("[GetVariable] Variable provided is a variable defined by ScriptedEvents.", source);

            if (DefinedVariables.TryGetValue(name, out CustomVariable customValue))
                result = new(customValue, false);

            if (DefinedPlayerVariables.TryGetValue(name, out CustomPlayerVariable customPlayerValue))
                result = new(customPlayerValue, false);

            if (source is not null && source.UniqueVariables.TryGetValue(name, out CustomVariable uniqueValue))
                result = new(uniqueValue, false);

            if (source is not null && source.UniquePlayerVariables.TryGetValue(name, out CustomPlayerVariable uniquePlayerValue))
                result = new(uniquePlayerValue, false);

            if (result.Item1 is not null)
            {
                if (result.Item1 is IArgumentVariable argSupport)
                {
                    DebugLog("[GetVariable] Variable provided has arguments.", source);
                    argSupport.RawArguments = argList.ToArray();

                    if (!skipProcessing)
                    {
                        ArgumentProcessResult processResult = ArgumentProcessor.Process(argSupport.ExpectedArguments, argSupport.RawArguments, result.Item1, source, false);

                        DebugLog($"[GetVariable] Variable argument processing completed. Success: {processResult.Success} | Message: {processResult.Message ?? "N/A"}", source);

                        if (!processResult.Success)
                            return new(false, result.Item1, processResult.Message, result.Item2);

                        argSupport.Arguments = processResult.NewParameters.ToArray();
                    }
                    else
                    {
                        DebugLog("[GetVariable] Argument processing skipped.", source);
                    }
                }

                if (result.Item1 is INeedSourceVariable sourcePls)
                {
                    sourcePls.Source = source;
                }
            }
            else
            {
                return new(true, null, $"Unknown variable '{name}' provided.", false);
            }

            ListPool<string>.Pool.Return(argList);
            DebugLog($"[GetVariable] Returning the variable value as {result.Item1}", source);
            return new(true, result.Item1, string.Empty, result.Item2);
        }

        public static bool TryGetVariable(string name, Script source, out VariableResult result, bool requireBrackets = true, bool skipProcessing = false)
        {
            DebugLog($"[TryGetVariable] Trying to get the '{name}' variable", source);
            result = GetVariable(name, source, requireBrackets, skipProcessing);

            if (result.Variable is null)
            {
                DebugLog($"[TryGetVariable] Fail! Was unable to get the '{name}' variable.", source);
                return false;
            }

            DebugLog($"[TryGetVariable] Success! The '{name}' variable was successfully extracted.", source);
            return true;
        }

        /// <summary>
        /// Attempts to get a <see cref="IEnumerable{T}"/> of <see cref="Player"/>s based on the input variable.
        /// </summary>
        /// <param name="name">The variable.</param>
        /// <param name="source">The source script.</param>
        /// <param name="players">The players found. If the operation was not successful, this will contain the error reason.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not players were found.</returns>
        /// <remarks>This should be used for variables where <paramref name="requireBrackets"/> is <see langword="false"/>. Otherwise, use <see cref="ScriptModule.TryGetPlayers(string, int?, out Structures.PlayerCollection, Script)"/>.</remarks>
        /// <seealso cref="ScriptModule.TryGetPlayers(string, int?, out PlayerCollection, Script)"/>
        public static bool TryGetPlayers(string name, Script source, out PlayerCollection players, bool requireBrackets = true)
        {
            DebugLog($"[TryGetPlayers] Trying to fetch {name} variable. [requireBrackets {requireBrackets}]", source);
            if (TryGetVariable(name, source, out VariableResult result, requireBrackets) && result.ProcessorSuccess)
            {
                if (result.Variable is IPlayerVariable plrVariable)
                {
                    players = new(plrVariable.Players.ToList());
                    DebugLog($"[TryGetPlayers] Fetch was successful!", source);
                    return true;
                }
                else
                {
                    DebugLog($"[TryGetPlayers] Fetch was unsuccessful! Variable fetched is not a player variable.", source);
                }
            }
            else
            {
                DebugLog($"[TryGetPlayers] Fetch was unsuccessful! {result.Message}", source);
            }

            players = new(null, false, result?.Message ?? "Invalid variable provided.");
            return false;
        }

        /// <summary>
        /// Replaces a string variable, if one is present. Otherwise, returns the same string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">Source script.</param>
        /// <param name="requireBrackets">Require brackets to replace?.</param>
        /// <returns>A string.</returns>
        /// <remarks>This is intended for a string that is either a variable entirely or a string entirely - this will not isolate and replace all variables in a string. See <see cref="ReplaceVariables(string, Script)"/>.</remarks>
        public static string ReplaceVariable(string input, Script source, bool requireBrackets = true)
        {
            if (TryGetVariable(input, source, out VariableResult var, requireBrackets))
            {
                if (!var.ProcessorSuccess) return var.Message;
                return var.String();
            }

            return input;
        }

        /// <summary>
        /// Replaces all the occurrences of variables in a string.
        /// </summary>
        /// <param name="input">The string to perform the replacements on.</param>
        /// <param name="source">The script that is currently running to replace variables. Used only for per-script variables.</param>
        /// <returns>The modified string.</returns>
        /// <remarks>This is intended for strings that contain both regular text and variables. Otherwise, see <see cref="ReplaceVariable(string, Script, bool)"/>.</remarks>
        public static string ReplaceVariables(string input, Script source)
        {
            string[] variables = IsolateVariables(input, source);

            foreach (string variable in variables)
            {
                source.DebugLog("Isolated variable: " + variable);

                if (!TryGetVariable(variable, source, out VariableResult vresult))
                {
                    source.DebugLog("Invalid variable.");
                    continue;
                }

                source.DebugLog("Valid variable.");

                if (!vresult.ProcessorSuccess)
                {
                    Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] Variable '{vresult.Variable.Name}' argument error: {vresult.Message}");
                    continue;
                }

                try
                {
                    input = input.Replace(variable, vresult.String(source, vresult.Reversed));
                }
                catch (InvalidCastException e)
                {
                    Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {ErrorGen.Get(ErrorCode.VariableReplaceError, vresult.Variable.Name, e.Message)}");
                }
                catch (Exception e)
                {
                    Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {ErrorGen.Get(ErrorCode.VariableReplaceError, vresult.Variable.Name, source?.Debug == true ? e : e.Message)}");
                }
            }

            return input;
        }

        public static string ReplaceVariables(object input, Script source)
            => ReplaceVariables(input.ToString(), source);

        /// <summary>
        /// Isolates all variables from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The variables used within the string.</returns>
        public static string[] IsolateVariables(string input, Script source)
        {
            source?.DebugLog($"Isolating variables from: {input}");
            List<string> result = ListPool<string>.Pool.Get();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c is '{')
                {
                    int index = input.IndexOf('}', i);

                    source?.DebugLog($"Detected variable opening symbol, char {i}. Closing index {index}. Substring {index - i + 1}.");

                    string variable = input.Substring(i, index - i + 1);

                    source?.DebugLog($"Variable: {variable}");

                    result.Add(variable);
                }
            }

            return ListPool<string>.Pool.ToArrayReturn(result);
        }

        public override void Init()
        {
            base.Init();
            foreach (Type type in MainPlugin.Singleton.Assembly.GetTypes())
            {
                if (typeof(IVariableGroup).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    IVariableGroup temp = (IVariableGroup)Activator.CreateInstance(type);

                    Log.Debug($"Adding variable group: {type.Name}");
                    Groups.Add(temp);
                }
            }
        }

        private static void DebugLog(string message, Script source)
        {
            source?.DebugLog($"[VariableSystem] {message}");
        }
    }
}
