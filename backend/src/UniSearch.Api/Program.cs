using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using UniSearch.Api.Models.Entities;
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

// JWT
var jwtSecret = builder.Configuration["JWT_SECRET"]
    ?? throw new InvalidOperationException(
        "JWT_SECRET n'est pas défini dans les variables d'environnement.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuer n'est pas configuré.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audience n'est pas configuré.");

if (jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT_SECRET doit contenir au moins 32 caractères.");
}

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
    });

builder.Services.AddSingleton(
    new JwtService(jwtSecret, jwtIssuer, jwtAudience));

builder.Services.AddAuthorization();

// Configure MongoDB et les services associés
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoUrl));
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
    serviceProvider
        .GetRequiredService<IMongoClient>()
        .GetDatabase(databaseName));
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services
    .GetRequiredService<UserRepository>()
    .EnsureIndexesAsync();

using (var scope = app.Services.CreateScope())
{
    var databaseSeeder = scope.ServiceProvider
        .GetRequiredService<DatabaseSeeder>();

    await databaseSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
