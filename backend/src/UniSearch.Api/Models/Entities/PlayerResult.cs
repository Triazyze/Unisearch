using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public class PlayerResult
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string Id { get; set; }

    required public string UserId { get; set; }

    required public string GridId { get; set; }

    required public int Time { get; set; }

    required public int TraceAttempts { get; set; }
}
