namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;

    using ScriptedEvents.Structures;

    /// <summary>
    /// Module that controls storing and retreiving objects based on their string representation.
    /// </summary>
    public class ObjectReferenceModule : SEModule
    {
        private readonly Dictionary<string, object> _objectReferences = new();

        public override string Name => "ObjectReferenceModule";

        public override void Kill()
        {
            base.Kill();
            _objectReferences.Clear();
        }

        public string ToReference(object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));

            var ident = $"[; {obj.GetType().Name} object {Guid.NewGuid():N} ;]";
            _objectReferences[ident] = obj;

            return ident;
        }

        public bool TryGetReference<T>(string identifier, out T? result, out ErrorInfo? errorInfo)
        {
            result = default;
            if (!_objectReferences.TryGetValue(identifier, out object value))
            {
                errorInfo = new ErrorInfo(
                    "Invalid object identifier",
                    $"Provided identifier '{identifier}' does not represent a valid object reference.",
                    "TryGetReference method");
                return false;
            }

            if (value.GetType() != typeof(T))
            {
                errorInfo = new ErrorInfo(
                    "Invalid type for object reference",
                    $"Fetched object from '{identifier}' has a '{value.GetType().Name}', but expected '{typeof(T).Name}'.",
                    "TryGetReference method");
                return false;
            }

            result = (T)value;
            errorInfo = null;
            return true;
        }
    }
}
