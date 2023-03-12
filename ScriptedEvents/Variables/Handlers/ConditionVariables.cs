namespace ScriptedEvents.Variables.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Variables.Condition;
    using ScriptedEvents.Variables.Condition.Roles;
    using ScriptedEvents.Variables.Interfaces;
    using Random = UnityEngine.Random;

    /// <summary>
    /// A class used to store and retrieve all non-player variables.
    /// </summary>
    public static class ConditionVariables
    {
        /// <summary>
        /// Maps each <see cref="RoleTypeId"/> variable (eg. "{SCP173}") to a respective <see cref="RoleTypeVariable"/>.
        /// </summary>
        public static readonly Dictionary<string, RoleTypeVariable> RoleTypeIds = ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId))).ToDictionary(x => $"{{{x.ToString().ToUpper()}}}", x => new RoleTypeVariable(x));

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="IVariableGroup"/> representing all the valid condition variables.
        /// </summary>
        public static List<IVariableGroup> Groups { get; } = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, CustomVariable> DefinedVariables { get; } = new();

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

                    if (temp.GroupType is not VariableGroupType.Condition)
                        continue;

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
        /// <remarks>Curly braces will be added automatically if they are not present already.</remarks>
        public static void DefineVariable(string name, string desc, object input)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name = name + "}";

            DefinedVariables[name] = new(name, desc, input);
        }

        /// <summary>
        /// Removes a previously-defined variable.
        /// </summary>
        /// <param name="name">The name of the variable, with curly braces.</param>
        public static void RemoveVariable(string name)
        {
            if (DefinedVariables.ContainsKey(name))
                DefinedVariables.Remove(name);
        }

        /// <summary>
        /// Removes all defined variables.
        /// </summary>
        public static void ClearVariables()
        {
            DefinedVariables.Clear();
        }

        /// <summary>
        /// Alternative to <see cref="string.Replace(string, string)"/> which takes an object as the newValue (and ToStrings it automatically).
        /// </summary>
        /// <param name="input">The string to perform the replacement on.</param>
        /// <param name="oldValue">The string to look for.</param>
        /// <param name="newValue">The value to replace it with.</param>
        /// <returns>The modified string.</returns>
        public static string Replace(this string input, string oldValue, object newValue) => input.Replace(oldValue, newValue.ToString());

        public static Tuple<IConditionVariable, bool> GetVariable(string name, Script source = null)
        {
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

            if (source is not null && source.UniqueVariables.TryGetValue(name, out CustomVariable customValue2))
                result = new(customValue2, false);

            if (result.Item1 is not null && result.Item1 is IArgumentVariable argSupport)
            {
                argSupport.Arguments = argList.ToArray();
            }

            ListPool<string>.Pool.Return(argList);
            return result;
        }

        public static bool TryGetVariable(string name, out IConditionVariable variable, out bool reversed, Script source = null)
        {
            Tuple<IConditionVariable, bool> res = GetVariable(name, source);

            variable = res.Item1;
            reversed = res.Item2;

            return variable != null;
        }

        /// <summary>
        /// Replaces all the occurrences of variables in a string.
        /// </summary>
        /// <param name="input">The string to perform the replacements on.</param>
        /// <param name="source">The script that is currently running to replace variables. Used only for per-script variables.</param>
        /// <returns>The modified string.</returns>
        public static string ReplaceVariables(string input, Script source = null)
        {
            string[] variables = ConditionHelper.IsolateVariables(input);

            foreach (var variable in variables)
            {
                if (TryGetVariable(variable, out IConditionVariable condition, out bool reversed, source))
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
                        case IObjectVariable @object:
                            input = input.Replace(variable, @object.Value);
                            break;
                    }
                }
            }

            return input;
        }
    }
}