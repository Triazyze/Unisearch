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

    public async Task<List<Grid>> FindAllAsync()
    {
        return await _grids.Find(Builders<Grid>.Filter.Empty).ToListAsync();
    }

    public async Task<List<Grid>> FindUnassignedGridsAsync()
    {
        var filter = Builders<Grid>.Filter.Eq(grid => grid.Pack, null);
        return await _grids.Find(filter).ToListAsync();
    }

    public async Task<List<Grid>> FindByPackNameAsync(string packName)
    {
        var filter = Builders<Grid>.Filter.Eq(grid => grid.Pack!.Name, packName);
        return await _grids.Find(filter).ToListAsync();
    }

    public async Task CreateAsync(Grid grid)
    {
        await _grids.InsertOneAsync(grid);
    }

    public async Task SetPackAsync(string id, Grid.PackDetails? pack)
    {
        var filter = Builders<Grid>.Filter.Eq(grid => grid.Id, id);
        var update = Builders<Grid>.Update
            .Set(grid => grid.Pack, pack);

        await _grids.UpdateOneAsync(filter, update);
    }
}
