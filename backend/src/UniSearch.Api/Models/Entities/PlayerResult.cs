using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public class PlayerResult
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    required public string UserId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    required public string GridId { get; set; }

    [BsonElement("time")]
    required public int Time { get; set; }

    [BsonElement("traceAttempts")]
    required public int TraceAttempts { get; set; }
}
