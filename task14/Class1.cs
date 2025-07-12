using System;
using System.Linq;
using System.Threading;

namespace task14;
public static class DefiniteIntegral
{
    public static object _lock = new object();

    public static double Solve(double a, double b, Func<double, double> f, double step, int threadsNumber)
    {
        return threadsNumber <= 0 ? throw new ArgumentException("threads must be > 0") :
        a >= b ? throw new ArgumentException("a must be less than b") : CalculateIntegralWithThreads(a, b, f, step, threadsNumber);
    }

    public static double CalculateIntegralWithThreads(double a, double b, Func<double, double> f, double step, int threadsNumber)
    {
        double total = 0.0;
        double length = (b - a) / threadsNumber;
        using var barrier = new Barrier(threadsNumber + 1);
        var threads = Enumerable.Range(0, threadsNumber).Select(i => new Thread(() =>
            {
                double start = a + i * length;
                double end = (i == threadsNumber - 1) ? b : start + length;
                double sum = CalculateIntegral(f, start, end, step);
                lock (_lock) total += sum;
                barrier.SignalAndWait();
            })).ToArray();
        Array.ForEach(threads, t => t.Start());
        barrier.SignalAndWait();
        return total;
    }

    public static double CalculateIntegral(Func<double, double> f, double a, double b, double step)
    {
        int steps = (int)Math.Ceiling((b - a) / step);
        return Enumerable.Range(0, steps).Select(k =>
        {
            double x = a + k * step; double xNext = Math.Min(x + step, b);
            return (f(x) + f(xNext)) * (xNext - x) / 2;
        }).Sum();
    }
}

