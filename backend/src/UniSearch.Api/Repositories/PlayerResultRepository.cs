using MongoDB.Driver;
using UniSearch.Api.Models.Entities;

namespace UniSearch.Api.Repositories;

public sealed class PlayerResultRepository
{
    private readonly IMongoCollection<PlayerResult> _playerResults;

    public PlayerResultRepository(IMongoDatabase database)
    {
        _playerResults = database.GetCollection<PlayerResult>("playerResults");
    }

    // Garantit qu'un joueur ne possède qu'un résultat par grille.
    public async Task EnsureIndexesAsync()
    {
        var indexes = new[]
        {
            new CreateIndexModel<PlayerResult>(
                Builders<PlayerResult>.IndexKeys
                .Ascending(result => result.UserId)
                .Ascending(result => result.GridId),
                new CreateIndexOptions { Name = "ux_player_results_user_grid", Unique = true }),
        };

        await _playerResults.Indexes.CreateManyAsync(indexes);
    }

    public async Task<PlayerResult?> FindByUserIdAndGridIdAsync(string userId, string gridId)
    {
        var filter = Builders<PlayerResult>.Filter.And(
            Builders<PlayerResult>.Filter.Eq(playerResult => playerResult.UserId, userId),
            Builders<PlayerResult>.Filter.Eq(playerResult => playerResult.GridId, gridId));
        return await _playerResults.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(PlayerResult playerResult)
    {
        await _playerResults.InsertOneAsync(playerResult);
    }

    public async Task ReplaceAsync(PlayerResult playerResult)
    {
        var filter = Builders<PlayerResult>.Filter.Eq(pr => pr.Id, playerResult.Id);
        await _playerResults.ReplaceOneAsync(filter, playerResult);
    }
}
