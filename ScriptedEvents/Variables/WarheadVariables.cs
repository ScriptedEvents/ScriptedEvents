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
            new WarheadArmed(),
            new WarheadOpened(),
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

    public class WarheadArmed : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{WARHEADARMED}";

        /// <inheritdoc/>
        public string ReversedName => "{!WARHEADARMED}";

        /// <inheritdoc/>
        public string Description => "Whether or not the Alpha Warhead is armed (the lever is switched to ON).";

        /// <inheritdoc/>
        public bool Value => Warhead.LeverStatus;
    }

    public class WarheadOpened : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{WARHEADOPENED}";

        /// <inheritdoc/>
        public string ReversedName => "{!WARHEADOPENED}";

        /// <inheritdoc/>
        public string Description => "Whether or not the Alpha Warhead keycard panel (on the surface) is unlocked.";

        /// <inheritdoc/>
        public bool Value => Warhead.IsKeycardActivated;
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
