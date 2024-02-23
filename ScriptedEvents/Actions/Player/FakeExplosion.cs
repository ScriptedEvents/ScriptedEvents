namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class CameraShakeAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "FAKEEXPLOSION";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Imitates the Alpha Warhead explosion, without killing anyone.";

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Warhead.Shake();
            return new(true);
        }
    }
}