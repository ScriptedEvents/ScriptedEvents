﻿using System;
using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class SilentCassieAction : IScriptAction
    {
        public string Name => "SILENTCASSIE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script scr)
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
                Cassie.MessageTranslated(text, text, isNoisy: false);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                    return new(false, "Cannot show captions without a corresponding message.");

                if (string.IsNullOrWhiteSpace(cassieArgs[1]))
                    Cassie.Message(cassieArgs[0], isNoisy: false);
                else
                    Cassie.MessageTranslated(cassieArgs[0], cassieArgs[1], isNoisy: false);
            }

            return new(true, string.Empty);
        }
    }
}
