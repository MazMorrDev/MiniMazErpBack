using DotNetEnv;
using MiniMazErpBack;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebAppBuilderConfig.ConfigureBuilder(builder, Env.GetString("CONNECTION_STRING_POSTGRESQL"));
WebAppBuilderConfig.ConfigureCorsPolicy(builder);

var app = builder.Build();

WebAppConfig.UseDevAppConfigs(app);
WebAppConfig.UseGeneralAppConfigs(app);