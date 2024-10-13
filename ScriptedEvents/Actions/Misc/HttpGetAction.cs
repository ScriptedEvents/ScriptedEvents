namespace ScriptedEvents.Actions.MiscActions
{
    using System;
    using System.Collections.Generic;

    using MEC;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using UnityEngine.Networking;

    public class HttpGetAction : IHelpInfo, ITimingAction, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "HTTPGET";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Sends an HTTP GET request to a website.";

        /// <inheritdoc/>
        public string LongDescription => @"This action pauses the script until the request is complete. Upon completion, three variables will be created:
- $HTTPSUCCESS - True or false, depending on if the website returned HTTP Code 200.
- $HTTPCODE - The status code response.
- $HTTPRESULT - The body of information returned from the website.

These variables are created as per-script variables, meaning they can only be used in the script that created them. If these variables already exist (such as a prior HTTPGET/HTTPPOST execution), they will be overwritten with new values.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("url", typeof(string), "The URL to send a GET request to.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            string coroutineKey = $"HTTPGET_COROUTINE_{DateTime.Now.Ticks}";
            CoroutineHandle handle = Timing.RunCoroutine(InternalSendHTTP(script, Parser.ReplaceContaminatedValueSyntax(RawArguments[0], script)), coroutineKey);
            CoroutineHelper.AddCoroutine("HTTPGET", handle, script);
            message = new(true);
            return Timing.WaitUntilDone(handle);
        }

        private IEnumerator<float> InternalSendHTTP(Script script, string input)
        {
            UnityWebRequest discordWWW = UnityWebRequest.Get(input);
            yield return Timing.WaitUntilDone(discordWWW.SendWebRequest());

            string result;
            if (discordWWW.responseCode != 200)
                result = $"ERROR {discordWWW.responseCode}";
            else
                result = discordWWW.downloadHandler.text;

            script.AddLiteralVariable("HTTPCODE", discordWWW.responseCode.ToString(), true);
            script.AddLiteralVariable("HTTPSUCCESS", (discordWWW.responseCode == 200).ToString().ToUpper(), true);
            script.AddLiteralVariable("HTTPRESULT", result, true);
        }
    }
}