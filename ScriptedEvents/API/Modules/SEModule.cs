namespace ScriptedEvents.API.Modules
{
    using Exiled.API.Features;

    public class SEModule
    {
        public virtual string Name => "Unknown SE Module";

        public virtual bool ShouldGenerateFiles => false;

        public bool IsActive { get; private set; }

        public virtual void Init()
        {
            Log.Info($"Initializing SE module '{Name}'");
            IsActive = true;
        }

        public virtual void Kill()
        {
            Log.Info($"Terminating SE module '{Name}'");
            IsActive = false;
        }

        public virtual void GenerateFiles() =>
            Log.Info($"Generating files for SE module '{Name}'");
    }
}
