using System.Linq.Expressions;
using UltraParser;

namespace Tests;
public class MapConfiguratorConstructorTests
{
    [Fact]
    public void MapConfigurator_TableReaderIsNull_ThrowsArgumentNullException()
    {
        MapConfigurator<Order, string> configurator;
        
        Assert.Throws<ArgumentNullException>(() => configurator = new(o => o.Name, null));
    }

    [Fact]
    public void MapConfigurator_SelectorIsNull_ThrowsArgumentNullException()
    {
        MapConfigurator<Order, string> configurator;
        Assert.Throws(typeof(ArgumentNullException), () => configurator = new(null, new()));
    }
    
    [Fact]
    public void MapConfigurator_BodyIsNotMember_ThrowsArgumentException()
    {
        Expression<Func<Order, string>> selector = x => x.TestMethod();
        Exception exception = Assert.Throws(typeof(ArgumentException), () => new MapConfigurator<Order, string>(selector, new()));
        Assert.Contains("Выражение должно обращаться к свойству", exception.Message);
    }
    
    [Fact]
    public void MapConfigurator_MemberIsField_ThrowsArgumentException()
    {
        Expression<Func<Order, string>> selector = x => x.TestField;
        Exception exception = Assert.Throws(typeof(ArgumentException), () => new MapConfigurator<Order, string>(selector, new()));
        Assert.Contains("Выражение должно ссылаться именно на свойство, а не на поле", exception.Message);
    }
}