using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public class Grid
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string Id { get; set; }

    [BsonElement("size")]
    required public int Size { get; set; }

    [BsonElement("letters")]
    required public List<string> Letters { get; set; }

    [BsonElement("words")]
    required public List<string> Words { get; set; }

    [BsonElement("difficulty")]
    required public string Difficulty { get; set; }

    [BsonElement("updatedAt")]
    required public DateTime UpdatedAt { get; set; }

    [BsonElement("pack")]
    required public PackDetails? Pack { get; set; }

    public sealed class PackDetails
    {
        [BsonElement("name")]
        required public string Name { get; set; } = string.Empty;

        [BsonElement("date")]
        required public DateTime? Date { get; set; }
    }
}
