using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Repositories;
using UniSearch.Api.Services;
using UniSearch.Api.Services.Authentication;
using UniSearch.Api.Services.Grids;

const string ApiRateLimitPolicy = "api";

var builder = WebApplication.CreateBuilder(args);

// Contrôleurs et validation HTTP

builder.Services.AddControllers();

// Uniformise les erreurs de validation automatique des DTOs.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = _ =>
        new BadRequestObjectResult(ApiErrors.BadRequest);
});

// Limite les appels aux endpoints API pour réduire les abus et les tentatives répétées.
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(ApiRateLimitPolicy, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                AutoReplenishment = true,
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = ApiErrors.TooManyRequests.StatusCode;
        await context.HttpContext.Response.WriteAsJsonAsync(
            ApiErrors.TooManyRequests,
            cancellationToken: cancellationToken);
    };
});

// Configuration MongoDB

// Connexion et sélection de la base MongoDB.

var mongoUri = builder.Configuration["MONGODB_URI"]
    ?? throw new InvalidOperationException(
        "MONGODB_URI n'est pas défini dans les variables d'environnement.");

var mongoUrl = new MongoUrl(mongoUri);
var databaseName = mongoUrl.DatabaseName
    ?? throw new InvalidOperationException(
        "MONGODB_URI doit contenir le nom de la base de données");

// Authentification et autorisation JWT

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
        options.MapInboundClaims = false;
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

                NameClaimType = JwtRegisteredClaimNames.Sub,
                RoleClaimType = "role",
            };
    });

builder.Services.AddSingleton(
    new JwtService(jwtSecret, jwtIssuer, jwtAudience));

builder.Services.AddAuthorization();

// Persistance MongoDB, repositories et services

// Connexions aux collections MongoDB et services applicatifs.
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoUrl));
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
    serviceProvider
        .GetRequiredService<IMongoClient>()
        .GetDatabase(databaseName));
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<GridRepository>();
builder.Services.AddSingleton<PlayerResultRepository>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PackService>();
builder.Services.AddScoped<GridVerificationService>();
builder.Services.AddScoped<DatabaseSeeder>();

// Documentation Swagger / OpenAPI

// Configure Swagger et OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Saisissez uniquement le jeton JWT, sans le préfixe Bearer.",
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
    });
});

var app = builder.Build();

// Initialisation de la base de données

await app.Services
    .GetRequiredService<UserRepository>()
    .EnsureIndexesAsync();
await app.Services
    .GetRequiredService<PlayerResultRepository>()
    .EnsureIndexesAsync();

using (var scope = app.Services.CreateScope())
{
    var databaseSeeder = scope.ServiceProvider
        .GetRequiredService<DatabaseSeeder>();

    await databaseSeeder.SeedAsync();
}

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStatusCodePages();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting(ApiRateLimitPolicy);

app.Run();
