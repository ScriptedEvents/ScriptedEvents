using ScriptedEvents.Enums;

namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pools;

    using PlayerRoles;
    using Respawning;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// Tool to generate error messages and strings, for consistency between all actions.
    /// </summary>
    public static class MsgGen
    {
        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of types and a string to show for them.
        /// </summary>
        public static Dictionary<Type, string> TypeToString { get; } = new()
        {
            { typeof(string), "String (Message/Text)" },
            { typeof(char), "Character" },
            { typeof(int), "Int (Whole Number)" },
            { typeof(byte), "Byte (Whole Number, 0-255)" },
            { typeof(float), "Float (Number)" },
            { typeof(bool), "Boolean (TRUE/FALSE)" },
            { typeof(Player[]), "Player List" },
            { typeof(Door[]), "Door List" },
            { typeof(Room[]), "Room List" },
            { typeof(RoleTypeId), "RoleTypeId (ID / Number)" },
            { typeof(SpawnableTeamType), "Spawnable Team (ChaosInsurgency OR NineTailedFox)" },
            { typeof(RoomType), "RoomType (ID / Number)" },
            { typeof(IVariable), "Variable" },
            { typeof(IPlayerVariable), "Player Variable" },
            { typeof(ILiteralVariable), "Literal (Raw Text) Variable" },
            { typeof(object), "Any Type" },
        };

        /// <summary>
        /// Gets a pretty display for a type.
        /// </summary>
        /// <param name="type">The type to get the display of.</param>
        /// <returns>The display.</returns>
        public static string Display(this Type type)
        {
            if (TypeToString.TryGetValue(type, out string display))
                return display;

            return type.Name;
        }

        /// <summary>
        /// Gets a pretty display for a <see cref="ActionSubgroup"/>.
        /// </summary>
        /// <param name="group">The <see cref="ActionSubgroup"/>.</param>
        /// <returns>The display.</returns>
        public static string Display(this ActionSubgroup group)
        {
            return group switch
            {
                ActionSubgroup.Cassie => "C.A.S.S.I.E",
                ActionSubgroup.Misc => "Miscellaneous",
                ActionSubgroup.RoundRule => "Round Rule",
                _ => group.ToString(),
            };
        }
    }
}
