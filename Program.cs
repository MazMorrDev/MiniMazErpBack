using DotNetEnv;
using MiniMazErpBack;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
WebAppBuilderConfig.ConfigureBuilder(builder, Env.GetString("CONECCTION_STRING_POSTGRESQL"));
WebAppBuilderConfig.ConfigureCorsPolicy(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();