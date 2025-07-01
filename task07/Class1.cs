namespace task07;
using System;
using System.Reflection;
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}
[DisplayNameAttribute("Пример класса")]
[VersionAttribute(1, 0)]
public class SampleClass
{
    [DisplayNameAttribute("Числовое свойство")]
    public int Number { get; }
    [DisplayNameAttribute("Тестовый метод")]
    public void TestMethod() {}
}
public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var classname = type.GetCustomAttribute<DisplayNameAttribute>();
        if (classname != null) { Console.WriteLine($"Class displayName: {classname.DisplayName}"); }

        var version = type.GetCustomAttribute<VersionAttribute>();
        if (version != null) { Console.WriteLine($"Class version: {version.Major}.{version.Minor}"); }

        Console.WriteLine("Methods of the class:\n");
        Array.ForEach(type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), 
        m => {var method = m.GetCustomAttribute<DisplayNameAttribute>();
        if (method != null) Console.WriteLine($"{m.Name}, description: {method.DisplayName}"); });

        Console.WriteLine("Properities:\n");
        Array.ForEach(type.GetProperties(), p => {var property = p.GetCustomAttribute<DisplayNameAttribute>();
        if (property != null) Console.WriteLine($"{p.Name}, description: {property.DisplayName}"); } );
    }

}
