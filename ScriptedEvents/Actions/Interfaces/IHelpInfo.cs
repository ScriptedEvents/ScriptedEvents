namespace ScriptedEvents.Actions.Interfaces
{
    using ScriptedEvents.Structures;

    public interface IHelpInfo
    {
        public string Description { get; }
        public Argument[] ExpectedArguments { get; }
    }
}
