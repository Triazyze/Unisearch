using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public class Pack
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string Id { get; set; }

    required public DateTime Date { get; set; }

    required public List<string> GridIds { get; set; }
}
