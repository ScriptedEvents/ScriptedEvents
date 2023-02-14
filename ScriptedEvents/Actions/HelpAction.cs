﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Condition;
    using ScriptedEvents.Variables.Handlers;
    using ScriptedEvents.Variables.Interfaces;

    public class HelpAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HELP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Gets information about a command.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[] { new Argument("action", typeof(string), "The name of the action. Case-sensitive.", true) };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            // List Help
            if (Arguments[0].ToUpper() == "LIST")
            {
                StringBuilder sbList = StringBuilderPool.Pool.Get();
                sbList.AppendLine();
                sbList.AppendLine($"List of all actions.");

                foreach (KeyValuePair<string, Type> kvp in ScriptHelper.ActionTypes)
                {
                    IAction lAction = Activator.CreateInstance(kvp.Value) as IAction;
                    IHelpInfo lhelpInfo = lAction as IHelpInfo;

                    if (lAction is IHiddenAction)
                        continue;

                    sbList.AppendLine($"{lAction.Name} : {lhelpInfo?.Description ?? "No Description"}");
                }

                Log.Info(StringBuilderPool.Pool.ToStringReturn(sbList));

                return new(true);
            }

            // Action Help
            else if (ScriptHelper.ActionTypes.TryGetValue(Arguments[0], out Type type))
            {
                IAction action = Activator.CreateInstance(type) as IAction;

                if (action is not IHelpInfo helpInfo)
                    return new(false, "The command provided is not supported in the HELP action.");

                StringBuilder sb = StringBuilderPool.Pool.Get();

                if (helpInfo.ExpectedArguments.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendLine($"{action.Name}: {helpInfo.Description}");

                if (helpInfo.ExpectedArguments.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.Append("Arguments:");
                }

                foreach (Argument arg in helpInfo.ExpectedArguments)
                {
                    string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                    sb.AppendLine();
                    sb.AppendLine($"{chars[0]}{arg.ArgumentName}{chars[1]}");
                    sb.AppendLine($"  Required: {(arg.Required ? "YES" : "NO")}");
                    sb.AppendLine($"  Type: {arg.TypeString}");
                    sb.AppendLine($"  {arg.Description}");
                }

                Log.Info(StringBuilderPool.Pool.ToStringReturn(sb));
                return new(true);
            }

            // Variable help
            else if (Arguments[0].StartsWith("{") && Arguments[0].EndsWith("}"))
            {
                bool valid = false;

                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine();

                if (ConditionVariables.TryGetVariable(Arguments[0], out IConditionVariable variable, out bool reversed))
                {
                    valid = true;
                    sb.AppendLine("=== CONDITION VARIABLE ===");
                    sb.AppendLine($"Name: {variable.Name}");
                    sb.AppendLine($"Description: {variable.Description}");

                    switch (variable)
                    {
                        case IBoolVariable @bool:
                            bool value = reversed ? !@bool.Value : @bool.Value;
                            sb.AppendLine($"Current Value: {(value ? "TRUE" : "FALSE")}");
                            break;
                        case IFloatVariable @float:
                            sb.AppendLine($"Current Value: {@float.Value}");
                            break;
                        case IStringVariable @string:
                            sb.AppendLine($"Current Value: {@string.Value}");
                            break;
                        case CustomVariable custom:
                            sb.AppendLine($"Current Value: {custom.Value}");
                            break;
                    }

                    sb.AppendLine();
                }

                if (PlayerVariables.TryGetVariable(Arguments[0], out IPlayerVariable playerVariable))
                {
                    valid = true;
                    sb.AppendLine($"=== PLAYER VARIABLE ===");
                    sb.AppendLine($"Name: {playerVariable.Name}");
                    sb.AppendLine($"Description: {playerVariable.Description}");
                    sb.AppendLine($"Current Value: {(playerVariable.Players.Count() == 0 ? "[None]" : string.Join(", ", playerVariable.Players.Select(ply => ply.Nickname)))}");
                }

                if (!valid)
                {
                    return new(false, "Invalid variable provided for the HELP action.");
                }

                return new(true, StringBuilderPool.Pool.ToStringReturn(sb));
            }

            // Nope
            return new(false, "Invalid argument provided for the HELP action.");
        }
    }
}
