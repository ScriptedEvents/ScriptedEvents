namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;
    using UnityEngine.Networking;

    public class HttpPostAction : IHelpInfo, ITimingAction
    {
        public static List<string> Coroutines { get; } = new();

        /// <inheritdoc/>
        public string Name => "HTTPPOST";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Sends an HTTP POST request to a website.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("url", typeof(string), "The URL to send a POST request to.", true),
            new Argument("body", typeof(string), "The body to send (JSON formatting). Variables are supported.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                return null;
            }

            string body = string.Join(" ", Arguments.Skip(1));
            body = VariableSystem.ReplaceVariables(body, script);

            string coroutineKey = $"HTTPPOST_COROUTINE_{DateTime.UtcNow.Ticks}";
            Coroutines.Add(coroutineKey);
            message = new(true);
            return Timing.WaitUntilDone(InternalSendHTTP(script, Arguments[0], body), coroutineKey);
        }

        private IEnumerator<float> InternalSendHTTP(Script script, string input, string body)
        {
            UnityWebRequest discordWWW = UnityWebRequest.Put(input, body);
            discordWWW.method = "POST";
            yield return Timing.WaitUntilDone(discordWWW.SendWebRequest());

            string result;
            if (discordWWW.responseCode != 200)
                result = $"ERROR {discordWWW.responseCode}";
            else
                result = discordWWW.downloadHandler.text;

            script.AddVariable("{HTTPSUCCESS}", "Whether or not the result of an HTTP request was successful.", (discordWWW.responseCode == 200).ToString().ToUpper());
            script.AddVariable("{HTTPRESULT}", "The result of a request through the HTTPGET or HTTPPOST actions.", result);
        }
    }
}