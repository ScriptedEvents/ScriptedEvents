![VERSION](https://img.shields.io/github/v/release/Thundermaker300/ScriptedEvents?include_prereleases&logo=gitbook&style=for-the-badge)
![DOWNLOADS](https://img.shields.io/github/downloads/Thundermaker300/ScriptedEvents/total?logo=github&style=for-the-badge)
[![DISCORD](https://img.shields.io/discord/1060274824330620979?label=Discord&logo=discord&style=for-the-badge)](https://discord.gg/3j54zBnbbD)


# ScriptedEvents
SCP:SL Exiled plugin to create event "scripts". These scripts can be set up to run once per round, multiple times per round, or by command only.

## Getting Started
Fair warning: This plugin is very complex and has a lot of features. However, once you understand it, the capabilities are close to endless. My best suggestion is to play around with the plugin, as that's the easiest way to learn it. Tips to get started include:
* Read the documents that are generated when you first install the plugin and restart the server. Your console will tell you where they are located the first time (typically directly inside the Configs folder).
* The `shelp` server console command is going to help you out a ton! This command will generate documentation and open it in a `.txt` file. Type `shelp LIST` in your server console to generate a list of actions.
  * Note to Pterodactyl users: Pterodactyl does not like opening files on demand and will generally throw a permission error. It will still generate the file inside your `Configs/ScriptedEvents` folder, it will just not open it. As such, it is encouraged to use a local server for using this command.
* The parent remote-admin command for this plugin is `scriptedevents` (aliases: `script`, `scr`). Running this command will show examples on how to use it.

### Code Validation
The wonderful @Elektryk-Andrzej has provided a Visual Studio Code extension to validate scripts in real-time! If you are using VS Code to write your scripts, you can add [this extension](https://marketplace.visualstudio.com/items?itemName=ElektrykAndrzej.e-secas) to validate your scripts while writing them!

### Permissions
* `script.action` - Run a single non-logic action via command.
* `script.execute` - Execute a script.
* `script.list` - View all scripts.
* `script.read` - Read a script.
* `script.stopall` - Stop all running scripts.

### Default Config
```yml
scripted_events:
# Whether or not to enable the Scripted Events plugin.
  is_enabled: true
  debug: false
  # Enable logs for starting/stopping scripts.
  enable_logs: true
  # If a script encounters an error, broadcast a notice to the person who ran the command, informing of the error. The broadcast ONLY shows to the command executor.
  broadcast_issues: true
  # If set to true, players with overwatch enabled will not be affected by any commands related to players.
  ignore_overwatch: true
  # The string to use for countdowns.
  countdown_string: '<size=26><color=#5EB3FF><b>{TEXT}</b></color></size>\n{TIME}'
  # The maximum amount of actions that can run in one second, before the script is force-stopped. Increasing this value allows for more actions to occur at the same time, but increases the risk of the server crashing (or restarting due to missed heartbeats). This maximum can be bypassed entirely by including the "!-- NOSAFETY" flag in a script.
  max_actions_per_second: 25
  # Define a custom set of permissions used to run a certain script. The provided permission will be added AFTER script.execute (eg. script.execute.examplepermission for the provided example).
  required_permissions:
    ExampleScriptNameHere: examplepermission
  # [ADVANCED] Define scripts to execute when certain events occur.
  on: {}
  # [ADVANCED] Define a custom command to run a script when it is executed.
  commands: []
```

## For developers
There are two methods for adding your own actions to ScriptedEvents in your plugin.

### Directly Referencing Plugin
This method works by adding a reference to the plugin.

Create a new class, it needs to inherit `ScriptedEvents.API.Actions.IAction`, then implement this interface.

Then, in your OnEnabled, add `ScriptedEvents.API.ApiHelper.RegisterActions();`

The problem with using this method is that your plugin will ONLY function if ScriptedEvents is also installed, which is not ideal servers may use your plugin but not ScriptedEvents.

### Reflection
The alternative to the above method is by using reflection to access the `ApiHelper` class. From there, call the `RegisterCustomAction(string, Func<string[], Tuple<bool, string>>)` method.

The above method takes a string, the name of the plugin, and it takes a defined function. This function gives you a `string[]`, representing the arguments that were given from the script. It must return a `Tuple<bool, string>`, with the bool representing whether or not execution was successful, and the message to show. If it is NOT successful, a message should be provided. If it is successful, a message is optional (should be set to `string.Empty`).

If your plugin is disabled in-game, and Scripted Events is still running, this may cause a problem if a script uses your action. As such, it is recommended to call the `ApiHelper.UnregisterCustomAction(string name)` method if your action is no longer usable.

For ease of debugging, both `RegisterCustomAction` and `UnregisterCustomAction` return a string message representing whether or not they were successful.

This method is much more recommended, as your plugin does not need to have Scripted Events installed in order for your plugin to function. However, it is not as straight forward as the previous method, and reflection is significantly slower than the previous method (which is why you only need to use it once in your plugin).

To view an example of this method in action, see the [Round Reports](https://github.com/Thundermaker300/RoundReports/blob/master/RoundReports/ScriptedEventsIntegration.cs) implementation of it.

#### Other Reflection API
* `IEnumerable<Player> ApiHelper.GetPlayers(string input, int max = -1)` - Gets a list of players based on input variables, and a maximum amount to select a random maximum. This should be used instead of the classic `Player.Get()` as this method also supports all of Scripted Events' variables (including user-defined variables).
* `Tuple<bool, float> ApiHelper.Math(string input)` - Performs a math calculation using the given string. This method supports all of Scripted Events' variables (including user-defined variables). Returns a success boolean and the result of the equation as a float.
