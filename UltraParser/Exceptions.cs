namespace UltraParser;

public class TableReaderColumnNotFoundException : Exception
{
    public override string Message => "Столбец с указанным названием не найден в заголовках файла.";
    public  TableReaderColumnNotFoundException() => Console.WriteLine(Message);
}