using MongoDB.Driver;
using UniSearch.Api.Models;

namespace UniSearch.Api.Repositories;

public sealed class UserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email)
    {
        var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Eq(user => user.Username, username),
            Builders<User>.Filter.Eq(user => user.Email, email));

        return await _users.Find(filter).AnyAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task EnsureIndexesAsync()
    {
        var indexes = new[]
        {
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(user => user.Username),
                new CreateIndexOptions { Name = "ux_users_username", Unique = true }),
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(user => user.Email),
                new CreateIndexOptions { Name = "ux_users_email", Unique = true }),
        };

        await _users.Indexes.CreateManyAsync(indexes);
    }

    public async Task<User?> FindByUsernameOrEmailAsync(string identifier)
    {
        var filter = Builders<User>.Filter.Eq(user => user.Username, identifier) | Builders<User>.Filter.Eq(user => user.Email, identifier);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }
}
