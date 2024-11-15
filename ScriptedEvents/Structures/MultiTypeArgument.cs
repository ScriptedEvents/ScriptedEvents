namespace ScriptedEvents.Structures
{
    using System;

    public class MultiTypeArgument : Argument
    {
        public MultiTypeArgument(string argumentName, Type[] allowedTypes, string description, bool required)
            : base(argumentName, null, description, required)
        {
            AllowedTypes = allowedTypes;
        }

        public Type[] AllowedTypes { get; }
    }
}