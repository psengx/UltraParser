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
    public void TableReaderToList_Success()
    {
        var csvContent = "Name;Id\nPotato;25\nTomato;30";
        using var stream = CreateStream(csvContent);
        var reader = new TableReader<Order>();
        
        List<Order> result = reader.FromCsv(stream).WithHeader().Delimiter(";")
            .OnError().Skip().Map(x => x.Name).FromColumn(1).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
//     [Fact]
//     public void TableReaderToList_StreamIsEmpty_ThrowInvalidOperationException()
//     {
//         Assert.Throws<InvalidOperationException>(() => new TableReader<Order>());
//     }
}

/*
 * public List<T> ToList()
    {
        List<T> result = [];

        IEnumerable<string[]> lines;

        switch (_sourceType)
        {
            case SourceType.Csv:
                lines = ReadCsv(); break;
            case SourceType.Xlsx:
                throw new NotImplementedException();
            default: throw new Exception();
        }
        string[] header = [];
        if (_withHeader)
        {
            header = lines.First();
            lines = lines.Skip(1);
        }

        foreach (string[] line in lines)
        {
            T obj = Activator.CreateInstance<T>();
            List<string> errors = [];
            foreach (IMapConfiguration<T> configuration in _configurations)
            {
                int index = configuration.GetColumnIndex(header);
                if (configuration.IsRequired() && string.IsNullOrWhiteSpace(line[index]))
                {
                    errors.Add("Required value is empty");
                    continue;
                }
                configuration.SetValue(obj, line[index]);
            }

            List<Action> updates = [];
            foreach (IMapConfiguration<T> configuration in _configurations)
            {
                errors.AddRange(configuration.Validate(obj));
            }
            foreach (IMapConfiguration<T> configuration in _configurations)
            {
                errors.AddRange(configuration.Validate(obj));

                if (configuration.OnConflictStrategy != OnConflictStrategy.None)
                {
                    int index = result.FindIndex(item => configuration.IsFieldEquals(item, obj));
                    if(index >= 0)
                    {
                        switch (configuration.OnConflictStrategy)
                        {
                            case OnConflictStrategy.Skip: continue;
                            case OnConflictStrategy.Fail: errors.Add("Conflict happened"); break;
                            case OnConflictStrategy.Update: result[index] = obj; continue;
                            case OnConflictStrategy.UpdateWithTransform: updates.Add(() => configuration.Transform(result[index], obj)); continue;
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                switch (_onErrorStrategy)
                {
                    case OnErrorStrategy.Skip: continue;
                    case OnErrorStrategy.Throw: throw new Exception();
                    case OnErrorStrategy.Stop: return result;
                }
            }
            
            result.Add(obj);
        }
        return result;
    }
 */

public class InvalidTestModel
{
    public InvalidTestModel(string data) { }
}