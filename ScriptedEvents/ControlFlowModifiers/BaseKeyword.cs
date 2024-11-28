using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;

namespace ScriptedEvents.ControlFlowModifiers
{
    public abstract class BaseKeyword
    {
        private Dictionary<string, Type>? _registeredTypes;
        
        public Dictionary<string, Type> RegisteredTypes => _registeredTypes ?? GetDerivedTypesWithRepresentation();
        
        private Dictionary<string, Type> GetDerivedTypesWithRepresentation()
        {
            var derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(BaseKeyword)) && !type.IsAbstract)
                .ToArray();

            var representationDict = new Dictionary<string, Type>();

            foreach (var type in derivedTypes)
            {
                var instance = Activator.CreateInstance(type);
                var representationProperty = type.GetProperty("Representation");

                if (representationProperty == null || representationProperty.PropertyType != typeof(string)) continue;

                if (representationProperty.GetValue(instance) is string representation)
                {
                    representationDict[representation] = type;
                }
            }

            _registeredTypes = representationDict;
            return _registeredTypes;
        }
    }
}