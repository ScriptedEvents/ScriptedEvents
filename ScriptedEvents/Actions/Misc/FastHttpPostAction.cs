namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    using UnityEngine.Networking;

    public class FastHttpPostAction : IHelpInfo, IScriptAction
    {
        /// <inheritdoc/>
        public string Name => "FASTHTTPPOST";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Works the same as HTTPPOST, but does not create variables and does not wait until the request is finished.";

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
            body = VariableSystemV2.ReplaceVariables(body, script);
            UnityWebRequest discordWWW = UnityWebRequest.Put(VariableSystemV2.ReplaceVariable(RawArguments[0], script), body);
            discordWWW.method = "POST";
            discordWWW.SetRequestHeader("Content-Type", "application/json");
            discordWWW.SendWebRequest();
            return new(true);
        }
    }
}