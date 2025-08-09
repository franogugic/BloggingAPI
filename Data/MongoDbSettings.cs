namespace PersonalBloggingPlatformAPI.Models;

// i napravimo klasu za bazu
public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
    public required string ArticlesCollectionName { get; set; }
}