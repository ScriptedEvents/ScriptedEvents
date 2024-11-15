using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine;

namespace ScriptedEvents.Actions.Math
{
    public class MathRootAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MathRoot";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Math;

        /// <inheritdoc/>
        public string Description => "Returns a root of the input number.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("value", typeof(float), "The number to root.", true),
             new Argument("rootLevel", typeof(float), "The root level. Default: 2", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(Mathf.Pow((float)Arguments[0]!, 1 / ((float?)Arguments[1] ?? 2)).ToUpper()));
        }
    }
}