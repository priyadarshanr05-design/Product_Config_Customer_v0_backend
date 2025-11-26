using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product_Config_Customer_v0.Data.Seeders;
using Product_Config_Customer_v0.Repositories;
using Product_Config_Customer_v0.Repositories.Interfaces;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Shared;
using Product_Config_Customer_v0.Workers;

namespace Product_Config_Customer_v0.Shared
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration config)
        {
            // Scoped services
            services.AddScoped<IUser_Login_TenantProvider, User_Login_TenantProvider>();
            services.AddScoped<IUser_Login_DatabaseResolver, User_Login_DatabaseResolver>();
            services.AddScoped<IUser_Login_Service, User_Login_Service>();

            services.AddScoped<IRequestModelRepository>(sp =>
            {
                var connStr = config.GetConnectionString("DefaultConnection");
                return new RequestModelRepository(connStr);
            });

            services.AddScoped<IRequestModelDomainRepository, RequestModelDomainRepository>();
            services.AddScoped<ParentAPI_01_ProcessRequest_Service>();
            services.AddScoped<ParentAPI_01_DuplicateHandler_Service>();
            services.AddScoped<ParentAPI_02_Request_Status_Service>();
            services.AddScoped<ParentAPI_01_GenToken_Service>();

            // Singletons
            services.AddSingleton<User_Login_Jwt_Token_Service>();
            services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();
            services.AddHostedService<ParentAPI_ProcessRequest_Worker>();
            services.AddScoped<UserSeeder>();
            services.AddScoped<DomainAdminUserSeeder>();

            services.AddScoped<Domain_02_Add_Service>();
            services.AddScoped<Domain_03_Delete_Service>();
            services.AddScoped<Domain_01_AdminLogin_Service>();

            services.AddScoped<Users_06_InternalEmailDomain_Add_Service>();
            services.AddScoped<Users_09_InternalEmailDomain_Delete_Service>();
            services.AddScoped<Users_07_InternalEmailDomain_Read_Service>();
            services.AddScoped<Users_08_InternalEmailDomain_Update_Service>();

            // Transient services
            services.AddTransient<IEmailSender, SmtpEmailSender>();

            return services;
        }

        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
