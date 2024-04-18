namespace ScriptedEvents.Structures
{
    public class OptionsArgument : Argument
    {
        public OptionsArgument(string argumentName, bool required, params Option[] options)
            : base(argumentName, typeof(string), $"The {argumentName} to use.", required)
        {
            Options = options;
        }

        public Option[] Options { get; }
    }
}
