﻿namespace ScriptedEvents.Structures
{
    using System;

    using ScriptedEvents.Enums;

    public class ArgValuePredicate
    {
        public ArgValuePredicate(Type argType, bool optional = false)
        {
            ArgType = argType;
            Optional = optional;
        }

        public ArgValuePredicate(Type argType, ArgPredicate predicate)
        {
            ArgType = argType;
        }

        public Type ArgType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether predicate is set to optional.
        /// If true, the argument processor will not raise an error if the type fails to parse and will parse it to string instead.
        /// </summary>
        public bool Optional { get; set; } = false;

        /// <summary>
        /// Gets or sets mode.
        /// </summary>
        public ArgPredicate Mode { get; set; } = ArgPredicate.None;
    }
}