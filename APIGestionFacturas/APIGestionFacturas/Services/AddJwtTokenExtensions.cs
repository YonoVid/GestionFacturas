﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Services
{
    public static class AddJwtTokenServiceExtensions
    {
        /// <summary>
        /// Make all the configuration to all JWT token service to Services of API.
        /// </summary>
        /// <param name="service"> Services of the API. </param>
        /// <param name="configuration"> Options to configure JWT service. </param>
        public static void AddJwtTokenService(this IServiceCollection service, IConfiguration configuration)
        {
            //Add JWT settings
            var bindJwtSettings = new JwtSettings();

            configuration.Bind("JsonWebTokenKeys", bindJwtSettings);

            service.AddSingleton(bindJwtSettings);

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = bindJwtSettings.ValidateIssuerSigningKey,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(bindJwtSettings.IssuerSigningKey)),
                        ValidateIssuer = bindJwtSettings.ValidateIssuer,
                        ValidIssuer = bindJwtSettings.ValidIssuer,
                        ValidateAudience = bindJwtSettings.ValidateAudience,
                        ValidAudience = bindJwtSettings.ValidAudience,
                        RequireExpirationTime = bindJwtSettings.RequireExpirationTime,
                        ValidateLifetime = bindJwtSettings.ValidateLifetime,
                        ClockSkew = TimeSpan.FromDays(1)
                    };
                });
        }
    }
}
