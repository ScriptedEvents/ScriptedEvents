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

    public class VariableSystem : SEModule
    {
        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="IVariableGroup"/> representing all the valid condition variables.
        /// </summary>
        public static List<IVariableGroup> Groups { get; } = new();

        /// <inheritdoc/>
        public override string Name => "VariableSystem";

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, CustomLiteralVariable> DefinedVariables { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of player variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, CustomPlayerVariable> DefinedPlayerVariables { get; } = new();

        public static bool IsValidVariableSyntax<T>(string name, out string processedName)
            where T : IVariable
        {
            processedName = name.ToUpper();

            if (processedName.Length < 2 || processedName.Contains(' '))
            {
                return false;
            }

            if (typeof(T) == typeof(IPlayerVariable) && processedName[0] != '@')
            {
                return false;
            }

            if (typeof(T) == typeof(ILiteralVariable) && processedName[0] != '$')
            {
                return false;
            }

            processedName = processedName.Substring(1);
            return true;
        }

        public static bool TryDefineLiteralVariable(string name, string value, bool isVariableNameVerified)
        {
            if (!isVariableNameVerified && !IsValidVariableSyntax<ILiteralVariable>(name, out name))
            {
                return false;
            }

            DefinedVariables[name] = new(name, string.Empty, value);
            return true;
        }

        public static bool TryDefinePlayerVariable(string name, IEnumerable<Player> value, bool isVariableNameVerified)
        {
            if (!isVariableNameVerified && !IsValidVariableSyntax<IPlayerVariable>(name, out name))
            {
                return false;
            }

            DefinedPlayerVariables[name] = new(name, string.Empty, value);
            return true;
        }

        public static bool TryDefineVariable<T>(T variable)
            where T : class, IVariable
        {
            switch (variable)
            {
                case CustomPlayerVariable playerVariable:
                {
                    DefinedPlayerVariables[playerVariable.Name] = playerVariable;
                    return true;
                }

                case CustomLiteralVariable literalVariable:
                {
                    DefinedVariables[literalVariable.Name] = literalVariable;
                    return true;
                }

                default:
                    return false;
            }
        }

        public static VariableResult GetVariable<T>(string name, Script script, bool isVariableNameVerified)
            where T : IVariable
        {
            if (!isVariableNameVerified && !IsValidVariableSyntax<T>(name, out name))
            {
                return new(false, null, $"Variable name '{name}' is invalid for specified {typeof(T).Name}.");
            }

            foreach (var variable in from @group in Groups from variable in @group.Variables where variable.Name.ToUpper() == name where variable is T select variable)
            {
                return new(true, variable);
            }

            var isIVariable = typeof(T) == typeof(IVariable);

            if (typeof(T) == typeof(ILiteralVariable) || isIVariable)
            {
                if (DefinedVariables.TryGetValue(name, out CustomLiteralVariable customValue))
                {
                    return new(true, customValue);
                }

                if (script.UniqueLiteralVariables.TryGetValue(name, out CustomLiteralVariable uniqueValue))
                {
                    return new(true, uniqueValue);
                }

                if (!isIVariable)
                    return new(false, null, $"The variable name '{name}' does not match any SE predefined, global or local literal variable.");
            }

            if (typeof(T) == typeof(IPlayerVariable) || isIVariable)
            {
                if (DefinedPlayerVariables.TryGetValue(name, out CustomPlayerVariable customValue))
                {
                    return new(true, customValue);
                }

                if (script.UniquePlayerVariables.TryGetValue(name, out CustomPlayerVariable uniqueValue))
                {
                    return new(true, uniqueValue);
                }

                if (!isIVariable)
                    return new(false, null, $"The variable name '{name}' does not match any SE predefined, global or local player variable.");
            }

            if (isIVariable)
                return new(false, null, $"The variable name '{name}' does not match any SE predefined, global or local variable.");

            throw new Exception("Report to developer!");
        }

        public static bool TryGetVariable<T>(string name, Script source, out T? result, bool isVariableNameVerified)
            where T : IVariable
        {
            result = default;
            var variableResult = GetVariable<T>(name, source, isVariableNameVerified);

            if (variableResult.Variable is null || !variableResult.ProcessorSuccess)
                return false;

            result = (T)variableResult.Variable;
            return true;
        }

        public static bool TryGetPlayersFromVariable(string name, Script source, out IEnumerable<Player> result, bool isVariableNameVerified)
        {
            result = Array.Empty<Player>();

            if (!TryGetVariable<IPlayerVariable>(name, source, out var variable, isVariableNameVerified) || variable is null)
                return false;

            result = variable.GetPlayers();
            return true;
        }

        public static bool TryGetStringFromVariable(string name, Script source, out string result, bool isVariableNameVerified)
        {
            result = string.Empty;

            if (!TryGetVariable<ILiteralVariable>(name, source, out var variable, isVariableNameVerified) || variable is null)
                return false;

            result = variable.Value;
            return true;
        }

        /// <summary>
        /// Removes all defined variables.
        /// </summary>
        public static void ClearVariables()
        {
            DefinedVariables.Clear();
            DefinedPlayerVariables.Clear();
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
    }
}
