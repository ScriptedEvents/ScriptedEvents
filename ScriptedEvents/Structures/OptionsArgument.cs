namespace ScriptedEvents.Structures
{
    using ScriptedEvents.Interfaces;

    public class OptionsArgument : Argument
    {
        public OptionsArgument(string argumentName, bool required, params IOption[] options)
            : base(argumentName, typeof(string), $"The {argumentName} to use.", required)
        {
            Options = options;
        }

        public IOption[] Options { get; }
    }
}
