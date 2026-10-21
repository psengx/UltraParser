namespace UltraParser;

public interface ITableReaderOnErrorConfigurator<T>
{
    ITableReaderConfigurator<T> Stop();
    ITableReaderConfigurator<T> Throw();
    ITableReaderConfigurator<T> Skip();
}

public interface ITableReaderOnConflictConfigurator<T, TProperty> 
    where  TProperty : IComparable<TProperty>
{
    ITableReaderOnConflictConfigurator<T, TProperty> SkipExiting();
    ITableReaderOnConflictConfigurator<T, TProperty> UpdateExiting();
    ITableReaderOnConflictConfigurator<T, TProperty> UpdateExiting(Func<T, T, T> transform);
    ITableReaderOnConflictConfigurator<T, TProperty> Fail();
}