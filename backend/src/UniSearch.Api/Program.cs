using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using UniSearch.Api.Models;
using UniSearch.Api.Repositories;
using UniSearch.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// MongoDB
var mongoUri = builder.Configuration["MONGODB_URI"]
    ?? throw new InvalidOperationException(
        "MONGODB_URI n'est pas défini dans les variables d'environnement.");

var mongoUrl = new MongoUrl(mongoUri);
var databaseName = mongoUrl.DatabaseName
    ?? throw new InvalidOperationException(
        "MONGODB_URI doit contenir le nom de la base de données");

builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoUrl));
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
    serviceProvider
        .GetRequiredService<IMongoClient>()
        .GetDatabase(databaseName));
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services
    .GetRequiredService<UserRepository>()
    .EnsureIndexesAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
