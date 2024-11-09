using ScriptedEvents.Variables.PlayerCount;

namespace ScriptedEvents.Variables.Misc
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Miscellaneous";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Cuffer(),
            new Storage(),
            new Log(),
        };
    }

    public class Log : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{LOG}";

        /// <inheritdoc/>
        public string Description => "Shows the name of the variable with its value. Useful for quick debugging.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variable", typeof(IConditionVariable), "The variable.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IConditionVariable variable = (IConditionVariable)Arguments[0];

                return $"{variable.Name.Trim('{', '}')} = {variable.String()}";
            }
        }
    }

    public class Cuffer : IPlayerVariable, IArgumentVariable, IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CUFFER}";

        /// <inheritdoc/>
        public string Description => "Gets the player who has cuffed a specified player, if specified player is not cuffed or the specified player was not cuffed by a different player, value will be an empty player variable.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("player", typeof(Player), "The player which is cuffed.", true),
        };

        public float Value => Players.Count();

        public IEnumerable<Player> Players
        {
            get
            {
                var plr = ((Player)Arguments[0]).Cuffer;
                return plr is null ? Enumerable.Empty<Player>() : new[] { plr };
            }
        }
    }

    public class Storage : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{STORAGE}";

        /// <inheritdoc/>
        public string Description => "Retrives a variable from storage.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The variable name to retrive.", true),
        };

        /// <inheritdoc/>
        public string Value => VariableStorage.Read(RawArguments[0]);
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
