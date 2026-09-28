using MongoDB.Driver;
using UniSearch.Api.Models.Entities;

namespace UniSearch.Api.Repositories;

public sealed class PackRepository
{
    private readonly IMongoCollection<Pack> _packs;

    public PackRepository(IMongoDatabase database)
    {
        _packs = database.GetCollection<Pack>("packs");
    }

    public async Task<Pack?> FindByIdAsync(string id)
    {
        var filter = Builders<Pack>.Filter.Eq(pack => pack.Id, id);
        return await _packs.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Pack pack)
    {
        await _packs.InsertOneAsync(pack);
    }
}
