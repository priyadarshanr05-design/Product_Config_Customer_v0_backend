using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Data.Seeders;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Repositories;
using Product_Config_Customer_v0.Repositories.Interfaces;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;
using Product_Config_Customer_v0.Shared;
using Product_Config_Customer_v0.Workers;
using System.Net;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Load the default .env file (if present)
Env.Load();

// Load environment-specific .env file
if (builder.Environment.IsDevelopment())
{
    Env.Load(".env.development");
}
else if (builder.Environment.IsProduction())
{
    Env.Load(".env.production");
}

// Optional: log environment
Console.WriteLine($"Running in {builder.Environment.EnvironmentName} mode!");


// Make environment variables override appsettings.json
builder.Configuration
       .AddEnvironmentVariables();

// Register IHttpContextAccessor first
builder.Services.AddHttpContextAccessor();

// Add custom application services (TenantProvider, repositories, JWT service, etc.)
builder.Services.AddCustomServices(builder.Configuration);

// Add DbContexts (default for migrations, tenant DbContext configured later)
builder.Services.AddDbContext<DomainManagementDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DomainManagementDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DomainManagementDb"))
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

// Swagger with JWT
builder.Services.AddSwaggerWithJwt();

// CORS and HttpClient
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();


builder.Services.AddDbContext<DomainManagementDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DomainManagementDb");
    options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});


var app = builder.Build();

// Middleware
var enableSwaggerInProd = Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

if (app.Environment.IsDevelopment() || enableSwaggerInProd)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseTenantResolution();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeederService>();
    await seeder.MigrateAndSeedAsync();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var domainDb = services.GetRequiredService<DomainManagementDbContext>();
    var userSeeder = services.GetRequiredService<UserSeeder>();
    var domainAdminSeeder = services.GetRequiredService<DomainAdminUserSeeder>();

    // Migrate DomainManagement DB
    await domainDb.Database.MigrateAsync();

    // Seed DomainAdminUsers
    await domainAdminSeeder.SeedAsync();

    // Fetch all tenant database names dynamically
    var tenantDbNames = await domainDb.AnonymousRequestControls
        .Select(d => d.DatabaseName)
        .ToListAsync();

    // Migrate & seed each tenant DB
    foreach (var dbName in tenantDbNames)
    {
        var connString = $"server={Environment.GetEnvironmentVariable("DB_SERVER")};port={Environment.GetEnvironmentVariable("DB_PORT")};database={dbName};user={Environment.GetEnvironmentVariable("DB_USERNAME")};password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));

        using var tenantDb = new ApplicationDbContext(optionsBuilder.Options);
        await tenantDb.Database.MigrateAsync();

        // Seed user-related data
        await userSeeder.SeedAsync(tenantDb);

        // Transform dbName into a proper email domain
        string domainName = dbName;
        if (domainName.StartsWith("CustDb_", StringComparison.OrdinalIgnoreCase))
        {
            domainName = domainName.Substring("CustDb_".Length); // remove prefix
        }
        domainName = domainName.ToLower(); // convert to lowercase
        domainName += ".com"; // append .com

        // Seed InternalUsersEmailDomains if empty
        if (!tenantDb.InternalUsersEmailDomains.Any())
        {
            tenantDb.InternalUsersEmailDomains.Add(new Users_InternalEmailDomain
            {
                Id = 1,
                EmailDomain = "visualallies.com"
            });

            tenantDb.InternalUsersEmailDomains.Add(new Users_InternalEmailDomain
            {
                Id = 2,
                EmailDomain = domainName  // dynamically generated
            });

            await tenantDb.SaveChangesAsync();
        }

        Console.WriteLine($"Migration & seeding applied for tenant database: {dbName}");
    }
}

app.Run();
