using System.Text;
using UltraParser;
namespace Tests;

public class TableReaderConstructorTests
{
    [Fact]
    public void TableReaderConstructor_WithNullConstructor_ThrowInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => new TableReader<InvalidTestModel>());
    }

    [Fact]
    public void TableReaderConstructor_Success()
    {
        Exception? exception = Record.Exception(() => new TableReader<Order>());
        Assert.Null(exception);
    }
}

public class TableReaderToListTests
{
    private MemoryStream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
    [Fact]
    public void TableReaderToList_FromCsv_Success()
    {
        var csvContent = "Name;Id\nPotato;25\nTomato;30";
        using var stream = CreateStream(csvContent);
        var reader = new TableReader<Order>();
        
        List<Order> result = reader.FromCsv(stream).WithHeader().Delimiter(";")
            .OnError().Skip().Map(x => x.Name).FromColumn(1).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void TableReaderToList_StreamIsEmpty_ThrowInvalidOperationException()
    {
        Exception exception = Assert.Throws<InvalidOperationException>(() => new TableReader<Order>().ToList());
        Assert.Equal("Поток пуст. Сначала вызовите FromCsv() или FromXlsx()", exception.Message);
    }
     [Fact]
         public void TableReaderToList_FromXlsx_Success()
         {
             TableReader<Order> reader = new TableReader<Order>();
        
             List<Order> result = reader.FromXlsx("test.xlsx").WithHeader()
                 .OnError().Skip().Map(x => x.Name).FromColumn(1).ToList();

             Assert.NotNull(result);
             Assert.Equal(3, result.Count);
         }
     
}
public class InvalidTestModel
{
    public InvalidTestModel(string data) { }
}