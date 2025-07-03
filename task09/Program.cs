using System;
using System.Reflection;
using CommandLib;
using System.Linq;
namespace task09;

public class App
{
    public static void Main(string[] args)
    {
        if (args.Length < 1) { Console.WriteLine("Input error"); return; }
        var assembly = Assembly.LoadFrom(args[0]);
        WriteAssemblyMetadata(assembly);
    }
    public static void WriteAssemblyMetadata(Assembly assembly)
    {
         assembly.GetTypes().Where(t => t.IsClass).ToList().ForEach(type => 
                {
                    PrintTypeMetadata(type);
                    Console.WriteLine(new string('=', 50));
                });
    }
    public static void PrintTypeMetadata(Type type)
    {
        Console.WriteLine($"Class: {type.Name}");
        PrintAttributes(type);
        Console.WriteLine("\nConstructors:");
        type.GetConstructors().ToList().ForEach(ctor => 
            {
                Console.WriteLine($"{type.Name}({string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
                PrintAttributes(ctor);
            });

        Console.WriteLine("\nMethods:");
        type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).ToList().ForEach(method => 
            {
                Console.WriteLine($"{method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
                PrintAttributes(method);
            });

        Console.WriteLine("\nProperties:");
        type.GetProperties().ToList().ForEach(prop => 
            {
                Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name} {{ {(prop.CanRead ? "get;" : "")} {(prop.CanWrite ? "set;" : "")} }}");
                PrintAttributes(prop);
            });
    }

    static void PrintAttributes(MemberInfo member)
    {
        var attributes = member.GetCustomAttributesData();
        if (!attributes.Any()) return;

        Console.WriteLine("Attributes");
        Array.ForEach(attributes.ToArray(), attr => 
        {
        Console.Write($"{attr.AttributeType.Name}");
        if (attr.ConstructorArguments.Count > 0)
        {
            Console.Write(string.Join(", ", attr.ConstructorArguments.Select(a => a.Value)));
        }
        });
    }
}