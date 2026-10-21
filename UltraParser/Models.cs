namespace UltraParser.Models;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt  { get; set; }
    public string Theme { get; set; }
    public string Content { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int Views { get; set; }
    public ArticleStatus Status { get; set; }
    public List<string> Tags { get; set; }

}

public enum ArticleStatus
{
    Draft,
    Published,
    Archived
}

public enum OnConflictStrategy
{
    None,
    Skip,
    Update,
    UpdateWithTransform,
    Fail
}

public enum SourceType
{
    Csv,
    Xlsx
}

public enum OnErrorStrategy
{
    Stop,
    Skip,
    Throw
}