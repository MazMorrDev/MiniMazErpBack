using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MiniMazErpBack;

public class WebAppBuilderConfig
{

    public static void ConfigureBuilder(WebApplicationBuilder builder, string connectionString)
    {
        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi("v1"); // Customize the document name
        builder.Services.AddDbContext<AppDbContext>(optionsAction: options => options.UseNpgsql(connectionString));
        builder.Services.AddScoped<IBuyService, BuyService>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();
        builder.Services.AddScoped<IMovementService, MovementService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ISellService, SellService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        // 1. Agregar servicios de autenticación
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,         // Que expire: TRUE
                    ValidateIssuerSigningKey = true, // Que esté firmado: TRUE

                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MiniMazErpBack",
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MiniMazErpFront",

                    // Usa la MISMA clave que en GenerateJwtToken
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                           Environment.GetEnvironmentVariable("JWT_KEY") ?? "fallback_key_32_chars_long_123456"
                        )
                    )
                };
            });
    }

    public static void ConfigureCorsPolicy(WebApplicationBuilder builder)
    {
        // Configure CORS to local development
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }
}
