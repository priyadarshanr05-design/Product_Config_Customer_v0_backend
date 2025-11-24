using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Product_Config_Customer_v0.Data
{
    public class DomainManagementDbContextFactory : IDesignTimeDbContextFactory<DomainManagementDbContext>
    {
        public DomainManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DomainManagementDbContext>();

            // Design-time connection string
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=pass;database=product_config_domain_management",
                ServerVersion.AutoDetect("server=localhost;user=root;password=pass;database=product_config_domain_management")
            );

            return new DomainManagementDbContext(optionsBuilder.Options);
        }
    }
}
