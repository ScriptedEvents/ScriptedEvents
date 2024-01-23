namespace ScriptedEvents.Variables.Chance
{
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.Variables.Interfaces;
    using System;
    using UnityEngine;

    public class ChanceVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Chances";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Chance(),
            new Chance3(),
            new Chance5(),
            new Chance10(),
            new Chance20(),
            new Chance100(),
            new Rand(),
        };
    }

    public class Chance : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE}";

        /// <inheritdoc/>
        public string Description => "Always returns a random decimal between 0-1.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.value;
    }

    public class Chance3 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE3}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-3.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 4);
    }

    public class Chance5 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE5}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-5.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 6);
    }

    public class Chance10 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE10}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-10.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 11);
    }

    public class Chance20 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE20}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-20.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 21);
    }

    public class Chance100 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE100}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-100.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 101);
    }

    public class Rand : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{RAND}";

        /// <inheritdoc/>
        public string Description => "Returns a random number from provided range. If range is not provided, will return a random number from 0 to 1.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new Argument("number", typeof(int), "A starting number of the random range. If an ending number is not provided, it will be treated as end number, and 0 as start number.", false),
                new Argument("number", typeof(int), "An ending number of the random range.", false),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Arguments.Length == 0) return UnityEngine.Random.value;

                if (!VariableSystem.TryParse(Arguments[0], out float startNum))
                    throw new ArgumentException(ErrorGen.Get(137, Arguments[0]));

                float endNum;

                if (Arguments.Length >= 2)
                {
                    if (!VariableSystem.TryParse(Arguments[1], out endNum))
                        throw new ArgumentException(ErrorGen.Get(137, Arguments[1]));
                }
                else
                {
                    endNum = startNum;
                    startNum = 1;
                }

                return UnityEngine.Random.Range(startNum, endNum);
            }
        }
    }
}
