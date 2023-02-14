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
    using ScriptedEvents.Variables.Interfaces;
    using ScriptedEvents.Variables.Player.Roles;

    /// <summary>
    /// A class used to store and retrieve all player variables.
    /// </summary>
    public static class PlayerVariables
    {
        public static List<IVariableGroup> Groups { get; } = new();

        public static readonly Dictionary<string, RoleTypeVariable> RoleTypeIds = ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId))).ToDictionary(x => $"{{{x.ToString().ToUpper()}}}", x => new RoleTypeVariable(x));

        internal static Dictionary<string, IEnumerable<Player>> DefinedVariables { get; } = new();

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

        public static void DefineVariable(string name, IEnumerable<Player> input)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name = name + "}";
            DefinedVariables[name] = input;
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

            if (DefinedVariables.TryGetValue(input, out IEnumerable<Player> result))
                return result;

            return null;
        }

        public static bool TryGet(string input, out IEnumerable<Player> players)
        {
            players = Get(input);
            return players != null;
        }
    }
}
