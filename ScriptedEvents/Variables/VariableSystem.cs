namespace ScriptedEvents.Variables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// A class used to store and retrieve all variables.
    /// </summary>
    public static class VariableSystem
    {
        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="IVariableGroup"/> representing all the valid condition variables.
        /// </summary>
        public static List<IVariableGroup> Groups { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, CustomVariable> DefinedVariables { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of player variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, CustomPlayerVariable> DefinedPlayerVariables { get; } = new();

        /// <summary>
        /// Sets up the player variable system by adding every <see cref="IVariable"/> related to conditional variables to the <see cref="Groups"/> list.
        /// </summary>
        public static void Setup()
        {
            Log.Debug("Initializing variable system");
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(IVariableGroup).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    IVariableGroup temp = (IVariableGroup)Activator.CreateInstance(type);

                    Log.Debug($"Adding variable group: {type.Name}");
                    Groups.Add(temp);
                }
            }
        }

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="desc">A description of the variable.</param>
        /// <param name="input">The value of the variable.</param>
        /// <param name="source">The script source.</param>
        /// <remarks>Curly braces will be added automatically if they are not present already.</remarks>
        public static void DefineVariable(string name, string desc, string input, Script source = null)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name += "}";

            source?.DebugLog($"Defined variable {name} with value {input}");

            DefinedVariables[name] = new(name, desc, input);
        }

        /// <summary>
        /// Defines a player variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="desc">A description of the variable.</param>
        /// <param name="players">The players.</param>
        /// <remarks>Curly braces will be added automatically if they are not present already.</remarks>
        public static void DefineVariable(string name, string desc, List<Player> players)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name += "}";

            DefinedPlayerVariables[name] = new(name, desc, players);
        }

        /// <summary>
        /// Removes a previously-defined variable.
        /// </summary>
        /// <param name="name">The name of the variable, with curly braces.</param>
        public static void RemoveVariable(string name)
        {
            if (DefinedVariables.ContainsKey(name))
                DefinedVariables.Remove(name);

            if (DefinedPlayerVariables.ContainsKey(name))
                DefinedPlayerVariables.Remove(name);
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
        /// <returns>A tuple containing the variable and whether or not it's a reversed boolean value.</returns>
        public static Tuple<IConditionVariable, bool> GetVariable(string name, Script source = null, bool requireBrackets = true)
        {
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
                }
            }

            source?.DebugLog($"Attempting to retrieve variable {variableName} with args {string.Join(" ", argList)}");

            Tuple<IConditionVariable, bool> result = new(null, false);

            foreach (IVariableGroup group in Groups)
            {
                foreach (IVariable variable in group.Variables)
                {
                    if (variable.Name == variableName && variable is IConditionVariable condition)
                        result = new(condition, false);
                    else if (variable is IBoolVariable boolVariable && boolVariable.ReversedName == variableName)
                        result = new(boolVariable, true);
                }
            }

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
                    argSupport.Arguments = argList.ToArray();
                }

                if (result.Item1 is INeedSourceVariable sourcePls)
                {
                    sourcePls.Source = source;
                }
            }

            ListPool<string>.Pool.Return(argList);
            return result;
        }

        /// <summary>
        /// Try-gets a variable.
        /// </summary>
        /// <param name="name">The input string.</param>
        /// <param name="variable">The variable found, if successful.</param>
        /// <param name="reversed">If the value is a reversed boolean value.</param>
        /// <param name="source">The script source.</param>
        /// <param name="requireBrackets">If brackets are required to parse the variable.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetVariable(string name, out IConditionVariable variable, out bool reversed, Script source = null, bool requireBrackets = true)
        {
            Tuple<IConditionVariable, bool> res = GetVariable(name, source, requireBrackets);

            variable = res.Item1;
            reversed = res.Item2;

            return variable != null;
        }

        /// <summary>
        /// Attempts to get a <see cref="IEnumerable{T}"/> of <see cref="Player"/>s based on the input variable.
        /// </summary>
        /// <param name="name">The variable.</param>
        /// <param name="players">The players found. Can be <see langword="null"/>.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not players were found.</returns>
        /// <remarks>This should be used for variables where <paramref name="requireBrackets"/> is <see langword="false"/>. Otherwise, use <see cref="ScriptHelper.TryGetPlayers(string, int?, out Structures.PlayerCollection, Script)"/>.</remarks>
        /// <seealso cref="ScriptHelper.TryGetPlayers(string, int?, out Structures.PlayerCollection, Script)"/>
        public static bool TryGetPlayers(string name, out PlayerCollection players, Script source = null, bool requireBrackets = true)
        {
            if (TryGetVariable(name, out IConditionVariable variable, out _, source, requireBrackets))
            {
                if (variable is IPlayerVariable plrVariable)
                {
                    players = new(plrVariable.Players.ToList());
                    return true;
                }
            }

            players = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="float"/>. Functionally similar to <see cref="float.Parse(string)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>The result of the cast, or <see cref="float.NaN"/> if the cast failed.</returns>
        public static float Parse(string input, Script source = null, bool requireBrackets = true)
        {
            if (float.TryParse(input, out float fl))
                return fl;

            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IFloatVariable floatVar)
                    return floatVar.Value;
                if (var is ILongVariable longVar)
                    return longVar.Value;
                else if (var is IStringVariable stringVar && float.TryParse(stringVar.Value, out float res))
                    return res;
            }

            return float.NaN;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="int"/>. Functionally similar to <see cref="float.Parse(string)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>The result of the cast, or <see cref="int.MinValue"/> if the cast failed.</returns>
        public static int ParseInt(string input, Script source = null, bool requireBrackets = true)
        {
            if (int.TryParse(input, out int fl))
                return fl;

            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IFloatVariable floatVar)
                    return (int)floatVar.Value;
                if (var is ILongVariable longVar)
                    return (int)longVar.Value;
                else if (var is IStringVariable stringVar && int.TryParse(stringVar.Value, out int res))
                    return res;
            }

            return int.MinValue;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="long"/>. Functionally similar to <see cref="long.Parse(string)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>The result of the cast, or <see cref="long.MinValue"/> if the cast failed.</returns>
        public static long ParseLong(string input, Script source = null, bool requireBrackets = true)
        {
            if (long.TryParse(input, out long fl))
                return fl;

            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IFloatVariable floatVar)
                    return (long)floatVar.Value;
                if (var is ILongVariable longVar)
                    return longVar.Value;
                else if (var is IStringVariable stringVar && long.TryParse(stringVar.Value, out long res))
                    return res;
            }

            return int.MinValue;
        }

        /// <summary>
        /// Replaces a string variable, if one is present. Otherwise, returns the same string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">Source script.</param>
        /// <param name="requireBrackets">Require brackets to replace?.</param>
        /// <returns>A string.</returns>
        /// <remarks>This is intended for a string that is either a variable entirely or a string entirely - this will not isolate and replace all variables in a string. See <see cref="ReplaceVariables(string, Script)"/>.</remarks>
        public static string ReplaceVariable(string input, Script source = null, bool requireBrackets = true)
        {
            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IStringVariable str)
                    return str.Value;
            }

            return input;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="float"/>. Functionally similar to <see cref="float.TryParse(string, out float)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParse(string input, out float result, Script source = null, bool requireBrackets = true)
        {
            result = Parse(input, source, requireBrackets);
            return result != float.NaN;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="int"/>. Functionally similar to <see cref="int.TryParse(string, out int)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParse(string input, out int result, Script source = null, bool requireBrackets = true)
        {
            result = ParseInt(input, source, requireBrackets);
            return result != int.MinValue;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="long"/>. Functionally similar to <see cref="long.TryParse(string, out int)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParse(string input, out long result, Script source = null, bool requireBrackets = true)
        {
            result = ParseLong(input, source, requireBrackets);
            return result != long.MinValue;
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
        public static bool TryParse<T>(string input, out T result, Script source = null, bool requireBrackets = true)
            where T : struct, Enum
        {
            if (Enum.TryParse(input, true, out result))
            {
                return true;
            }

            if (TryGetVariable(input, out IConditionVariable vr, out _, source, requireBrackets))
            {
                if (vr is IStringVariable strVar && Enum.TryParse(strVar.Value, true, out result))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Replaces all the occurrences of variables in a string.
        /// </summary>
        /// <param name="input">The string to perform the replacements on.</param>
        /// <param name="source">The script that is currently running to replace variables. Used only for per-script variables.</param>
        /// <returns>The modified string.</returns>
        /// <remarks>This is intended for strings that contain both regular text and variables. Otherwise, see <see cref="ReplaceVariable(string, Script, bool)"/>.</remarks>
        public static string ReplaceVariables(string input, Script source = null)
        {
            string[] variables = IsolateVariables(input, source);

            foreach (var variable in variables)
            {
                if (TryGetVariable(variable, out IConditionVariable condition, out bool reversed, source))
                {
                    try
                    {
                        switch (condition)
                        {
                            case IBoolVariable @bool:
                                bool result = reversed ? !@bool.Value : @bool.Value;
                                input = input.Replace(variable, result ? "TRUE" : "FALSE");
                                break;
                            case IFloatVariable @float:
                                input = input.Replace(variable, @float.Value);
                                break;
                            case ILongVariable @long:
                                input = input.Replace(variable, @long.Value);
                                break;
                            case IStringVariable @string:
                                input = input.Replace(variable, @string.Value);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {ErrorGen.Get(140, condition.Name, source?.Debug == true ? e : e.Message)}");
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// Isolates all variables from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The variables used within the string.</returns>
        public static string[] IsolateVariables(string input, Script source = null)
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
    }
}