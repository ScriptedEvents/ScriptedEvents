namespace ScriptedEvents.API.Features.ScriptHelpGenerator
{
    using System.ComponentModel;

    public class GeneratorConfig
    {
        [Description("Whether or not to generate documentation for actions.")]
        public bool generate_actions;

        [Description("Whether or not to generate documentation for variables.")]
        public bool generate_variables;

        [Description("Whether or not to generate documentation for error codes.")]
        public bool generate_error_codes;

        [Description("Whether or not to generate documentation for Exiled/SCP:SL enumerations.")]
        public bool generate_enums;
    }
}
