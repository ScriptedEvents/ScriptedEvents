namespace ScriptedEvents.Variables.Chance
{
    using System;

    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
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
            new Rand(),
        };
    }

    public class Rand : IFloatVariable, IArgumentVariable, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "{RANDOM}";

        /// <inheritdoc/>
        public string Description => "Returns a random number from provided range.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
                new OptionsArgument("type", true,
                    new("INT", "Will return an integer."),
                    new("FLOAT", "Will return a decimal (floating point) number.")),
                new Argument("startNumber", typeof(string), "A starting number of the random range.", true),
                new Argument("endNumber", typeof(string), "An ending number of the random range.", true),
        };

        public string LongDescription => @"The return value will be a random number from the provided range, depending on the numbers and the type.

If 'type' is set to 'INT':
> act PRINT My integer is {RANDOM:INT:1:100}
> My integer is 60

If 'type' is set to 'FLOAT':
> act PRINT My float is {RANDOM:FLOAT:0:1}
> My float is 0.35227";

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                float result;
                string mode = Arguments[0].ToUpper();
                switch (mode)
                {
                    case "INT":
                        int r1i;
                        int r2i;
                        try
                        {
                            r1i = Convert.ToInt32(Arguments[1]);
                        }
                        catch
                        {
                            throw new InvalidCastException(ErrorGen.Get(API.Enums.ErrorCode.InvalidNumber, Arguments[1]));
                        }

                        try
                        {
                            r2i = Convert.ToInt32(Arguments[2]) + 1;
                        }
                        catch
                        {
                            throw new InvalidCastException(ErrorGen.Get(API.Enums.ErrorCode.InvalidNumber, Arguments[2]));
                        }

                        result = UnityEngine.Random.Range(r1i, r2i);
                        break;

                    case "FLOAT":
                        float r1f;
                        float r2f;

                        try
                        {
                            r1f = Convert.ToSingle(Arguments[1]);
                        }
                        catch
                        {
                            throw new InvalidCastException(ErrorGen.Get(API.Enums.ErrorCode.InvalidNumber, Arguments[1]));
                        }

                        try
                        {
                            r2f = Convert.ToSingle(Arguments[2]);
                        }
                        catch
                        {
                            throw new InvalidCastException(ErrorGen.Get(API.Enums.ErrorCode.InvalidNumber, Arguments[2]));
                        }

                        result = UnityEngine.Random.Range(r1f, r2f);
                        break;

                    default:
                        throw new Exception("[4562] error - you shouldnt ever see this error");
                }

                return result;
            }
        }
    }
}
