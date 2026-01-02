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
        // app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
