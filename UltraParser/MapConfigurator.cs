using System.Linq.Expressions;
using System.Reflection;
using UltraParser.Models;

namespace UltraParser;

public class MapConfigurator<T, TProperty> :
    ITableReaderMapConfigurator<T, TProperty>,
    ITableReaderOnConflictConfigurator<T, TProperty>,
    IMapConfiguration<T>
    where TProperty : IComparable<TProperty>
{
    private int _columnIndex = -1;
    private string? _columnName;
    private Action<T, TProperty> _setter;
    private Func<T, TProperty> _getter;
    private bool _required = false;
    private OnConflictStrategy _conflictStrategy = OnConflictStrategy.None;
    private Func<T, T, T>? _updateTransform = null;
    private Func<string, TProperty>? _convert = null;
    private readonly TableReader<T> _tableReader;
    private List<Validator<TProperty, T>> _validators = [];
    public OnConflictStrategy OnConflictStrategy => _conflictStrategy;

    public bool IsFieldEquals(T obj1, T obj2)
    {
        return _getter(obj1).CompareTo(_getter(obj2)) == 0;
    }

    public T Transform(T obj1, T obj2)
    {
        return _updateTransform(obj1, obj2);
    }

    public bool IsRequired() => _required; 

public int GetColumnIndex()
    {
        if (_columnIndex != -1) 
            return _columnIndex;
        throw new Exception();
    }

    public int GetColumnIndex(string[] headers)
    {
        if (_columnIndex != -1) 
            return _columnIndex;
        if (headers.Length == 0)
            throw new Exception();
        if (_columnName != null)
        {
            _columnIndex = Array.IndexOf(headers, _columnName);
            
            if (_columnIndex != -1)
                return _columnIndex;
        }
        throw new Exception();
    }

    public MapConfigurator(Expression<Func<T, TProperty>> selector, TableReader<T>  tableReader)
    {
        if (selector.Body is not MemberExpression memberExpression)
        {
            throw new Exception();
        }
        if (memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new Exception();
        }
        ParameterExpression objectParameter = Expression.Parameter(typeof(T));
        MemberExpression member = Expression.Property(objectParameter, propertyInfo);
        ParameterExpression valueParameter = Expression.Parameter(typeof(TProperty));
        BinaryExpression operation = Expression.Assign(member, valueParameter);
        _setter =  Expression.Lambda<Action<T, TProperty>>(operation, objectParameter, valueParameter)
                             .Compile();

        _getter = Expression.Lambda<Func<T, TProperty>>(member, objectParameter)
            .Compile();
        _tableReader = tableReader;
    }

    #region
    public ITableReaderMapConfigurator<T, TProperty> FromColumn(string columnName) 
    {
        _columnName = columnName;
        return this;
    }

    public ITableReaderMapConfigurator<T, TProperty> FromColumn(int index)
    {
        _columnIndex = index;
        return this;
    }
    
    public ITableReaderMapConfigurator<T, TProperty> Required()
    {
        _required = true;
        return this;
    }

    public ITableReaderMapConfigurator<T, TProperty> Convert(Func<string, TProperty> convert)
    {
        _convert = convert;
        return this;
    }
    
    public ITableReaderOnConflictConfigurator<T, TProperty> OnConflict()
    {
        return this;
    }

    public ITableReaderOnConflictConfigurator<T, TProperty> SkipExiting()
    {
        _conflictStrategy = OnConflictStrategy.Skip;
        return this;
    }

    public ITableReaderOnConflictConfigurator<T, TProperty> UpdateExiting()
    {
        _conflictStrategy = OnConflictStrategy.Update;
        _updateTransform = null;
        return this;
    }

    public ITableReaderOnConflictConfigurator<T, TProperty> UpdateExiting(Func<T, T, T> transform)
    {
        _conflictStrategy = OnConflictStrategy.UpdateWithTransform;
        _updateTransform = transform;
        return this;
    }

    public ITableReaderOnConflictConfigurator<T, TProperty> Fail()
    {
        _conflictStrategy = OnConflictStrategy.Fail;
        return this;
    }
    #endregion
    
    public void SetValue(T obj, string stringValue)
    {
        TProperty value;
        if (stringValue is TProperty castedValue)
            value = castedValue;
        else
        {
            if (_convert == null)
                throw new Exception();
            value = _convert.Invoke(stringValue);
        }
        
        _setter(obj, value);
    }

    public ITableReaderMapConfigurator<T, TNextProperty> Map<TNextProperty>(Expression<Func<T, TNextProperty>> selector) 
    where TNextProperty : IComparable<TNextProperty>
    {
        return _tableReader.Map(selector);
    }
    public ITableReaderOnErrorConfigurator<T> OnError()
    {
        return _tableReader.OnError();
    }
    public List<T> ToList()
    {
        return _tableReader.ToList();
    }
    
    public List<string?> Validate(T obj)
    {
        return _validators.Select(validator => validator.Validate(obj, _getter(obj)))
            .Where(error =>  error != null)
            .ToList(); 
    }

    public ITableReaderMapConfigurator<T, TProperty> Validate(Func<TProperty, T, bool> condition, string errorMessage)
    {
        Validator<TProperty, T> validator = new(condition, errorMessage);
        _validators.Add(validator);
        return this;
    }
    public ITableReaderMapConfigurator<T, TProperty> Validate(Func<TProperty, bool> condition, string errorMessage)
    {
        Func<TProperty, T, bool> validateCondition = (prop, obj) =>  condition(prop);
        Validator<TProperty, T> validator = new(validateCondition, errorMessage);
        _validators.Add(validator);
        return this;
    }
}