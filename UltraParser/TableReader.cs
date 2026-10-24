using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ExcelDataReader;
using UltraParser.Models;

namespace UltraParser;

public static class TableReader 
{
    public static ITableReaderSourceConfigurator<T> For<T>()
    {
        return new TableReader<T>();
    }
}

public class TableReader<T> : 
    ITableReaderSourceConfigurator<T>,
    ITableReaderCsvConfigurator<T>,
    ITableReaderXlsxConfigurator<T>,
    ITableReaderOnErrorConfigurator<T>
{
    private Stream _stream;
    private string _delimiter = ";";
    private SourceType _sourceType;
    private Encoding _encoding = Encoding.UTF8;
    private bool _withHeader = false;
    private OnErrorStrategy _onErrorStrategy = OnErrorStrategy.Throw;
    private List<IMapConfiguration<T>> _configurations = [];
    
    private readonly Func<T> _createInstance;
    public TableReader()
    {
        Type type =  typeof(T);
        ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);

        if (constructor == null)
            throw new InvalidOperationException();
        
        NewExpression newExpression = Expression.New(constructor);
        _createInstance = Expression.Lambda<Func<T>>(newExpression).Compile();
    }
    #region
    // CSV Options
    public ITableReaderCsvConfigurator<T> FromCsv(string path)
    {
        _stream = File.OpenRead(path);
        
        _sourceType = SourceType.Csv;
        return this;
    }
    public ITableReaderCsvConfigurator<T> FromCsv(Stream stream)
    {
        _stream = stream;
        _sourceType = SourceType.Csv;
        return this;
    }
    ITableReaderCsvConfigurator<T> ITableReaderCsvConfigurator<T>.WithHeader()
    {
        _withHeader = true;
        return this;
    }
    ITableReaderCsvConfigurator<T> ITableReaderCsvConfigurator<T>.Encoding(Encoding encoding)
    {
        _encoding = encoding;
        return this;
    }
    public ITableReaderCsvConfigurator<T> Delimiter(string delimiter)
    {
        _delimiter  = delimiter;
        return this;
    }

    // XLSX Options
    public ITableReaderXlsxConfigurator<T> FromXlsx(string path)
    {
        _sourceType  = SourceType.Xlsx;
        _stream = File.OpenRead(path);
        return this;
    }
    public ITableReaderXlsxConfigurator<T> FromXlsx(Stream stream)
    {
        _stream = stream;
        _sourceType = SourceType.Xlsx;
        return this;
    }
    ITableReaderXlsxConfigurator<T> ITableReaderXlsxConfigurator<T>.WithHeader()
    {
        _withHeader = true;
        return this;
    }
    ITableReaderXlsxConfigurator<T> ITableReaderXlsxConfigurator<T>.Encoding(Encoding encoding)
    {
        _encoding = encoding;
        return this;
    }

    // ERROR Options
    public ITableReaderOnErrorConfigurator<T> OnError()
    {
        return this;
    }
    public ITableReaderConfigurator<T> Stop()
    {
        _onErrorStrategy = OnErrorStrategy.Stop;
        return this;
    }
    public ITableReaderConfigurator<T> Throw()
    {
        _onErrorStrategy = OnErrorStrategy.Throw;
        return this;
    }
    public ITableReaderConfigurator<T> Skip()
    {
        _onErrorStrategy  = OnErrorStrategy.Skip;
        return this;
    }
    #endregion
    // MAP Config
    public ITableReaderMapConfigurator<T, TProperty> Map<TProperty>(Expression<Func<T, TProperty>> selector) 
        where TProperty : IComparable<TProperty>
    {
        MapConfigurator<T, TProperty> configurator = new(selector, this);
        _configurations.Add(configurator);
        return configurator;
    }

    public List<T> ToList()
    {
        if (_stream == null) throw new InvalidOperationException("Поток пуст. Сначала вызовите FromCsv() или FromXlsx()");
        List<T> result = [];

        IEnumerable<string[]> lines;

        switch (_sourceType)
        {
            case SourceType.Csv:
                lines = ReadCsv(); break;
            case SourceType.Xlsx:
                lines = ReadXlsx(); break;
            default: throw new InvalidOperationException("Не выбран исходный тип");
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

    private IEnumerable<string[]> ReadCsv()
    {
        _stream.Seek(0, SeekOrigin.Begin);
        StreamReader reader = new StreamReader(_stream);

        string? str = null;

        while ((str = reader.ReadLine()) != null)
        {
            yield return str.Split(_delimiter);
        }
        reader.Dispose();
        yield break;
    }

    public IEnumerable<string[]> ReadXlsx()
    {
        _stream.Seek(0, SeekOrigin.Begin);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        IExcelDataReader reader = ExcelReaderFactory.CreateReader(_stream);
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            { 
                row[i] = reader.GetValue(i)?.ToString() ?? string.Empty;
            }

            yield return row;
        } 
        reader.Dispose();
        yield break;
    }
}


