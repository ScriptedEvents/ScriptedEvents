namespace ScriptedEvents
{
    using Exiled.API.Interfaces;

    public class Translations : ITranslation
    {
        public string MissingPermission { get; set; } = "Missing permission: {PERMISSION}";

        public string CommandSuccess { get; set; } = "Successfully ran {SUCCESSAMOUNT} scripts.";

        public string CommandSuccessWithFailure { get; set; } = "Successfully ran {SUCCESSAMOUNT} scripts. Failed to run {FAILAMOUNT} scripts: {FAILED}";

        public string CommandCooldown { get; set; } = "This command is on cooldown and can be used in {SECONDS} seconds.";

        public string CommandCooldownSingular { get; set; } = "This command is on cooldown and can be used in {SECONDS} second.";

        public string DisabledScript { get; set; } = "This script is disabled.";

        public string MissingScript { get; set; } = "This script could not be found.";
    }
}
