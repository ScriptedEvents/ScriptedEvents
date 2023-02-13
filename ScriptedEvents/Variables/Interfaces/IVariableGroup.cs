using ScriptedEvents.API.Enums;

namespace ScriptedEvents.Variables.Interfaces
{
    public interface IVariableGroup
    {
        public VariableGroupType GroupType { get; }

        public IVariable[] Variables { get; }
    }
}
