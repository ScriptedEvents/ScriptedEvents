namespace ScriptedEvents.Variables.Booleans
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class BooleanVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Booleans";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new CassieSpeaking(),
            new Decontaminated(),
            new RoundEnded(),
            new RoundInProgress(),
            new RoundStarted(),
            new Scp914Active(),
            new WarheadCounting(),
            new WarheadDetonated(),
            new WaveRespawning(),

            new IsScriptRunning(),
        };
    }

    public class CassieSpeaking : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{CASSIESPEAKING}";

        /// <inheritdoc/>
        public string ReversedName => "{!CASSIESPEAKING}";

        /// <inheritdoc/>
        public string Description => "Whether or not CASSIE is currently speaking.";

        /// <inheritdoc/>
        public bool Value => Cassie.IsSpeaking;
    }

    public class Decontaminated : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{DECONTAMINATED}";

        /// <inheritdoc/>
        public string ReversedName => "{!DECONTAMINATED}";

        /// <inheritdoc/>
        public string Description => "Whether or not Light Containment Zone has been decontaminated.";

        /// <inheritdoc/>
        public bool Value => Map.IsLczDecontaminated;
    }

    public class RoundEnded : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDENDED}";

        /// <inheritdoc/>
        public string ReversedName => "{!ROUNDENDED}";

        /// <inheritdoc/>
        public string Description => "Whether or not the round has ended.";

        /// <inheritdoc/>
        public bool Value => Round.IsEnded;
    }

    public class RoundInProgress : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDINPROGRESS}";

        /// <inheritdoc/>
        public string ReversedName => "{!ROUNDINPROGRESS}";

        /// <inheritdoc/>
        public string Description => "Whether or not the round is in progress.";

        /// <inheritdoc/>
        public bool Value => Round.InProgress;
    }

    public class RoundStarted : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDSTARTED}";

        /// <inheritdoc/>
        public string ReversedName => "{!ROUNDSTARTED}";

        /// <inheritdoc/>
        public string Description => "Whether or not the round has started.";

        /// <inheritdoc/>
        public bool Value => Round.IsStarted;
    }

    public class Scp914Active : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCP914ACTIVE}";

        /// <inheritdoc/>
        public string ReversedName => "{!SCP914ACTIVE}";

        /// <inheritdoc/>
        public string Description => "Whether or not SCP-914 is currently active.";

        /// <inheritdoc/>
        public bool Value => Scp914.IsWorking;
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

    public class WaveRespawning : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{WAVERESPAWNING}";

        /// <inheritdoc/>
        public string ReversedName => "{!WAVERESPAWNING}";

        /// <inheritdoc/>
        public string Description => "Whether or not a wave has respawned within the last 5 seconds.";

        /// <inheritdoc/>
        public bool Value => MainPlugin.Handlers.IsRespawning;
    }

    public class IsScriptRunning : IBoolVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{ISRUNNING}";

        /// <inheritdoc/>
        public string ReversedName => "{!ISRUNNING}";

        /// <inheritdoc/>
        public string Description => "Whether or not a specific script is running.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("scriptName", typeof(string), "The name of the script.", true),
        };

        /// <inheritdoc/>
        public bool Value
        {
            get
            {
                if (Arguments.Length < 1) return false;

                string scriptName = Arguments[0];
                return ScriptHelper.RunningScripts.Any(scr => scr.Key.ScriptName == scriptName && scr.Value.IsRunning);
            }
        }
    }
}
