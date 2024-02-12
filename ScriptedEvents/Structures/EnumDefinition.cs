namespace ScriptedEvents.Structures
{
    using System;

    public class EnumDefinition
    {
        public EnumDefinition(Type type, string desc, string longDesc = "")
        {
            EnumType = type;
            Description = desc;
            LongDescription = longDesc;
        }

        public Type EnumType { get; }

        public string Description { get; }

        public string LongDescription { get; }

        public Array ObjectItems => Enum.GetValues(EnumType);
    }

    public class EnumDefinition<T> : EnumDefinition
    {
        public EnumDefinition(string desc, string longDesc = "")
            : base(typeof(T), desc, longDesc)
        {
        }
    }
}
