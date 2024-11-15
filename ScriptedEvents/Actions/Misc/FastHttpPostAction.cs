using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using UnityEngine.Networking;

namespace ScriptedEvents.Actions.Misc
{
    public class FastHttpPostAction : IHelpInfo, IScriptAction
    {
        /// <inheritdoc/>
        public string Name => "FastHTTPPost";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Works the same as HTTPPost, but does not create variables and does not yield the script execution until the request is finished.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("url", typeof(string), "The URL to send a POST request to.", true),
            new Argument("body", typeof(string), "The body to send (JSON formatting).", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string body = Arguments.JoinMessage(1);

            UnityWebRequest discordWWW = UnityWebRequest.Put((string)Arguments[0]!, body);
            discordWWW.method = "POST";
            discordWWW.SetRequestHeader("Content-Type", "application/json");
            discordWWW.SendWebRequest();
            return new(true);
        }
    }
}