namespace ScriptedEvents.Variables.Chance
{
    using System;

    using ScriptedEvents.Structures;
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.Variables.Interfaces;

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

    [Obsolete("Please use the {RAND} variable.")]
    public class Chance : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE}";

        /// <inheritdoc/>
        public string Description => "Always returns a random decimal between 0-1.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.value;
    }

    [Obsolete("Please use the {RAND} variable.")]
    public class Chance3 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE3}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-3.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 4);
    }

    [Obsolete("Please use the {RAND} variable.")]
    public class Chance5 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE5}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-5.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 6);
    }

    [Obsolete("Please use the {RAND} variable.")]
    public class Chance10 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE10}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-10.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 11);
    }

    [Obsolete("Please use the {RAND} variable.")]
    public class Chance20 : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CHANCE20}";

        /// <inheritdoc/>
        public string Description => "Always returns a random number from 1-20.";

        /// <inheritdoc/>
        public float Value => UnityEngine.Random.Range(1, 21);
    }

    [Obsolete("Please use the {RAND} variable.")]
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
        public string Description => "Returns a random number from provided range. When a range composed of integers only, the return number will be an integer. If the range is composed of at least 1 float, then a float will be returned.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new Argument("startNumber", typeof(object), "A starting number of the random range.", true),
                new Argument("endNumber", typeof(object), "An ending number of the random range.", true),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if ((Arguments[0] is string x0 && x0.Contains(".")) || (Arguments[1] is string x1 && x1.Contains(".")))
                {
                    return UnityEngine.Random.Range(Convert.ToSingle(Arguments[0]), Convert.ToSingle(Arguments[1]));
                }

                return UnityEngine.Random.Range(Convert.ToInt32(Arguments[0]), Convert.ToInt32(Arguments[1]) + 1);
            }
        }
    }
}
