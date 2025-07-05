namespace task11;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
public static class ClassGenerator
{
    public static dynamic CreateCalculator()
    {
        string Calculator = @"
        using System;
        public class Calculator
        {
            public int Add(int a, int b) => a + b;
            public int Minus(int a, int b) => a - b;
            public int Mul(int a, int b) => a * b;
            public int Div(int a, int b) => a / b;
        }
        new Calculator()
        ";
        var code = ScriptOptions.Default.WithReferences(typeof(object).Assembly,typeof(
        Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly).WithImports("System");

        return Task.Run(async () => await CSharpScript.EvaluateAsync<dynamic>(
        Calculator, code)).GetAwaiter().GetResult();
        
    }
}
