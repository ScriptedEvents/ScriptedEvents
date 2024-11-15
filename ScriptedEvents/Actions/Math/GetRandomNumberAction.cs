using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Math
{
    public class GetRandomNumberAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetRandomNumber";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Math;

        /// <inheritdoc/>
        public string Description => "Returns a random number from provided range.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("type", true,
                new OptionValueDepending("Int", "A random integer.", typeof(int)),
                new OptionValueDepending("Float", "A random float.", typeof(float))),
            new Argument("startNumber", typeof(string), "The starting number of the random range.", true),
            new Argument("endNumber", typeof(string), "The ending number of the random range.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            float result;
            string mode = Arguments[0]!.ToUpper();
            switch (mode)
            {
                case "INT":
                    int r1I;
                    int r2I;
                    try
                    {
                        r1I = Convert.ToInt32(Arguments[1]);
                    }
                    catch
                    {
                        throw new InvalidCastException();
                    }

                    try
                    {
                        r2I = Convert.ToInt32(Arguments[2]) + 1;
                    }
                    catch
                    {
                        throw new InvalidCastException();
                    }

                    result = UnityEngine.Random.Range(r1I, r2I);
                    break;

                case "FLOAT":
                    float r1F;
                    float r2F;

                    try
                    {
                        r1F = Convert.ToSingle(Arguments[1]);
                    }
                    catch
                    {
                        throw new InvalidCastException();
                    }

                    try
                    {
                        r2F = Convert.ToSingle(Arguments[2]);
                    }
                    catch
                    {
                        throw new InvalidCastException();
                    }

                    result = UnityEngine.Random.Range(r1F, r2F);
                    break;

                default:
                    throw new ImpossibleException();
            }

            return new(true, new(result.ToString()));
        }
    }
}