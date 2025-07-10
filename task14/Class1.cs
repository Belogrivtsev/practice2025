using System;
using System.Threading;
using System.Linq;

namespace task14;

public class DefiniteIntegral
{
    public static object _lock = new object();

    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0) throw new ArgumentException("Threads number must be > 0");
        if (a >= b) throw new ArgumentException("Invalid interval: a must be less than b");

        double total = 0.0;
        double intervalLength = (b - a) / threadsNumber;
        using var barrier = new Barrier(threadsNumber + 1);

        var threads = Enumerable.Range(0, threadsNumber).Select(i => {double start = a + i * intervalLength;
        double end = (i == threadsNumber - 1) ? b : start + intervalLength;
        return new Thread(() => 
            {
                double partialSum = CalculateIntegral(start, end, function, step);
                lock (_lock) total += partialSum; 
                barrier.SignalAndWait();
            });}).ToArray();

        Array.ForEach(threads, t => t.Start());
        barrier.SignalAndWait();

        return total;
    }

    public static double CalculateIntegral(double a, double b, Func<double, double> function, double step)
    {
        int steps = (int)Math.Ceiling((b - a) / step);
        return Enumerable.Range(0, steps).Select(k => a + k * step).Aggregate(0.0, (sum, x) => 
            {
                double xNext = Math.Min(x + step, b);
                return sum + (function(x) + function(xNext)) * (xNext - x) / 2;
            });
    }
}