namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;

    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class BindAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "BIND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Changes a name of a local variable. If a variable in question is global, a local copy is created with the given name.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("newVariableName", typeof(string), "The name of the new variable.", true),
            new Argument("oldVariable", typeof(IConditionVariable), "The value of the variable.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string newVarName = RawArguments[0];
            IConditionVariable oldVar = (IConditionVariable)Arguments[1];

            if (oldVar is IPlayerVariable oldPlayerVar)
            {
                if (script.UniquePlayerVariables.ContainsKey(oldPlayerVar.Name))
                {
                    script.UniquePlayerVariables.Remove(oldPlayerVar.Name);
                }

                script.AddPlayerVariable(newVarName, oldPlayerVar.Description, oldPlayerVar.Players.ToList());
            }
            else
            {
                if (script.UniqueVariables.ContainsKey(oldVar.Name))
                {
                    script.UniqueVariables.Remove(oldVar.Name);
                }

                script.AddVariable(newVarName, oldVar.Description, oldVar.String());
            }

            return new(true);
        }
    }
}