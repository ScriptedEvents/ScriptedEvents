namespace ScriptedEvents.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Adds tags.
    /// </summary>
    internal static class CreditHandler
    {
        private static readonly CreditTag BetatesterRank = new("ScriptedEvents Betatester", "yellow");
        private static readonly CreditTag DeveloperRank = new("ScriptedEvents Developer", "orange");

        private static readonly Dictionary<string, CreditTag> RegisteredCreditTags = new()
        {
            { "76561199476313529@steam", DeveloperRank },
            { "76561198073944082@steam", BetatesterRank },
            { "76561198308107750@steam", BetatesterRank },
        };

        /// <summary>
        /// Adds a credit tag if applicable.
        /// </summary>
        /// <param name="player">Player.</param>
        internal static void AddCreditTagIfApplicable(Player player)
        {
            if (player.DoNotTrack)
                return;

            bool hasGlobalBadge = player.GlobalBadge.HasValue;
            bool hasRank = !string.IsNullOrEmpty(player.RankName);
            bool hasHiddenRank = !string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge);

            if (hasGlobalBadge || hasRank || hasHiddenRank)
                return;

            if (!RegisteredCreditTags.TryGetValue(player.UserId, out var creditTag))
                return;

            player.RankName = creditTag.Name;
            player.RankColor = creditTag.Color;
        }

        private struct CreditTag
        {
            internal readonly string Name;
            internal readonly string Color;

            internal CreditTag(string name, string color)
            {
                Name = name;
                Color = color;
            }
        }
    }
}