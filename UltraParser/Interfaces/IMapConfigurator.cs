using UltraParser.Models;

namespace UltraParser;

public interface IMapConfiguration<T> 
{
    public void SetValue(T obj, string stringValue);
    public int GetColumnIndex(string[] headers);
    List<string>? Validate(T obj);
    OnConflictStrategy OnConflictStrategy { get; }
    bool IsFieldEquals(T obj1, T obj2);
    T Transform(T obj1, T obj2);
    bool IsRequired();
}