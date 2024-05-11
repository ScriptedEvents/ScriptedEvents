namespace ScriptedEvents.API.Extensions
{
    using System;

    using Exiled.API.Features;

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
                        throw new InvalidCastException();
                }
            }
            catch (InvalidCastException e)
            {
                Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {(source?.Debug == true ? e : e.Message)}");
            }
            catch (Exception e)
            {
                Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {(source?.Debug == true ? e : e.Message)}");
            }

            return "ERROR";
        }
    }
}
