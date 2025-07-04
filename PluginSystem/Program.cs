using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.IO;
namespace PluginSystem;
public class PluginLoadAttribute : Attribute
{
    public string[] Dependencies { get; }
    public PluginLoadAttribute(params string[] dependencies) { Dependencies = dependencies; }
}
public class PluginSystem
{
    public static void Run()
    {
        var plugins = Directory.EnumerateFiles("Plugins", "*.dll").Select(Assembly.LoadFrom).SelectMany(t => t.GetExportedTypes())
        .Where(t => t.GetCustomAttribute<PluginLoadAttribute>() != null).ToList();

        if (!plugins.Any())
        {
            Console.WriteLine("Plugins not found");
            return;
        }

        plugins.ForEach(t =>
        {
            var instance = Activator.CreateInstance(t);
            var execute = t.GetMethod("Execute");
            execute?.Invoke(instance, null);
        });
    }
}
public class App
{
    public static void Main(string[] args)
    {
        PluginSystem.Run();
    }
}
