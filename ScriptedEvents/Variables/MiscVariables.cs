namespace ScriptedEvents.Variables.Misc
{
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.Variables.Interfaces;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Miscellaneous";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {

        };
    }
}
