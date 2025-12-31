using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class WebAppBuilderConfig
{

    public static void ConfigureBuilder(WebApplicationBuilder builder, string connectionString)
    {
        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi("v1"); // Customize the document name
        builder.Services.AddDbContext<AppDbContext>(optionsAction: options => options.UseNpgsql(connectionString));
        builder.Services.AddScoped<BuyService>();
        builder.Services.AddScoped<ClientService>();
        builder.Services.AddScoped<ExpenseService>();
        builder.Services.AddScoped<InventoryService>();
        builder.Services.AddScoped<MovementService>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<SellService>();
        builder.Services.AddScoped<WarehouseService>();
        builder.Services.AddControllers();
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
