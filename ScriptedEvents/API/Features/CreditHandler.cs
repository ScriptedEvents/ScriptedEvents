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
        private static readonly CreditTag SupportRank = new("ScriptedEvents Support", "aqua");
        private static readonly CreditTag QualityAssuranceRank = new("ScriptedEvents Quality Assurance", "deep_pink");

        private static readonly Dictionary<string, CreditTag> RegisteredCreditTags = new()
        {
            { "76561199476313529@steam", DeveloperRank },
            { "76561198073944082@steam", QualityAssuranceRank },
            { "76561198308107750@steam", BetatesterRank },
            { "76561198980842957@steam", SupportRank },
            { "76561198145306256@steam", new("ScriptedEvents Betatester", "cyan") }
        };

        /// <summary>
        /// Adds a credit tag if applicable.
        /// </summary>
        /// <param name="player">Player.</param>
        internal static bool AddCreditTagIfApplicable(Player player)
        {
            if (!MainPlugin.Singleton.EnabledRanks)
                return false;
            
            bool hasGlobalBadge = player.GlobalBadge.HasValue;
            bool hasRank = !string.IsNullOrEmpty(player.RankName);
            bool hasHiddenRank = !string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge);

            if (hasGlobalBadge || hasRank || hasHiddenRank)
                return false;

            if (!RegisteredCreditTags.TryGetValue(player.UserId, out var creditTag))
                return false;

            player.RankName = creditTag.Name;
            player.RankColor = creditTag.Color;
            return true;
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