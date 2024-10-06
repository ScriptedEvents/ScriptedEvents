namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Extensions;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class RandomItemAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "RANDOM-ITEM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RandomEnums;

        /// <inheritdoc/>
        public string Description => "Returns a random 'ItemType'.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, variablesToRet: new[] { (Enum.GetValues(typeof(ItemType)) as ItemType[]).GetRandomValue().ToString() });
        }
    }
}