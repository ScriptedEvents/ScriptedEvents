namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;

    using InventorySystem.Items.Usables.Scp330;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class GiveCandyAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "GIVECANDY";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Inventory;

        /// <inheritdoc/>
        public string Description => "Gives the targeted players a candy.";

        /// <inheritdoc/>
        public string LongDescription => $@" A full list of valid Candy IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((CandyKindID[])Enum.GetValues(typeof(CandyKindID))).Where(r => r is not CandyKindID.None).Select(r => $"- [{r:d}] {r}"))}";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to give the candy to.", true),
            new Argument("item", typeof(CandyKindID), "The candy to give.", true),
            new Argument("amount", typeof(int), "The amount to give. Variables are supported. Default: 1", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse<CandyKindID>(Arguments[1], out CandyKindID itemType, script))
                return new(false, "Invalid candy type provided.");

            int amt = 1;

            if (Arguments.Length > 2)
            {
                if (!VariableSystem.TryParse(Arguments[2], out amt, script))
                    return new(MessageType.NotANumber, this, "amount", Arguments[2]);
            }

            PlayerCollection plys = (PlayerCollection)Arguments[0];

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
