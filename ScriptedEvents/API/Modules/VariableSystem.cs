namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
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
        private static Dictionary<string, CustomLiteralVariable> GlobalLiteralVariables { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of player variables that were defined in run-time.
        /// </summary>
        private static Dictionary<string, CustomPlayerVariable> GlobalPlayerVariables { get; } = new();

        public static bool IsValidVariableSyntax<T>(string name, out string processedName, out ErrorInfo? errorInfo)
            where T : IVariable
        {
            processedName = name.ToUpper();

            if (processedName.Length < 2 || processedName.Contains(' '))
            {
                errorInfo = Error(
                    "Invalid variable syntax",
                    $"Provided variable name '{processedName}' is too short or contains whitespace, and is therefore not valid.");
                return false;
            }

            if (typeof(T) == typeof(IPlayerVariable) && processedName[0] != '@')
            {
                errorInfo = Error(
                    "Invalid player variable syntax",
                    $"Provided variable name '{processedName}' doesn't have the starting '@' token, therefore is's not a valid player variable.");
                return false;
            }

            if (typeof(T) == typeof(ILiteralVariable) && processedName[0] != '$')
            {
                errorInfo = Error(
                    "Invalid literal variable syntax",
                    $"Provided variable name '{processedName}' doesn't have the starting '$' token, therefore is's not a valid literal variable.");
                return false;
            }

            errorInfo = null;
            processedName = processedName.Substring(1);
            return true;
        }

        public static bool TryDefineLiteralVariable(string name, string value, bool isVariableNameVerified, out ErrorTrace? errorTrace)
        {
            if (!isVariableNameVerified && !IsValidVariableSyntax<ILiteralVariable>(name, out name, out var err))
            {
                errorTrace = new(err!);
                errorTrace.Append(Error("Literal variable defining error", $"Literal variable '{name}' cant be defined since its syntax is invalid."));
                return false;
            }

            errorTrace = null;
            GlobalLiteralVariables[name] = new(name, string.Empty, value);
            return true;
        }

        public static bool TryDefinePlayerVariable(string name, IEnumerable<Player> value, bool isVariableNameVerified, out ErrorTrace? errorTrace)
        {
            if (!isVariableNameVerified && !IsValidVariableSyntax<IPlayerVariable>(name, out name, out var err))
            {
                errorTrace = new(err!);
                errorTrace.Append(Error("Player variable defining error", $"Player variable '{name}' cant be defined since its syntax is invalid."));
                return false;
            }

            errorTrace = null;
            GlobalPlayerVariables[name] = new(name, string.Empty, value);
            return true;
        }

        public static void DefineGlobalVariable<T>(T variable)
            where T : class, IVariable
        {
            switch (variable)
            {
                case CustomPlayerVariable playerVariable:
                {
                    GlobalPlayerVariables[playerVariable.Name] = playerVariable;
                    return;
                }

                case CustomLiteralVariable literalVariable:
                {
                    GlobalLiteralVariables[literalVariable.Name] = literalVariable;
                    return;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IVariable? GetVariable<T>(string name, Script script, bool isVariableNameVerified, out ErrorTrace? trace)
            where T : IVariable
        {
            trace = null;
            if (!isVariableNameVerified && !IsValidVariableSyntax<T>(name, out name, out var err))
            {
                trace = new(err!);
                trace.Append(Error("Variable fetch error", $"Variable '{name}' cannot be fetched since its syntax is invalid."));
                return null;
            }

            foreach (var variable in from @group in Groups from variable in @group.Variables where variable.Name.ToUpper() == name where variable is T select variable)
            {
                return variable;
            }

            var isAnyVariable = typeof(T) == typeof(IVariable);
            var isLiteralVariable = typeof(T) == typeof(ILiteralVariable);
            var isPlayerVariable = typeof(T) == typeof(IPlayerVariable);

            if (isLiteralVariable || isAnyVariable)
            {
                if (GlobalLiteralVariables.TryGetValue(name, out CustomLiteralVariable literal))
                {
                    return literal;
                }

                if (script.LocalLiteralVariables.TryGetValue(name, out CustomLiteralVariable literal2))
                {
                    return literal2;
                }

                if (isLiteralVariable)
                {
                    trace = Error(
                        "Variable doesn't exist",
                        $"There exists no literal variable under the name of '{name}'.").ToTrace();
                    return null;
                }
            }

            if (isPlayerVariable || isAnyVariable)
            {
                if (GlobalPlayerVariables.TryGetValue(name, out CustomPlayerVariable player))
                {
                    return player;
                }

                if (script.LocalPlayerVariables.TryGetValue(name, out CustomPlayerVariable player2))
                {
                    return player2;
                }

                if (isPlayerVariable)
                {
                    trace = Error(
                        "Variable doesn't exist",
                        $"There exists no player variable under the name of '{name}' in the existing scope.").ToTrace();
                    return null;
                }
            }

            trace = Error(
                "Variable doesn't exist",
                $"There exists no variable (either player or literal) under the name of '{name}'.").ToTrace();
            return null;
        }

        public static bool TryGetVariable<T>(string name, Script source, out T? result, bool isVariableNameVerified, out ErrorTrace? trace)
            where T : IVariable
        {
            result = default;
            var variableResult = GetVariable<T>(name, source, isVariableNameVerified, out trace);

            if (variableResult is null)
                return false;

            result = (T)variableResult;
            return true;
        }

        public static bool TryGetPlayersFromVariable(string name, Script source, out IEnumerable<Player> result, bool isVariableNameVerified, out ErrorTrace? trace)
        {
            result = Array.Empty<Player>();

            if (!TryGetVariable<IPlayerVariable>(name, source, out var variable, isVariableNameVerified, out trace))
                return false;

            result = variable!.GetPlayers();
            return true;
        }

        public static bool TryGetStringFromVariable(string name, Script source, out string result, bool isVariableNameVerified, out ErrorTrace? trace)
        {
            result = string.Empty;

            if (!TryGetVariable<ILiteralVariable>(name, source, out var variable, isVariableNameVerified, out trace) || variable is null)
                return false;

            result = variable.Value;
            return true;
        }

        /// <summary>
        /// Removes all defined variables.
        /// </summary>
        public static void ClearVariables()
        {
            GlobalLiteralVariables.Clear();
            GlobalPlayerVariables.Clear();
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

        private static ErrorInfo Error(string name, string description)
        {
            return new(name, description, "VariableSystem");
        }
    }
}
