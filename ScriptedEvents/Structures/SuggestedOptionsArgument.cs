namespace ScriptedEvents.Structures
{
    public class SuggestedOptionsArgument : OptionsArgument
    {
        public SuggestedOptionsArgument(string argumentName, bool required, params Option[] options)
            : base(argumentName, required, options)
        {
        }
    }
}