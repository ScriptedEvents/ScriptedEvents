namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using UnityEngine;

    /// <inheritdoc/>
    public class Math_RoundAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MATH-ROUND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Math;

        /// <inheritdoc/>
        public string Description => "Returns a rounded version of the input number.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("UP"),
                new("DOWN"),
                new("NEAR")),
            new Argument("value", typeof(float), "The number to round.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            float value = (float)Arguments[1];

            string res = Arguments[0].ToUpper() switch
            {
                "UP" => Mathf.Ceil(value).ToString(),
                "DOWN" => Mathf.Floor(value).ToString(),
                "NEAR" => Mathf.Round(value).ToString(),
                _ => throw new ArgumentException(),
            };

            return new(true, variablesToRet: new[] { res });
        }
    }
}