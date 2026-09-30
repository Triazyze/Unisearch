using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public sealed class GridPack
{
    [BsonElement("name")]
    required public string Name { get; set; }

    [BsonElement("date")]
    required public DateTime? Date { get; set; }
}
