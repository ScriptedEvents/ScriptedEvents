namespace ScriptedEvents.Variables.Strings
{
    using System;
    using System.Collections.Generic;
#pragma warning disable SA1402 // File may only contain a single type
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class StringVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Strings";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Len(),
            new Show(),
        };
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

                var conditionVariable = VariableSystem.GetVariable($"{{{name}}}", Source);
                if (conditionVariable.Item1 is not null)
                {
                    if (conditionVariable.Item1 is not IPlayerVariable variable)
                    {
                        return -1f;
                    }

                    return variable.Players.Count();
                }

                return -1f;
            }
        }
    }

    public class Command : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{C}";

        /// <inheritdoc/>
        public string Description => "Convert a player variable into a format to use with commands.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(string), "The name of the player variable.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length == 0)
                {
                    return "ERROR: MISSING PLAYER VARIABLE";
                }

                if (VariableSystem.TryGetPlayers(Arguments[0], out IEnumerable<Player> players, null)) // Todo: Support script
                {
                    return string.Join(".", players.Select(plr => plr.Id.ToString()));
                }

                return "ERROR: UNKNOWN PLAYER VARIABLE";
            }
        }
    }

    public class Show : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{SHOW}";

        /// <inheritdoc/>
        public string Description => "Reveal certain properties about the players in a player variable. This variable is designed to only be used with a player variable containing one player. However, it CAN be used with multiple players, and will list the display in the form of a comma-separated list.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name of the player variable to show.", true),
            new Argument("selector", typeof(string), "The type to show. Defaults to \"NAME\" Options: NAME, USERID, PLAYERID, ROLE, TEAM, ROOM, ZONE, HP, HEALTH, INV, INVCOUNT.", false),
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

                var conditionVariable = VariableSystem.GetVariable($"{{{name}}}", Source);
                if (conditionVariable.Item1 is not null)
                {
                    if (conditionVariable.Item1 is not IPlayerVariable variable)
                    {
                        return $"ERROR: No players associated with {conditionVariable.Item1.Name} variable";
                    }

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
                            "INVCOUNT" => ply.Items.Count.ToString(),
                            "INV" => string.Join(", ", ply.Items.Select(item => item.Type)),
                            "GOD" => ply.IsGodModeEnabled.ToString().ToUpper(),
                            _ => ply.Nickname,
                        };
                    }).OrderBy(s => s);
                    return string.Join(", ", display);
                }

                return "ERROR: INVALID VARIABLE";
            }
        }
    }
}
