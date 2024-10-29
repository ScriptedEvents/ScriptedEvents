namespace ScriptedEvents.Structures
{
    using ScriptedEvents.Interfaces;

    public class SuggestedOptionsArgument : OptionsArgument
    {
        public SuggestedOptionsArgument(string argumentName, bool required, params IOption[] options)
            : base(argumentName, required, options)
        {
        }
    }
}