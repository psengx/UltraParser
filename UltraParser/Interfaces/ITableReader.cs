using System.Linq.Expressions;
namespace UltraParser;

public interface ITableReaderConfigurator<T>
{
    ITableReaderOnErrorConfigurator<T> OnError();
    ITableReaderMapConfigurator<T, TProperty> Map<TProperty>(Expression<Func<T, TProperty>> selector)
        where TProperty : IComparable<TProperty>;
    List<T> ToList();
}

public interface ITableReaderMapConfigurator<T, TProperty> 
    : ITableReaderConfigurator<T> 
    where  TProperty : IComparable<TProperty>
{
    ITableReaderMapConfigurator<T, TProperty> FromColumn(string columnName);
    ITableReaderMapConfigurator<T, TProperty> FromColumn(int index);
    ITableReaderMapConfigurator<T, TProperty> Required();
    ITableReaderMapConfigurator<T, TProperty> Validate(Func<TProperty, T, bool> condition, string errorMessage);
    ITableReaderMapConfigurator<T, TProperty> Validate(Func<TProperty, bool> condition, string errorMessage);
    ITableReaderMapConfigurator<T, TProperty> Convert(Func<string, TProperty> convert);
    ITableReaderOnConflictConfigurator<T, TProperty> OnConflict();
}