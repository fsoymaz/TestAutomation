using CalculatorLib;

namespace CalculatorTests;

public class CalculatorTests
{
    private readonly Calculator _calculator;

    public CalculatorTests()
    {
        _calculator = new Calculator();
    }

    [Fact]
    public void Add_ShouldReturnSum_WhenTwoPositiveNumbers()
    {
        // Arrange
        int a = 5;
        int b = 3;
        int expected = 8;

        // Act
        int result = _calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ShouldReturnSum_WhenNegativeAndPositiveNumbers()
    {
        // Arrange
        int a = -5;
        int b = 3;
        int expected = -2;

        // Act
        int result = _calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ShouldReturnZero_WhenBothNumbersAreZero()
    {
        // Arrange
        int a = 0;
        int b = 0;
        int expected = 0;

        // Act
        int result = _calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }
}

