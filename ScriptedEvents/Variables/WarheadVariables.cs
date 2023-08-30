namespace ScriptedEvents.Variables.Warhead
{
#pragma warning disable SA1402 // File may only contain a single type.
    using ScriptedEvents.Variables.Interfaces;

    using Warhead = Exiled.API.Features.Warhead;

    public class WarheadVariables : IVariableGroup
    {
        public string GroupName => "Warhead";

        public IVariable[] Variables { get; } = new IVariable[]
        {
            new DetonationTime(),
            new WarheadCounting(),
            new WarheadDetonated(),
        };
    }

    public class DetonationTime : IFloatVariable
    {
        /// <inheritdoc/>
        public float Value => Warhead.DetonationTimer;

        /// <inheritdoc/>
        public string Name => "{DETONATIONTIME}";

        /// <inheritdoc/>
        public string Description => "The amount of time until the warhead detonates.";
    }

    public class WarheadCounting : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{WARHEADCOUNTING}";

        /// <inheritdoc/>
        public string ReversedName => "{!WARHEADCOUNTING}";

        /// <inheritdoc/>
        public string Description => "Whether or not the Alpha Warhead is currently counting down.";

        /// <inheritdoc/>
        public bool Value => Warhead.IsInProgress;
    }

    public class WarheadDetonated : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{WARHEADDETONATED}";

        /// <inheritdoc/>
        public string ReversedName => "{!WARHEADDETONATED}";

        /// <inheritdoc/>
        public string Description => "Whether or not the warhead has been detonated.";

        /// <inheritdoc/>
        public bool Value => Warhead.IsDetonated;
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
