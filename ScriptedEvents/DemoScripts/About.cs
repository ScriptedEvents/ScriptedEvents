namespace ScriptedEvents.DemoScripts
{
    using System;

    using ScriptedEvents.API.Constants;

    /// <summary>
    /// Demo script providing information to the server host.
    /// </summary>
    public class About : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "README";

        /// <inheritdoc/>
        public string Contents => @$"
## 
- {MainPlugin.Singleton!.Author}

GitHub: {MainPlugin.GitHub}
Discord Server: {MainPlugin.DiscordUrl}

Scripted Events Contributors:
{Constants.GenerateContributorList('-')}
##";
    }
}
