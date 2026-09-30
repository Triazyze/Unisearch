using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniSearch.Api.Models.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("username")]
    required public string Username { get; set; }

    [BsonElement("email")]
    required public string Email { get; set; }

    [BsonElement("passwordHash")]
    required public string PasswordHash { get; set; }

    [BsonElement("role")]
    [BsonRepresentation(BsonType.String)]
    required public Role Role { get; set; }
}
