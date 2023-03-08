namespace ScriptedEvents.Actions.Samples
{
    public class Sample
    {
        public Sample(string title, string description, string code)
        {
            Title = title;
            Description = description;
            Code = code;
        }

        public string Title { get; }

        public string Description { get; }

        public string Code { get; }
    }
}
