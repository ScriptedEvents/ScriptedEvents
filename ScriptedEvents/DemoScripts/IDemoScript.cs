namespace ScriptedEvents.DemoScripts
{
    public interface IDemoScript
    {
        public string FileName { get; }

        public string Contents { get; }
    }
}
