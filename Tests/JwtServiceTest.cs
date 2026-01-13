// JwtServiceTests.cs
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace MiniMazErpBack.Tests;

public class JwtServiceTests
{
    [Fact]
    public void GenerateJwtToken_WithValidClient_ReturnsToken()
    {
        // Arrange
        var jwtService = new JwtService();
        var client = new Client
        {
            Id = 1,
            Name = "testuser",
            HashedPassword = "dummy_hashed_password"  // AÑADIR ESTA LÍNEA
        };

        // Guardar variables originales
        var originalKey = Environment.GetEnvironmentVariable("JWT_KEY");
        var originalExpire = Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS");
        var originalIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var originalAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        // Configurar variables de entorno para el test
        Environment.SetEnvironmentVariable("JWT_KEY", "test_key_32_chars_long_1234567890");
        Environment.SetEnvironmentVariable("JWT_EXPIRE_HOURS", "24");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "TestIssuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "TestAudience");

        try
        {
            // Act
            var token = jwtService.GenerateJwtToken(client);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            
            // Verificar que es un JWT válido
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(token));
        }
        finally
        {
            // Restaurar variables originales
            Environment.SetEnvironmentVariable("JWT_KEY", originalKey);
            Environment.SetEnvironmentVariable("JWT_EXPIRE_HOURS", originalExpire);
            Environment.SetEnvironmentVariable("JWT_ISSUER", originalIssuer);
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", originalAudience);
        }
    }

    [Fact]
    public void GenerateJwtToken_WithNullClient_ThrowsException()
    {
        // Arrange
        var jwtService = new JwtService();

        // Configurar variables de entorno para el test
        var originalKey = Environment.GetEnvironmentVariable("JWT_KEY");
        Environment.SetEnvironmentVariable("JWT_KEY", "test_key_32_chars_long_1234567890");

        try
        {
            // Act & Assert
            // Usar "null!" para indicar que aceptamos el null en este contexto de test
            Assert.Throws<NullReferenceException>(() => jwtService.GenerateJwtToken(null!));
        }
        finally
        {
            // Restaurar variable
            Environment.SetEnvironmentVariable("JWT_KEY", originalKey);
        }
    }
}