using MongoDB.Driver;

namespace PersonalBloggingPlatformAPI.Models;

public class SortCriteria
{
    public string Field { get; set; }
    public string Direction { get; set; }
}