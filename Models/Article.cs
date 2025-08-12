using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonalBloggingPlatformAPI.Models;

public class Article
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;}=null!;
    
    [BsonElement("title")]
    public string Title {get; set;}= null!;
    
    [BsonElement("content")]
    public string Content { get; set; } = null!;
    
    [BsonElement("like")]
    public int Like { get; set; } = 0;

    [BsonElement("publishedAt")]
    public DateTime PublishedAt { get; set; }

    [BsonElement("tags")]
    public List<string>? Tags { get; set; }
}




