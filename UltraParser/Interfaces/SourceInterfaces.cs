using System.Text;

namespace UltraParser;

public interface ITableReaderSourceConfigurator<T> 
{
    ITableReaderCsvConfigurator<T> FromCsv(string path);
    ITableReaderCsvConfigurator<T> FromCsv(Stream stream);
    ITableReaderXlsxConfigurator<T> FromXlsx(string path);
    ITableReaderXlsxConfigurator<T> FromXlsx(Stream stream);
}

public interface ITableReaderCsvConfigurator<T> 
    : ITableReaderConfigurator<T> 
{
    ITableReaderCsvConfigurator<T> WithHeader();
    ITableReaderCsvConfigurator<T> Encoding(Encoding encoding);
    ITableReaderCsvConfigurator<T> Delimiter(string delimiter);
}
public interface ITableReaderXlsxConfigurator<T> 
    : ITableReaderConfigurator<T> 
{
    ITableReaderXlsxConfigurator<T> WithHeader();
    ITableReaderXlsxConfigurator<T> Encoding(Encoding encoding);
}