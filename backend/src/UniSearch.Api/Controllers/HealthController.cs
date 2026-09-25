using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UniSearch.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IMongoClient _mongoClient;

    public HealthController(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            await _mongoClient
                .GetDatabase("admin")
                .RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1));

            return Ok(new
            {
                status = "healthy",
            });
        }
        catch (MongoException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable);
        }
    }
}
