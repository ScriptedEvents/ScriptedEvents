namespace ScriptedEvents.API.Modules
{
    public interface IModule
    {
        public string Name { get; }

        public bool ShouldGenerateFiles { get; }

        public void Init();

        public void Kill();

        public void GenerateFiles();
    }
}
