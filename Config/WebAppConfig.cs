namespace MiniMazErpBack;

public class WebAppConfig
{
    public static void UseDevAppConfigs(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }

    public static void UseGeneralAppConfigs(WebApplication app)
    {
        app.UseCors("AllowSpecificOrigin");
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}
