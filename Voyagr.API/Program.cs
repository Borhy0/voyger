using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Voyagr.Application.Interfaces;
using Voyagr.Application.Services;
using Voyagr.Infrastructure.Data;
using Voyagr.Infrastructure.Repositories;
using Voyagr.Infrastructure.Services;
using System.Text.Json.Serialization;
using Voyagr.API.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;
using Voyagr.Application.DTOS.Common;
using Voyagr.Infrastructure.Data.Configurations;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args
});
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));


// Rebuild JSON configuration without file watching.
// This is important for restricted container environments
// such as free hosting instances with low inotify limits.
var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: false
    )
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: false
    )
    .AddEnvironmentVariables();

var configuration = configurationBuilder.Build();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors
                    .Select(e =>
                        string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "The value is invalid."
                            : e.ErrorMessage)
                    .ToArray()
            );

        var response = new ApiErrorResponse
        {
            Message = "Validation failed.",
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

// Database
var connectionString =
    configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is missing."
    );

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Dependency Injection
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();
//builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenService,RefreshTokenService>();
builder.Services.AddScoped<IFavoriteCurrencyRepository,FavoriteCurrencyRepository>();
builder.Services.AddScoped<IFavoriteCurrencyService,FavoriteCurrencyService>();
builder.Services.AddScoped<ITripRepository,TripRepository>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IImageStorageService,CloudinaryImageStorageService>();
builder.Services.AddMemoryCache();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// JWT
var jwtSettings = configuration.GetSection("Jwt");

var jwtKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException(
        "JWT Key is missing."
    );

var jwtIssuer = jwtSettings["Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer is missing."
    );

var jwtAudience = jwtSettings["Audience"]
    ?? throw new InvalidOperationException(
        "JWT Audience is missing."
    );

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    )
            };
    });
builder.Services.AddHttpClient();
//builder.Services.AddAuthorization();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(dbContext);
}

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS is handled by Render's proxy.
// Do not redirect inside the container.

// Authentication MUST come before Authorization
app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();