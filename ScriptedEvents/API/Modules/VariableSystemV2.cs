namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

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

            name = name.ToUpper();

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

            name = name.ToUpper();

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

        public static VariableResult InternalGetVariable(string initName, Script source)
        {
            void Log(string message)
            {
                DebugLog("[VariableSystem] [InternalGetVariable] " + message, source);
            }

            string name = initName.ToUpper();

            foreach (IVariableGroup group in Groups)
            {
                foreach (IVariable variable in group.Variables)
                {
                    if (variable.Name.ToUpper() != name)
                    {
                        continue;
                    }

                    Log($"Variable {initName} is a predefined SE variable.");
                    return new(true, variable);
                }
            }

            if (DefinedVariables.TryGetValue(name, out CustomVariable customValue))
            {
                Log($"Variable {initName} is a global literal variable.");
                return new(true, customValue);
            }

            if (DefinedPlayerVariables.TryGetValue(name, out CustomPlayerVariable customPlayerValue))
            {
                Log($"Variable {initName} is a global player variable.");
                return new(true, customPlayerValue);
            }

            if (source.UniqueVariables.TryGetValue(name, out CustomVariable uniqueValue))
            {
                Log($"Variable {initName} is a local literal variable.");
                return new(true, uniqueValue);
            }

            if (source.UniquePlayerVariables.TryGetValue(name, out CustomPlayerVariable uniquePlayerValue))
            {
                Log($"Variable {initName} is a local player variable.");
                return new(true, uniquePlayerValue);
            }

            Log($"No variable matches the provided name '{initName}'.");
            return new(false, null, $"The variable name '{initName}' (checked by '{name}') does not match any SE predefined, global or local variable.");
        }

        public static VariableResult InternalGetDynamicVariable(string initName, Script source)
        {
            return new(true, null);
        }

        /// <summary>
        /// Gets a variable.
        /// </summary>
        /// <param name="name">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <returns>A tuple containing the variable and whether or not it's a reversed boolean value.</returns>
        public static VariableResult GetVariable(string name, Script source)
        {
            void Log(string message)
            {
                DebugLog("[GetVariable] " + message, source);
            }

            Log($"Getting the '{name}' variable.");

            if (name.StartsWith("@"))
            {
                Log($"'{name}' is a standard variable since '@' is the first token.");
                return InternalGetVariable(name, source);
            }
            else if (name.CountOccurrences('{') + name.CountOccurrences('}') == 0)
            {
                return new(false, null,
                        $"'{name}' is not a variable. No start token '@' or brackets '{{}}' detected.");
            }

            return new(false, null);

            /*
             * add support for dynacts later
            if (name.CountOccurrences('{') != 1 || name.CountOccurrences('}') != 1)
            {
                return new(false, null,
                    $"The amount of opening ({{) and closing (}}) brackets in the variable '{name}' should be 1 each, but there are '{name.CountOccurrences('{')}' opening and '{name.CountOccurrences('}')}' closing brackets.");
            }

            if (name.First() != '{' || name.Last() != '}')
            {
                return new(false, null,
                    $"The opening ({{) and closing (}}) brackets in the variable '{name}' should be first and last accordingly, but theyre not.");
            }
            */
        }

        public static bool TryGetVariable(string name, Script source, out VariableResult result, bool skipProcessing = false)
        {
            result = GetVariable(name, source);

            if (result.Variable is null)
            {
                return false;
            }

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
        /// <remarks>This should be used for variables where <paramref name="requireBrackets"/> is <see langword="false"/>. Otherwise, use <see cref="SEParser.TryGetPlayers(string, int?, out Structures.PlayerCollection, Script)"/>.</remarks>
        /// <seealso cref="SEParser.TryGetPlayers(string, int?, out PlayerCollection, Script)"/>
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

        public override void Init()
        {
            base.Init();
            foreach (Type type in MainPlugin.Singleton.Assembly.GetTypes())
            {
                if (typeof(IVariableGroup).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    IVariableGroup temp = (IVariableGroup)Activator.CreateInstance(type);

                    Logger.Debug($"Adding variable group: {type.Name}");
                    Groups.Add(temp);
                }
            }
        }

        private static void DebugLog(string message, Script source)
        {
            Logger.Debug($"[VariableSystem] {message}", source);
        }
    }
}
