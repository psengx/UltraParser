namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        decimal a = 15;
        decimal b = 0;
        Assert.Throws<DivideByZeroException>(() => Devide(a, b));
    }

    [Theory]
    [InlineData(100, 10, 10)]
    
    [InlineData(15, 3, 5)]
    [InlineData(150, 25, 6)]
    public void Test2(int a, int b, int c)
    {
        decimal result = Devide(a, b);
        Assert.Equal(c, result);
    }

    public decimal Devide(decimal a, decimal b)
    {
        if (b == 0)
            throw new DivideByZeroException();
        return a / b;
    }
}