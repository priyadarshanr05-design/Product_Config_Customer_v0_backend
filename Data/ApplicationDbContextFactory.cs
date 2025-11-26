using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Product_Config_Customer_v0.Data;
using DotNetEnv;
using System;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Load .env.development into Environment variables
        Env.Load(".env.development");

        // Use a dummy/local tenant database for design-time migrations
        string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__TenantDevDb");

        // Interpolate ${DB_XXX} variables if any
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = connectionString
                .Replace("${DB_SERVER}", Environment.GetEnvironmentVariable("DB_SERVER") ?? "127.0.0.1")
                .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT") ?? "3306")
                .Replace("${DB_USERNAME}", Environment.GetEnvironmentVariable("DB_USERNAME") ?? "root")
                .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "pass");
        }

        // Fallback if connection string is missing
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("Using fallback local tenant DB for migration only.");
            connectionString = "server=127.0.0.1;port=3306;database=tenant_dev;user=root;password=pass;";
        }

        Console.WriteLine($"[DESIGN-TIME DB CONNECTION] → {connectionString}");

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
