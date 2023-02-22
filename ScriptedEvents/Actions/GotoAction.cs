namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class GotoAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GOTO";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Moves to the provided line.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (ADD, do not provide for specific line)", false),
            new Argument("line", typeof(int), "The line to move to. Variables & Math are NOT supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            switch (Arguments.ElementAtOrDefault(0).ToUpper())
            {
                case "ADD":
                    if (Arguments.Length < 2) return new(false, "ADD mode requires a line amount to add.");
                    if (int.TryParse(Arguments.ElementAtOrDefault(1), out int newline))
                    {
                        script.Jump(script.CurrentLine + newline);
                        return new(true);
                    }

                    break;
                default:
                    if (int.TryParse(Arguments.ElementAtOrDefault(0), out newline))
                    {
                        script.Jump(newline);
                        return new(true);
                    }

                    if (script.Labels.TryGetValue(Arguments.ElementAtOrDefault(0), out newline))
                    {
                        script.Jump(newline);
                        return new(true);
                    }

                    break;
            }

            return new(false, "Invalid line or label provided.");
        }
    }
}
