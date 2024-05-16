namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

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
            bool surroundedWithBothBrackets = name.StartsWith("{") && name.EndsWith("}");
            bool missingOneOrMoreBrackets = !name.StartsWith("{") || !name.EndsWith("}");

            if (!surroundedWithBothBrackets && missingOneOrMoreBrackets)
                source.DebugLog($"Provided variable '{name}' has malformed brackets! [surroundedWithBothBrackets: {surroundedWithBothBrackets}] [missingOneOrMoreBrackets: {missingOneOrMoreBrackets}]");

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
                    source.DebugLog($"Formatted argument '{argument} to '{arg}'");
                }
            }

            source?.DebugLog($"Attempting to retrieve variable '{variableName}' with args '{string.Join(", ", argList)}'");

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
                source?.DebugLog("The variable provided is not a variable predefined by ScriptedEvents.");
            else
                source?.DebugLog("Variable provided is a variable defined by ScriptedEvents.");

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
                    source?.DebugLog("Variable provided has arguments.");
                    argSupport.RawArguments = argList.ToArray();

                    ArgumentProcessResult processResult = ArgumentProcessor.Process(argSupport.ExpectedArguments, argSupport.RawArguments, result.Item1, source, out bool _, false);

                    source?.DebugLog($"Variable argument processing completed. Success: {processResult.Success} | Message: {processResult.Message ?? "N/A"}");

                    if (!processResult.Success && !skipProcessing)
                        return new(false, null, processResult.Message);

                    argSupport.Arguments = processResult.NewParameters.ToArray();
                }

                if (result.Item1 is INeedSourceVariable sourcePls)
                {
                    sourcePls.Source = source;
                }
            }

            ListPool<string>.Pool.Return(argList);
            source?.DebugLog($"Returning the variable value as {result.Item1}");
            return new(true, result.Item1, string.Empty, result.Item2);
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
    }
}
