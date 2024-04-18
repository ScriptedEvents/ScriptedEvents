namespace ScriptedEvents.Structures
{
    public class OptionsArgument : Argument
    {
        public OptionsArgument(string argumentName, string description, bool required, params string[] options)
            : base(argumentName, typeof(string), description, required)
        {
            Options = options;
        }

        public string[] Options { get; }
    }
}
