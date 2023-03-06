namespace ScriptedEvents.Variables.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.CustomRoles.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Variables.Condition;
    using ScriptedEvents.Variables.Interfaces;
    using ScriptedEvents.Variables.Player.Roles;

    /// <summary>
    /// A class used to store and retrieve all player variables.
    /// </summary>
    public static class PlayerVariables
    {
        /// <summary>
        /// Maps each <see cref="RoleTypeId"/> variable (eg. "{SCP173}") to a respective <see cref="RoleTypeVariable"/>.
        /// </summary>
        public static readonly Dictionary<string, RoleTypeVariable> RoleTypeIds = ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId))).ToDictionary(x => $"{{{x.ToString().ToUpper()}}}", x => new RoleTypeVariable(x));

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="IVariableGroup"/> representing all the valid player variables.
        /// </summary>
        public static List<IVariableGroup> Groups { get; } = new();

        internal static Dictionary<string, CustomPlayerVariable> DefinedVariables { get; } = new();

        /// <summary>
        /// Sets up the player variable system by adding every <see cref="IVariable"/> related to player variables to the <see cref="Groups"/> list.
        /// </summary>
        public static void Setup()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(IVariableGroup).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    IVariableGroup temp = (IVariableGroup)Activator.CreateInstance(type);

                    if (temp.GroupType is not VariableGroupType.Player)
                        continue;

                    Log.Debug($"Adding variable group: {type.Name}");
                    Groups.Add(temp);
                }
            }
        }

        public static void DefineVariable(string name, string desc, IEnumerable<Player> input)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name = name + "}";
            DefinedVariables[name] = new(name, desc, input);
        }

        public static void ClearVariables()
        {
            DefinedVariables.Clear();
        }

        public static IPlayerVariable GetVariable(string name)
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

            IPlayerVariable result = null;

            foreach (IVariableGroup group in Groups)
            {
                foreach (IVariable variable in group.Variables)
                {
                    if (variable.Name == variableName && variable is IPlayerVariable condition)
                        result = condition;
                }
            }

            if (RoleTypeIds.TryGetValue(name, out RoleTypeVariable value))
                result = value;

            if (DefinedVariables.TryGetValue(name, out CustomPlayerVariable customValue))
                result = customValue;

            if (CustomRoleTypeVariable.TryGetValue(name, out CustomRoleTypeVariable customRole))
                result = customRole;

            if (result is not null && result is IArgumentVariable argSupport)
            {
                argSupport.Arguments = argList.ToArray();
            }

            ListPool<string>.Pool.Return(argList);
            return result;
        }

        public static bool TryGetVariable(string name, out IPlayerVariable variable)
        {
            variable = GetVariable(name);
            return variable != null;
        }

        public static IEnumerable<Player> Get(string input)
        {
            input = input.RemoveWhitespace();

            if (TryGetVariable(input, out IPlayerVariable variable))
                return variable.Players;

            return null;
        }

        public static bool TryGet(string input, out IEnumerable<Player> players)
        {
            players = Get(input);
            return players != null;
        }
    }
}
