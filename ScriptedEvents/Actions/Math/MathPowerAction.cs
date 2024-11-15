using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine;

namespace ScriptedEvents.Actions.Math
{
    public class MathPowerAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MathPower";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Math;

        /// <inheritdoc/>
        public string Description => "Returns a exponentiated version of the input number.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("value", typeof(float), "Number to exponentiate.", true),
             new Argument("exponent", typeof(float), "The exponent level.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(Mathf.Pow((float)Arguments[0]!, (float)Arguments[1]!).ToUpper()));
        }
    }
}