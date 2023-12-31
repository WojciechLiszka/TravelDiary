﻿using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Models;
using Mapster;

namespace TravelDiary.Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            var authenticationSettings = new AuthenticationSettings();

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });

            services.AddAuthorization();

            services.AddScoped<IAuthorizationHandler, DiaryResourceOperationRequirementHandler>();

            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            
            typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        }
    }
}