using System;
using System.Collections.Generic;
using MEC;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine.Networking;

namespace ScriptedEvents.Actions.Misc
{
    public class HttpPostAction : IHelpInfo, ITimingAction, ILongDescription, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "HTTPPost";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Sends an HTTP POST request to a website.";

        /// <inheritdoc/>
        public string LongDescription => @"This action can either pause until the request is complete or not. This is handled by the 'mode' argument.

If 'shouldWait' mode is used, three values will be returned upon completion of the request:
- a TRUE/FALSE value, depending on if the website returned HTTP Code 200.
- a number value - The status code response.
- a string value - The body of information returned from the website.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("NoWait", "Action will not wait until the request is finished."),
                new Option("WaitForFinish", "Action will wait until the request is finished and will return values.")),
            new Argument("url", typeof(string), "The URL to send a POST request to.", true),
            new Argument("body", typeof(string), "The body to send (JSON formatting).", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            string body = Arguments.JoinMessage(1);

            string coroutineKey = $"HTTPPOST_COROUTINE_{DateTime.Now.Ticks}";
            CoroutineHandle handle = Timing.RunCoroutine(InternalSendHTTP(script, (string)Arguments[0], body), coroutineKey);
            CoroutineHelper.AddCoroutine("HTTPPOST", handle, script);
            message = new(true);
            return Timing.WaitUntilDone(handle);
        }

        private IEnumerator<float> InternalSendHTTP(Script script, string input, string body)
        {
            UnityWebRequest discordWWW = UnityWebRequest.Put(input, body);
            discordWWW.method = "POST";
            discordWWW.SetRequestHeader("Content-Type", "application/json");
            yield return Timing.WaitUntilDone(discordWWW.SendWebRequest());
            
            string result = discordWWW.responseCode != 200 
                ? $"ERROR {discordWWW.responseCode}" 
                : discordWWW.downloadHandler.text;

            script.AddLiteralVariable("HTTPCODE", discordWWW.responseCode.ToString(), true);
            script.AddLiteralVariable("HTTPSUCCESS", (discordWWW.responseCode == 200).ToString().ToUpper(), true);
            script.AddLiteralVariable("HTTPRESULT", result, true);
        }
    }
}