namespace ScriptedEvents.Variables.Condition.Strings
{
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using ScriptedEvents.Variables.Interfaces;
    using System.Linq;

    public class StringVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Strings";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new NextWave(),
            new LastRespawnTeam(),
            new LastUnitName(),
            new Show(),
        };
    }

    public class LastRespawnTeam : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{LASTRESPAWNTEAM}";

        /// <inheritdoc/>
        public string Description => "The most recent team that spawn.";

        /// <inheritdoc/>
        public string Value => MainPlugin.Handlers.MostRecentSpawn.ToString();
    }

    public class NextWave : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{NEXTWAVE}";

        /// <inheritdoc/>
        public string Description => "The next team to spawn, either NineTailedFox, ChaosInsurgency, or None.";

        /// <inheritdoc/>
        public string Value => Respawn.NextKnownTeam.ToString();
    }

    public class LastUnitName : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{LASTRESPAWNUNIT}";

        /// <inheritdoc/>
        public string Description => "The most recent team's unit name.";

        /// <inheritdoc/>
        public string Value => MainPlugin.Handlers.MostRecentSpawnUnit;
    }

    public class Show : IStringVariable, IArgumentVariable
    {

        /// <inheritdoc/>
        public string Name => "{SHOW}";

        /// <inheritdoc/>
        public string Description => "Show a player variable as a name or list of names.";

        public string[] Arguments { get; set; }

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name of the player variable to show.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length == 0)
                    return "ERROR: MISSING VARIABLE NAME";

                string name = Arguments[0].Replace("{", string.Empty).Replace("}", string.Empty);

                var variable = PlayerVariables.GetVariable($"{{{name}}}");
                if (variable is not null)
                {
                    return string.Join(", ", variable.Players.Select(r => r.Nickname));
                }

                return "ERROR: INVALID VARIABLE";
            }
        }
    }
}
