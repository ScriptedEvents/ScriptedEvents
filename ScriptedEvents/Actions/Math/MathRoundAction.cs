using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine;

namespace ScriptedEvents.Actions.Math
{
    public class MathRoundAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MathRound";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Math;

        /// <inheritdoc/>
        public string Description => "Returns a rounded version of the input number.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("roundMode", true,
                new Option("Up", "Rounds up to a full integer."),
                new Option("Down", "Rounds down to a full integer."),
                new Option("Near", "Rounds to the nearest integer.")),
            new Argument("value", typeof(float), "The number to round.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            float value = (float)Arguments[1]!;

            string res = Arguments[0]!.ToUpper() switch
            {
                "UP" => Mathf.Ceil(value).ToString(),
                "DOWN" => Mathf.Floor(value).ToString(),
                "NEAR" => Mathf.Round(value).ToString(),
                _ => throw new ArgumentException(),
            };

            return new(true, new(res));
        }
    }
}