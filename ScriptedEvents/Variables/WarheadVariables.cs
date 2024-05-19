namespace ScriptedEvents.Variables.Warhead
{
    using System;

    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;
#pragma warning disable SA1402 // File may only contain a single type.
    using ScriptedEvents.Variables.Interfaces;

    using Warhead = Exiled.API.Features.Warhead;

    public class WarheadVariables : IVariableGroup
    {
        public string GroupName => "Warhead";

        public IVariable[] Variables { get; } = new IVariable[]
        {
            new GeneralWarhead(),
        };
    }

    public class GeneralWarhead : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{WARHEAD}";

        /// <inheritdoc/>
        public string Description => "All-in-one variable for warhead related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ISDETONATED"),
                new("ISOPEN"),
                new("ISARMED"),
                new("ISCOUNTING"),
                new("DETONATIONTIME")),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                string mode = Arguments[0].ToUpper();

                return mode switch
                {
                    "ISDETONATED" => Warhead.IsDetonated.ToUpper(),
                    "ISOPEN" => Warhead.IsKeycardActivated.ToUpper(),
                    "ISARMED" => Warhead.LeverStatus.ToUpper(),
                    "ISCOUNTING" => Warhead.IsInProgress.ToUpper(),
                    "DETONATIONTIME" => Warhead.DetonationTimer.ToUpper(),
                    _ => throw new ArgumentException("Invalid mode.", mode)
                };
            }
        }

        public string[] RawArguments { get; set; }

        public object[] Arguments { get; set; }
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
