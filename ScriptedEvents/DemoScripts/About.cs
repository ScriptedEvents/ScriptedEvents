namespace ScriptedEvents.DemoScripts
{
    using System;

    public class About : IDemoScript
    {
        public string FileName => "README";

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
# Each action that takes a 'duration' parameter supports math, which means you can use math equations such as '5 * 5'.
# Each action that supports math also supports variables, eg. '5 * {{PLAYERSALIVE}}' to multiply 5 by the amount of players alive.
# All variables are listed on the plugin's wiki page.
# Lastly, have fun! This is meant to be a fun plugin to mess around with. If you have any questions, do not hesitate to join the Discord server and ask! I'm here to help you!
# - Thunder

# DemoScripts Generated at: {DateTime.UtcNow:f}
# Plugin Version (as of generation): {MainPlugin.Singleton.Version}";
    }
}
