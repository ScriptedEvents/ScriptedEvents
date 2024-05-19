namespace ScriptedEvents.API.Extensions
{
    using System;
    using System.Linq;

    using Exiled.API.Features;

    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Variables.Interfaces;

    public static class InterfaceExtensions
    {
        public static string String(this IVariable variable, Script source = null, bool reversed = false)
        {
            try
            {
                switch (variable)
                {
                    case IBoolVariable @bool:
                        bool result = reversed ? !@bool.Value : @bool.Value;
                        return result.ToUpper();
                    case IFloatVariable @float:
                        return @float.Value.ToString();
                    case ILongVariable @long:
                        return @long.Value.ToString();
                    case IStringVariable @string:
                        return @string.Value;
                    default: // Shouldn't be possible
                        throw new System.InvalidCastException($"{variable.Name} tried to cast to string, which resulted in an error.");
                }
            }
            catch (InvalidCastException e)
            {
                Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {(source?.Debug == true ? e : e.Message)}");
            }
            catch (Exception e)
            {
                Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {(source?.Debug == true ? e : e.Message)}");
                return source?.Debug == true ? e.ToString() : e.Message;
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
