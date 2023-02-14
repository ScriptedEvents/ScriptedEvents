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
            foreach (IVariableGroup group in Groups)
            {
                foreach (IVariable variable in group.Variables)
                {
                    if (variable.Name == name && variable is IPlayerVariable ply)
                        return ply;
                }
            }

            if (RoleTypeIds.TryGetValue(name, out RoleTypeVariable value))
                return value;

            if (DefinedVariables.TryGetValue(name, out CustomPlayerVariable customVariable))
                return customVariable;

            return null;
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
