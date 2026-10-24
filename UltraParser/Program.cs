using UltraParser.Models;

namespace UltraParser;

class Program
{
    static void Main(string[] args)
    {
        // List<Article> articles = TableReader.For<Article>()
        //     .FromCsv("test.csv")
        //     .WithHeader()
        //     .Delimiter(",")
        //     .Map(x => x.Id)
        //         .FromColumn("Id")
        //         .Convert(x => Guid.Parse(x))
        //     .Map(x => x.Title)
        //         .FromColumn("Title")
        //     .Map(x => x.CreatedAt)
        //         .FromColumn("CreatedAt")
        //         .Convert(x => DateTime.Parse(x))
        //     .ToList();

        List<Article> articles = TableReader.For<Article>()
            .FromXlsx("book.xlsx")
            .WithHeader()
            .Map(x => x.Title)
                .FromColumn("Title")
            .Map(x => x.Likes)
                .FromColumn("Likes")
                .Convert(x => int.Parse(x))
            .Map(x => x.CreatedAt)
                .FromColumn("CreatedAt")
                .Convert(x => DateTime.Parse(x))
            .ToList();

        foreach (Article article in articles)
        {
            Console.WriteLine(article.CreatedAt + " " + article.Title + " " + article.Likes);
        }
    }
}
