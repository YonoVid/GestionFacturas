using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIGestionFacturas.Tests.Controller
{
    public class BaseControllerTest : WebApplicationFactory<Program>
    {
        protected HttpClient _httpClient = null!;

        protected BaseControllerTest()
        {
            var webAppFactory = new WebApplicationFactory<Program>();

            _httpClient = webAppFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        var config = new OpenIdConnectConfiguration()
                        {
                            Issuer = MockJwtToken.Issuer
                        };

                        config.SigningKeys.Add(MockJwtToken.SecurityKey);
                        options.Configuration = config;
                    });
                });
            }).CreateClient();
        }

        protected void AddUserToken()
        {
            Claim[] claims = {
                new Claim("Id", (-1).ToString()),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Email, "example@mail.com"),
                new Claim(ClaimTypes.NameIdentifier, (-1).ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(10).ToString("MMM ddd dd yyyy HH:mm:ss tt")),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("UserOnly", "User")
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MockJwtToken.GenerateJwtToken(claims));

        }

        protected void AddAdminToken()
        {
            Claim[] claims = {
                new Claim("Id", (-1).ToString()),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Email, "example@mail.com"),
                new Claim(ClaimTypes.NameIdentifier, (-1).ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(10).ToString("MMM ddd dd yyyy HH:mm:ss tt")),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim("UserOnly", "Admin")
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MockJwtToken.GenerateJwtToken(claims));
        }
    }
}
