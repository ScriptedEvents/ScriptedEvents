using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features.Pools;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using ScriptedEvents.Variables.Interfaces;

namespace ScriptedEvents.DocumentationGeneration
{
    public static class DocsGen
    {
        internal static string GenerateActionList()
        {
            StringBuilder sbList = StringBuilderPool.Pool.Get();
            sbList.AppendLine("List of all actions. You can learn more about any action from this list by doing `shelp ActionName` like e.g. `shelp Print`.");

            var temp = ListPool<IAction>.Pool.Get();
            temp.AddRange(ScriptModule.Singleton!.ActionTypes.Select(kvp => (IAction)Activator.CreateInstance(kvp.Value)));

            var grouped = temp.GroupBy(a => a.Subgroup);

            foreach (var group in grouped.OrderBy(g => g.Key.Display()))
            {
                if (!group.Any() || (group.All(act => act is IHiddenAction || act.IsObsolete(out _)) && !MainPlugin.Configs.Debug))
                    continue;

                sbList.AppendLine();
                sbList.AppendLine($"== {group.Key.Display()} Actions ==");

                foreach (IAction lAction in group)
                {
                    if ((lAction is IHiddenAction && !MainPlugin.Configs.Debug) || lAction.IsObsolete(out _))
                        continue;

                    sbList.AppendLine($"{lAction.Name} : {((IHelpInfo)lAction).Description}");
                }
            }

            ListPool<IAction>.Pool.Return(temp);
            return StringBuilderPool.Pool.ToStringReturn(sbList);
        }

        internal static string GenerateVariableList()
        {
            var conditionList = VariableSystem.Groups.OrderBy(group => group.GroupName);

            StringBuilder sbList = StringBuilderPool.Pool.Get();
            sbList.AppendLine("The following are predefined variables for you to use in any script. These are not actions for your convenience.");
            sbList.AppendLine();

            foreach (IVariableGroup group in conditionList)
            {
                sbList.AppendLine($"+ {group.GroupName} +");
                foreach (IVariable variable in group.Variables.OrderBy(v => v.Name))
                {
                    sbList.AppendLine($"@{variable.Name} - {variable.Description}");
                }

                sbList.AppendLine();
            }

            return StringBuilderPool.Pool.ToStringReturn(sbList);
        }
    }
}