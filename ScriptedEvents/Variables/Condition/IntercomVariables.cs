namespace ScriptedEvents.Variables.Intercom
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class IntercomVariables : IVariableGroup
    {
        public string GroupName => "Intercom";

        public VariableGroupType GroupType => VariableGroupType.Condition;

        public IVariable[] Variables { get; } = new IVariable[]
        {
            new IntercomReady(),
            new IntercomInUse(),
            new IntercomCooldown(),
            new IntercomUseTime(),
        };
    }

    public class IntercomReady : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMREADY}";

        /// <inheritdoc/>
        public string ReversedName => "{!INTERCOMREADY}";

        /// <inheritdoc/>
        public string Description => "Indicates whether or not the intercom is ready to be used.";

        /// <inheritdoc/>
        public bool Value => !Intercom.InUse && Intercom.RemainingCooldown <= 0;
    }

    public class IntercomInUse : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMINUSE}";

        /// <inheritdoc/>
        public string ReversedName => "{!INTERCOMINUSE}";

        /// <inheritdoc/>
        public string Description => "Indicates whether or not the intercom is currently being used.";

        /// <inheritdoc/>
        public bool Value => Intercom.InUse;
    }

    public class IntercomCooldown : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMCOOLDOWN}";

        /// <inheritdoc/>
        public string Description => "Indicates how much time is left on the intercom's cooldown.";

        /// <inheritdoc/>
        public float Value => (float)Intercom.RemainingCooldown;
    }

    public class IntercomUseTime : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMUSETIME}";

        /// <inheritdoc/>
        public string Description => "Indicates how much time is left for the intercom speaker to speak.";

        /// <inheritdoc/>
        public float Value => Intercom.SpeechRemainingTime;
    }
}
