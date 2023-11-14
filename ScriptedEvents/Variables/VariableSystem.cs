namespace ScriptedEvents.Variables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using PlayerRoles;
    using ScriptedEvents.API.Features;

    using ScriptedEvents.Variables.Interfaces;
    using ScriptedEvents.Variables.Roles;

    /// <summary>
    /// A class used to store and retrieve all variables.
    /// </summary>
    public static class VariableSystem
    {
        /// <summary>
        /// Maps each <see cref="RoleTypeId"/> variable (eg. "{SCP173}") to a respective <see cref="RoleTypeVariable"/>.
        /// </summary>
        public static readonly Dictionary<string, RoleTypeVariable> RoleTypeIds = ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId)))
            .Where(role => role is not RoleTypeId.None)
            .ToDictionary(x => $"{{{x.ToString().ToUpper()}}}", x => new RoleTypeVariable(x));

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
                name = name + "}";

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
                name = name + "}";

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
        /// Alternative to <see cref="string.Replace(string, string)"/> which takes an object as the newValue (and ToStrings it automatically).
        /// </summary>
        /// <param name="input">The string to perform the replacement on.</param>
        /// <param name="oldValue">The string to look for.</param>
        /// <param name="newValue">The value to replace it with.</param>
        /// <returns>The modified string.</returns>
        public static string Replace(this string input, string oldValue, object newValue) => input.Replace(oldValue, newValue.ToString());

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

            if (RoleTypeIds.TryGetValue(name, out RoleTypeVariable value))
                result = new(value, false);

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

        public static bool TryGetVariable(string name, out IConditionVariable variable, out bool reversed, Script source = null, bool requireBrackets = true)
        {
            Tuple<IConditionVariable, bool> res = GetVariable(name, source, requireBrackets);

            variable = res.Item1;
            reversed = res.Item2;

            return variable != null;
        }

        public static bool TryGetPlayers(string name, out IEnumerable<Exiled.API.Features.Player> players, Script source = null, bool requireBrackets = true)
        {
            if (TryGetVariable(name, out IConditionVariable variable, out _, source, requireBrackets))
            {
                if (variable is IPlayerVariable plrVariable)
                {
                    players = plrVariable.Players;
                    return true;
                }
            }

            players = null;
            return false;
        }

        public static float Parse(string input, Script source = null, bool requireBrackets = true)
        {
            if (float.TryParse(input, out float fl))
                return fl;

            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IFloatVariable floatVar)
                    return floatVar.Value;
                else if (var is IStringVariable stringVar && float.TryParse(stringVar.Value, out float res))
                    return res;
            }

            return float.NaN;
        }

        public static string ReplaceVariable(string input, Script source = null, bool requireBrackets = true)
        {
            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IStringVariable str)
                    return str.Value;
            }

            return input;
        }

        public static bool TryParse(string input, out float result, Script source = null, bool requireBrackets = true)
        {
            result = Parse(input, source, requireBrackets);
            return result != float.NaN;
        }

        public static bool TryParse(string input, out int result, Script source = null, bool requireBrackets = true)
        {
            float floatResult = Parse(input, source, requireBrackets);

            if (floatResult == float.NaN)
            {
                result = -1;
                return false;
            }

            result = (int)floatResult;
            return result == floatResult;
        }

        public static bool TryParse<T>(string input, out T result, Script source = null, bool requireBrackets = true)
            where T : struct
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
        public static string ReplaceVariables(string input, Script source = null)
        {
            string[] variables = ConditionHelper.IsolateVariables(input, source);

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
                            case IStringVariable @string:
                                input = input.Replace(variable, @string.Value);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] Error replacing the {condition.Name} variable: {e}");
                    }
                }
            }

            return input;
        }
    }
}