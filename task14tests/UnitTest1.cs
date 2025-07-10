using Xunit;
using task14;

namespace task14tests;

public class DefiniteIntegralTests
{
    [Fact]
    public void DefiniteIntegral_ReturnCorrectValue()
    {
        var X = (double x) => x;

        var SIN = (double x) => Math.Sin(x);

        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 1e-4);

        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 1e-4);

        Assert.Equal(12.5, DefiniteIntegral.Solve(0, 5, X, 1e-6, 8), 1e-5);
    }
    [Fact]
    public void ConstantFunction_ReturnCorrectValue()
    {
    var CONST = (double x) => 3.0;

    Assert.Equal(30, DefiniteIntegral.Solve(0, 10, CONST, 1e-5, 4), precision: 5); 
    }

    [Fact]
    public void QuadraticFunction_ReturnCorrectValue()
    {
    var SQUARE = (double x) => x * x;

    Assert.Equal(9, DefiniteIntegral.Solve(0, 3, SQUARE, 1e-6, 4), precision: 5);
    }
}