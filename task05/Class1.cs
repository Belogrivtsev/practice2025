using System;
using System.Collections.Generic;
using System.Reflection;

namespace task05
{
    public class ClassAnalyzer
    {
        private readonly Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type;
        }

        public IEnumerable<string> GetPublicMethods()
        {
            return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).Select(m => m.Name);
        }

        public IEnumerable<string> GetMethodParams(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName)) { return Enumerable.Empty<string>(); }
            var method = _type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (method == null) {return Enumerable.Empty<string>();}
            return method.GetParameters().Select(p => p.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name));
        }

        public IEnumerable<string> GetAllFields()
        {
            return _type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Select(f => f.Name);
        }

        public IEnumerable<string> GetProperties()
        {
            return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return _type.GetCustomAttribute<T>() != null;
        }
    }
}