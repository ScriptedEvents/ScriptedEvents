namespace ScriptedEvents.DemoScripts
{
    using System;

    /// <summary>
    /// Demo script providing information to the server host.
    /// </summary>
    public class About : IDemoScript
    {
        /// <inheritdoc/>
        public string FileName => "README";

        /// <inheritdoc/>
        public string Contents => @$"!-- DISABLE
# Hello, and thank you for using my plugin, Scripted Events!
# I'm going to go over a couple of brief things in here you should know about setting up scripts. It's relatively easy!
# First and foremost, the GitHub's wiki is stocked full of information about different things you can do! I strongly encourage using this resource.
# Additionally, this folder will generate on server startup full of already-functioning scripts. You do not have to keep them, and you can delete the ScriptedEvents folder to re-generate them.
# All scripts in this folder can be loaded and executed by running ""script ex [filename]"" in-game (without the .txt extension).
# Example: To run this file, type ""script ex {FileName}"" in the RemoteAdmin panel.
# 'script.execute' permission is required to execute scripts (unless specified otherwise in config).
# Any line starting with a # will be ignored and can be used to comment what you're doing, TODOs, etc.
# Adding ""!-- DISABLE"" at the start of the script will disable its execution in-game. Useful for WIP/broken scripts.
# Almost every action that takes a message or number supports variables, eg. '{{PLAYERSALIVE}}' to get the amount of players alive.

# This plugin contains A LOT of features. You will likely get confused at first. That's okay, that's why documentation (and support) is here.
# The majority of this plugin's features are documented through the ""shelp"" server console command. Simply type ""shelp LIST"" in your server console to get started!
# shelp takes any of the following: ""LIST"", ""LISTVAR"", the name of an action, or the name of a variable with brackets.

# Lastly, have fun! This is meant to be a fun plugin to mess around with, as well as a utility plugin to create what you can imagine without the need for a custom plugin.
# If you have any questions, do not hesitate to join the Discord server and ask! I'm here to help you!
# - Thunder

# GitHub: {MainPlugin.GitHub}
# Discord Server: {MainPlugin.DiscordUrl}

# File Generated at: {DateTime.UtcNow:f}
# Plugin Version (as of generation): {MainPlugin.Singleton.Version}
# Experimental DLL: {(MainPlugin.IsExperimental ? "YES" : "NO")}";
    }
}
