#pragma warning disable SA1402 // File may only contain a single type
namespace ScriptedEvents.Variables.Escapes
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class EscapesVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Escapes";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Escapes(),
        };
    }

    public class Escapes : IFloatVariable, IPlayerVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{ESCAPES}";

        /// <inheritdoc/>
        public string Description => "Players which have escaped the facility.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new OptionsArgument("mode", true,
                    new("ALL", "Scientists and ClassDs which have escaped."),
                    new("SCIENTISTS", "Scientists which have escaped."),
                    new("CLASSD", "ClassDs which have escaped.")),
        };

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
#pragma warning disable CS8509 // Wyrażenie switch nie obsługuje wszystkich możliwych wartości jego typu danych wejściowych (nie jest kompletne).
                return Arguments[0].ToUpper() switch
                {
                    "ALL" => MainPlugin.Handlers.Escapes[RoleTypeId.ClassD].Union(MainPlugin.Handlers.Escapes[RoleTypeId.Scientist]),
                    "SCIENTISTS" => MainPlugin.Handlers.Escapes[RoleTypeId.Scientist],
                    "CLASSD" => MainPlugin.Handlers.Escapes[RoleTypeId.ClassD],
                };
#pragma warning restore CS8509 // Wyrażenie switch nie obsługuje wszystkich możliwych wartości jego typu danych wejściowych (nie jest kompletne).
            }
        }

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }
    }
}
