#pragma warning disable SA1402 // File may only contain a single type
namespace ScriptedEvents.Variables.Escapes
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.Variables.Interfaces;

    public class EscapesVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Escapes";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Escapes(),
            new ClassDEscapes(),
            new ScientistEscapes(),
        };
    }

    public class Escapes : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{ESCAPES}";

        /// <inheritdoc/>
        public string Description => "The amount of escapes. Equivalent to {CLASSDESCAPES} + {SCIENTISTESCAPES}.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => MainPlugin.Handlers.Escapes[RoleTypeId.ClassD]
            .Union(MainPlugin.Handlers.Escapes[RoleTypeId.Scientist]);
    }

    public class ClassDEscapes : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{CLASSDESCAPES}";

        /// <inheritdoc/>
        public string Description => "The amount of Class-D escapes.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => MainPlugin.Handlers.Escapes[RoleTypeId.ClassD];
    }

    public class ScientistEscapes : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCIENTISTESCAPES}";

        /// <inheritdoc/>
        public string Description => "The amount of Scientist escapes.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => MainPlugin.Handlers.Escapes[RoleTypeId.Scientist];
    }
}
