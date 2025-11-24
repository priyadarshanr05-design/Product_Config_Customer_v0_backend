using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Repositories
{
    public class RequestModelRepository : IRequestModelRepository
    {
        private readonly string _connString;

        public RequestModelRepository(string connString)
        {
            _connString = connString;
        }

        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(_connString, ServerVersion.AutoDetect(_connString))
                .Options;

            return new ApplicationDbContext(options);
        }

        public async Task<ParentAPI_Model_Request?> GetByRequestHashAsync(string requestHash)
        {
            await using var db = CreateDbContext();
            return await db.ParentAPI_Model_Requests.FirstOrDefaultAsync(r => r.RequestHash == requestHash);
        }

        public async Task<ParentAPI_Model_Request?> GetByRequestIdAsync(string requestId)
        {
            await using var db = CreateDbContext();
            return await db.ParentAPI_Model_Requests.FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        public async Task AddAsync(ParentAPI_Model_Request request)
        {
            await using var db = CreateDbContext();
            db.ParentAPI_Model_Requests.Add(request);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParentAPI_Model_Request request)
        {
            await using var db = CreateDbContext();
            db.ParentAPI_Model_Requests.Update(request);
            await db.SaveChangesAsync();
        }
    }
}
