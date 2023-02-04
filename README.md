![VERSION](https://img.shields.io/github/v/release/Thundermaker300/ScriptedEvents?include_prereleases&style=for-the-badge)
![DOWNLOADS](https://img.shields.io/github/downloads/Thundermaker300/ScriptedEvents/total?style=for-the-badge)
[![DISCORD](https://img.shields.io/discord/1060274824330620979?label=Discord&style=for-the-badge)](https://discord.gg/3j54zBnbbD)


# ScriptedEvents
SCP:SL Exiled plugin to create event "scripts". These scripts can be set up to run once per round, multiple times per round, or by command only.

## For server hosts
All documentation for this plugin can be found on its [wiki](https://github.com/Thundermaker300/ScriptedEvents/wiki). The wiki contains code samples and information about every action, variable, and supported conditions.

### Permissions
* `script.execute` - Execute a script.
* `script.list` - View all scripts.
* `script.stopall` - Stop all running scripts.

## For developers
If you're a dev and wanna add your own actions to this, it's rather simple. First of all, add the plugin as reference.

Create a new class, it needs to inherit `ScriptedEvents.API.Actions.IAction`, then implement this interface

Then, in your OnEnabled, add `ScriptedEvents.API.ApiHelper.RegisterActions(GetType());`

You are done
