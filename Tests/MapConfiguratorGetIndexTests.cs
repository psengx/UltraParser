using UltraParser;

namespace Tests;

public class MapConfiguratorGetIndexTests
{
    [Fact]
    public void GetColumnIndex_IndexIsNegative_ThrowsException()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        Assert.Throws<ArgumentOutOfRangeException>(() => configurator.GetColumnIndex());
    }
    [Fact]
    public void GetColumnIndexTest_IndexExists_ReturnsCorrectIndex()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        configurator.FromColumn(2);
        int index = configurator.GetColumnIndex([]);
        
        Assert.Equal(2, index);
    }

    [Fact]
    public void GetColumnIndexTest_HeadersIsEmpty_ThrowsException()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        
        Assert.Throws<ArgumentException>(() => { configurator.GetColumnIndex([]); });
    }

    [Fact]
    public void GetColumnIndexTest_ColumnNameNotExists_ThrowsException()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        
        Assert.Throws<TableReaderColumnNotFoundException>(() => { configurator.GetColumnIndex(["Id", "Name"]); });
    }
    [Fact]
    public void GetColumnIndexTest_ColumnNameExists_ColumnIndexNotExists_ThrowsException()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        configurator.FromColumn("Price");

        Assert.Throws<TableReaderColumnNotFoundException>(() => { configurator.GetColumnIndex(["Id", "Name"]); });
    }

    [Fact]
    public void GetColumnIndexTest_ColumnNameExists_ColumnIndexExists_CorrectIndex()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        configurator.FromColumn("Id");
        int result = configurator.GetColumnIndex(["Id", "Name"]);
        Assert.Equal(0, result);
    }
}

public class Order
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    
    public string? TestField;
    public string TestMethod() => "test";
}