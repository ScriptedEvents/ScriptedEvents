using System;
using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class CassieAction : IScriptAction, IHelpInfo
    {
        public string Name => "CASSIE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Makes a loud cassie announcement.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption. Variables are supported.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(false, "Missing text!");

            string text = string.Join(" ", Arguments);

            if (string.IsNullOrWhiteSpace(text))
                return new(false, "Missing text!");

            string[] cassieArgs = text.Split('|');

            for (int i = 0; i < cassieArgs.Length; i++)
            {
                cassieArgs[i] = ConditionVariables.ReplaceVariables(cassieArgs[i]);
            }

            if (cassieArgs.Length == 1)
            {
                text = ConditionVariables.ReplaceVariables(text);
                Cassie.MessageTranslated(text, text);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                    return new(false, "Cannot show captions without a corresponding message.");

                if (string.IsNullOrWhiteSpace(cassieArgs[1]))
                    Cassie.Message(cassieArgs[0]);
                else
                    Cassie.MessageTranslated(cassieArgs[0], cassieArgs[1]);
            }

            return new(true, string.Empty);
        }
    }
}
