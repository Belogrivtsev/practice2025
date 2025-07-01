namespace CommandRunner;

using System;
using CommandLib;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;

class App
{
    public static void Main(string[] args)
    {
        if (args.Length < 1) { Console.WriteLine("Input error"); }

        string dllPath = args[0];
        Assembly assembly = Assembly.LoadFrom(dllPath);
        var types = assembly.GetTypes;

        var commands = assembly.GetTypes().Where(t => !t.IsInterface && typeof(ICommand).IsAssignableFrom(t)).ToArray();
        Array.ForEach(commands, t =>
        {
            Console.WriteLine($"Find the command: {t.Name}"); ICommand command = UsingTheCommand(t);
            command.Execute();
        });
    }
    public static ICommand UsingTheCommand(Type command)
    {
        var constructors = command.GetConstructors();
        var constructor = constructors.FirstOrDefault();
        
        if (constructor == null) {  return (ICommand)Activator.CreateInstance(command);}

        var parameters = constructor.GetParameters();
        var args = parameters.Select((p, index) =>
        {
            Console.Write($"Enter: {p.Name}: ");
            return Convert.ChangeType(Console.ReadLine(), p.ParameterType);

        }).ToArray();
        return (ICommand)constructor.Invoke(args);
    }
}