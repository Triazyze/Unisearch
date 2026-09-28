using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    required public string Username { get; set; }

    required public string Email { get; set; }

    required public string PasswordHash { get; set; }

    [BsonRepresentation(BsonType.String)]
    required public Role Role { get; set; }
}
