﻿namespace ScriptedEvents.Variables.Condition.Chance
{
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;
    using UnityEngine;

    public class ChanceVariables : IVariableGroup
    {
        public VariableGroupType GroupType => VariableGroupType.Condition;

        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Chance(),
            new Chance3(),
            new Chance5(),
            new Chance10(),
            new Chance20(),
            new Chance100(),
        };
    }

    public class Chance : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE}";

        /// <inheritdoc/>
        public string Description => "Always returns a random decimal between 0-1.";

        /// <inheritdoc/>
        public float Value => Random.value;
    }

    public class Chance3 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE3}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-3.";

        /// <inheritdoc/>
        public float Value => Random.Range(1, 4);
    }

    public class Chance5 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE5}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-5.";

        /// <inheritdoc/>
        public float Value => Random.Range(1, 6);
    }

    public class Chance10 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE10}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-10.";

        /// <inheritdoc/>
        public float Value => Random.Range(1, 11);
    }

    public class Chance20 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE20}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-20.";

        /// <inheritdoc/>
        public float Value => Random.Range(1, 21);
    }

    public class Chance100 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE100}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-100.";

        /// <inheritdoc/>
        public float Value => Random.Range(1, 101);
    }
}
