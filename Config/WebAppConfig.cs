using Scalar.AspNetCore;

namespace MiniMazErpBack;

public class WebAppConfig
{
    public static void UseDevAppConfigs(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }

    public static void UseGeneralAppConfigs(WebApplication app)
    {
        app.UseCors("AllowSpecificOrigin");
        app.UseAuthentication();
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}
