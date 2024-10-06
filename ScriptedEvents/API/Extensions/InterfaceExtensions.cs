namespace ScriptedEvents.API.Extensions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Variables.Interfaces;

    public static class InterfaceExtensions
    {
        public static string String(this IVariable variable, Script? source = null)
        {
            try
            {
                switch (variable)
                {
                    case IPlayerVariable player:
                        return variable.Name;
                    case ILiteralVariable @string:
                        return @string.Value;
                    default: // Shouldn't be possible
                        throw new InvalidCastException($"{variable.Name} tried to cast to string, which resulted in an error.");
                }
            }
            catch (InvalidCastException e)
            {
                Logger.Warn(source?.IsDebug == true ? e.ToString() : e.Message, source);
            }
            catch (Exception e)
            {
                Logger.Warn(source?.IsDebug == true ? e.ToString() : e.Message, source);
                return source?.IsDebug == true ? e.ToString() : e.Message;
            }

            return "ERROR";
        }

        /// <summary>
        /// Determines if an action is obsolete.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="message">A message regarding the obsolete.</param>
        /// <returns>Whether or not the action is obsolete.</returns>
        public static bool IsObsolete(this IAction action, out string message)
        {
            Type t = action.GetType();
            var obsolete = t.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "ObsoleteAttribute");
            if (obsolete is not null)
            {
                message = obsolete.ConstructorArguments[0].Value.ToString();
                return true;
            }

            message = string.Empty;
            return false;
        }
    }
}
