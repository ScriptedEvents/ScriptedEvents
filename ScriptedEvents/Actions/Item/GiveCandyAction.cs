namespace ScriptedEvents.Actions.Item
{
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using InventorySystem.Items.Usables.Scp330;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class GiveCandyAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "GiveCandy";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Gives the specified players a candy.";

        /// <inheritdoc/>
        public string LongDescription => $@" A full list of valid Candy IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((CandyKindID[])Enum.GetValues(typeof(CandyKindID))).Where(r => r is not CandyKindID.None).Select(r => $"- [{r:d}] {r}"))}";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to give the candy to.", true),
            new Argument("item", typeof(CandyKindID), "The candy to give.", true),
            new Argument("amount", typeof(int), "The amount to give. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var plys = (Player[])Arguments[0]!;
            var itemType = (CandyKindID)Arguments[1]!;
            int amt = (int?)Arguments[2] ?? 1;

            foreach (Player player in plys)
            {
                for (int i = 0; i < amt; i++)
                {
                    player.TryAddCandy(itemType);
                }
            }

            return new(true);
        }
    }
}
