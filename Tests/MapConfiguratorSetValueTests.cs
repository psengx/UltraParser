using UltraParser;

namespace Tests;

public class MapConfiguratorSetValueTests
{
    [Fact]
    public void SetValue_ConvertIsNull_ThrowsException()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        Assert.Throws<ArgumentNullException>(() => configurator.SetValue(null, null));
    }

    [Fact]
    public void SetValue_CastedValue_Correct()
    {
        MapConfigurator<Order, string> configurator = new(x => x.Name, new());
        Order obj = new Order();
        string value = "TestValue";
        configurator.SetValue(obj, value);
        
        Assert.Equal("TestValue", obj.Name);
    }
    [Fact]
    public void SetValue_Convert_Correct()
    {
        MapConfigurator<Order, Guid> configurator = new(o => o.Id, new());
        configurator.Convert(x => Guid.Parse(x));
        Order obj = new Order();
        
        Guid id = Guid.NewGuid();
        configurator.SetValue(obj, id.ToString());
        
        Assert.Equal(id, obj.Id);
    }
}