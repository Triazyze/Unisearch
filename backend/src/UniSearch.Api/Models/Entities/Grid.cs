using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public class Grid
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string Id { get; set; }

    required public int Size { get; set; }

    required public List<string> Letters { get; set; }

    required public List<string> Words { get; set; }

    required public string Difficulty { get; set; }

    [BsonElement("pack")]
    required public GridPack? Pack { get; set; }
}
