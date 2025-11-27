using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product_Config_Customer_v0.Data.Seeders;
using Product_Config_Customer_v0.Repositories;
using Product_Config_Customer_v0.Repositories.Interfaces;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;
using Product_Config_Customer_v0.Shared;
using Product_Config_Customer_v0.Workers;

namespace Product_Config_Customer_v0.Shared
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration config)
        {
            // Scoped services
            services.AddScoped<IUser_03_Login_TenantProvider, User_03_Login_TenantProvider>();
            
            services.AddScoped<IRequestModelRepository>(sp =>
            {
                var connStr = config.GetConnectionString("DefaultConnection");
                return new RequestModelRepository(connStr);
            });

            services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>(); 
            services.AddScoped<IRequestModelDomainRepository, RequestModelDomainRepository>();

            services.AddScoped<IUser_01_Register_Service, User_01_Register_Service>();
            services.AddScoped<IUser_02_Verification_Service, User_02_Verification_Service>();            
            services.AddScoped<IUser_03_Login_Service, User_03_Login_Service>();
            services.AddScoped<IUser_03_Login_TenantProvider, User_03_Login_TenantProvider>();
            services.AddScoped<IUser_04_ForgetPassword_Service, User_04_ForgetPassword_Service>();
            services.AddScoped<IUser_05_Account_Delete_Service, User_05_Account_Delete_Service>();

            services.AddScoped<IUsers_01_InternalEmailDomain_Add_Service, Users_01_InternalEmailDomain_Add_Service>();
            services.AddScoped<IUsers_02_InternalEmailDomain_Read_Service, Users_02_InternalEmailDomain_Read_Service>();
            services.AddScoped<IUsers_03_InternalEmailDomain_Update_Service, Users_03_InternalEmailDomain_Update_Service>();
            services.AddScoped<IUsers_04_InternalEmailDomain_Delete_Service, Users_04_InternalEmailDomain_Delete_Service>();
            services.AddScoped<IUsers_05_InternalEmailDomain_Check_Service, Users_05_InternalEmailDomain_Check_Service>();

            services.AddScoped<ParentAPI_01_CheckAllowAnonymous>();

            services.AddScoped<IDomain_01_AdminLogin_Service, Domain_01_AdminLogin_Service>();
            services.AddScoped<IDomain_02_Add_Service, Domain_02_Add_Service>();
            services.AddScoped<IDomain_03_Read_Service, Domain_03_Read_Service>();
            services.AddScoped<IDomain_04_Update_Service, Domain_04_Update_Service>();
            services.AddScoped<IDomain_05_Delete_Service, Domain_05_Delete_Service>();

            services.AddScoped<IParentAPI_01_ProcessRequest_Service, ParentAPI_01_ProcessRequest_Service>();
            services.AddScoped<IParentAPI_01_DuplicateHandler_Service, ParentAPI_01_DuplicateHandler_Service>();
            services.AddScoped<IParentAPI_02_Request_Status_Service, ParentAPI_02_Request_Status_Service>();
            services.AddScoped<IParentAPI_01_GenToken_Service, ParentAPI_01_GenToken_Service>();

            services.AddScoped<IFeaturePage_01_Service, FeaturePage_01_Service>();


            // Singletons
            services.AddSingleton<IUser_03_Login_Jwt_Token_Service, User_03_Login_Jwt_Token_Service>();
            services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();
            services.AddHostedService<ParentAPI_ProcessRequest_Worker>();
            services.AddScoped<UserSeeder>();
            services.AddScoped<SeederService>();
            services.AddScoped<DomainAdminUserSeeder>();

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
