namespace ScriptedEvents.Variables.Condition.Strings
{
    using System;
#pragma warning disable SA1402 // File may only contain a single type
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using ScriptedEvents.Variables.Interfaces;

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
            new Len(),
            new Show(),
            new DoorState(),
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

    public class Len : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{LEN}";

        /// <inheritdoc/>
        public string Description => "Reveals the length of a player variable.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name of the player variable to retrieve the length of.", true),
        };

        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Arguments.Length == 0)
                    return -1f;

                string name = Arguments[0].Replace("{", string.Empty).Replace("}", string.Empty);

                var variable = PlayerVariables.GetVariable($"{{{name}}}", Source);
                if (variable is not null)
                {
                    return variable.Players.Count();
                }

                return -1f;
            }
        }
    }

    public class Show : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{SHOW}";

        /// <inheritdoc/>
        public string Description => "Reveal certain properties about the players in a player variable.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name of the player variable to show.", true),
            new Argument("selector", typeof(string), "The type to show. Defaults to \"NAME\" Options: NAME, USERID, PLAYERID, ROLE, TEAM, ROOM, ZONE, HP, HEALTH.", false),
        };

        public Script Source { get; set; } = null;

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length == 0)
                    return "ERROR: MISSING VARIABLE NAME";

                string selector = "NAME";

                if (Arguments.Length > 1)
                    selector = Arguments[1].ToUpper();

                string name = Arguments[0].Replace("{", string.Empty).Replace("}", string.Empty);

                var variable = PlayerVariables.GetVariable($"{{{name}}}", Source);
                if (variable is not null)
                {
                    IOrderedEnumerable<string> display = variable.Players.Select(ply =>
                    {
                        return selector switch
                        {
                            "NAME" => ply.Nickname,
                            "USERID" => ply.UserId,
                            "PLAYERID" => ply.Id.ToString(),
                            "ROLE" => ply.Role.Type.ToString(),
                            "TEAM" => ply.Role.Team.ToString(),
                            "ROOM" => ply.CurrentRoom.Type.ToString(),
                            "ZONE" => ply.Zone.ToString(),
                            "HP" or "HEALTH" => ply.Health.ToString(),
                            _ => ply.Nickname,
                        };
                    }).OrderBy(s => s);
                    return string.Join(", ", display);
                }

                return "ERROR: INVALID VARIABLE";
            }
        }
    }

    public class DoorState : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{DOORSTATE}";

        /// <inheritdoc/>
        public string Description => "Reveals the state of a door (either 'OPEN' or 'CLOSED').";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("door", typeof(DoorType), "The door to get the state of.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length < 1) return "ERROR: MISSING DOOR TYPE";

                if (!Enum.TryParse(Arguments[0], out DoorType dt))
                    return "ERROR: INVALID DOOR TYPE";

                Door d = Door.Get(dt);

                if (d is null)
                    return "ERROR: INVALID DOOR TYPE";

                return d.IsOpen ? "OPEN" : "CLOSED";
            }
        }
    }
}
