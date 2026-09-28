using MongoDB.Driver;
using UniSearch.Api.Models.Entities;

namespace UniSearch.Api.Repositories;

public sealed class GridRepository
{
    private readonly IMongoCollection<Grid> _grids;

    public GridRepository(IMongoDatabase database)
    {
        _grids = database.GetCollection<Grid>("grids");
    }

    public async Task<Grid?> FindByIdAsync(string id)
    {
        var filter = Builders<Grid>.Filter.Eq(grid => grid.Id, id);
        return await _grids.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Grid grid)
    {
        await _grids.InsertOneAsync(grid);
    }
}
