# ScriptedEvents
SCP:SL Exiled plugin to create event "scripts". These scripts can be set up to run once per round, multiple times per round, or by command only.

## For developers
If you're a dev and wanna add your own actions to this, it's rather simple. First of all, add the plugin as reference.

Create a new class, it needs to inherit `ScriptedEvents.API.Actions.IAction`, then implement this interface

Then, in your OnEnabled, add `ScriptedEvents.API.ApiHelper.RegisterActions(GetType());`

You are done