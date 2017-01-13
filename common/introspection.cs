using System;
using System.Collections.Generic;

namespace Ts
{
    public static class Introspection
    {
        public struct TypeAttributePair<T>
        {
            public Type Type;
            public T Attribute;
        }

        public static List<TypeAttributePair<T>> GetAllWithAttribute<T>(bool inherit = true)
            where T : Attribute
        {
            var results = new List<TypeAttributePair<T>>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetExportedTypes()) {
                    object[] attributes = type.GetCustomAttributes(typeof(T), inherit);
                    foreach (object attribute in attributes) {
                        var result = new TypeAttributePair<T>();
                        result.Type = type;
                        result.Attribute = (T)attribute;
                        results.Add(result);
                    }
                }
            }

            return results;
        }
    }
}
