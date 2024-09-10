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
             new Argument("variable", typeof(float), "The variable to round. Requires the variable to be a number.", true),
             new OptionsArgument("mode", false, new("UP"), new("DOWN"), new("NEAREST", "Default option")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            float value = (float)Arguments[0];
            string result = "big fucky wucky";

            switch (Arguments.Length < 2 ? "NEAREST" : Arguments[1].ToUpper())
            {
                case "UP":
                    result = Mathf.Ceil(value).ToString(); break;

                case "DOWN":
                    result = Mathf.Floor(value).ToString(); break;

                case "NEAREST":
                    result = Mathf.Round(value).ToString(); break;
            }

            return new(true, variablesToRet: new[] { (object)result });
        }
    }
}