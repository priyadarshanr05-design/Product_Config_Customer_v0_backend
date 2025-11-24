using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Services;

namespace Product_Config_Customer_v0.Controllers
{
    [ApiController]
    [Route("api/user/delete")]
    public class User_Account_Delete_Controller : ControllerBase
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;
        private readonly IUser_Login_TenantProvider _tenantProvider;

        public User_Account_Delete_Controller(
            IUser_Login_DatabaseResolver dbResolver,
            IUser_Login_TenantProvider tenantProvider)
        {
            _dbResolver = dbResolver;
            _tenantProvider = tenantProvider;
        }

        [Authorize]
        [HttpDelete]
        public IActionResult Delete()
        {
            var tenant = _tenantProvider.TenantKey;
            if (!_dbResolver.TryGetConnectionString(tenant, out var connString))
                return BadRequest("Unknown domain. Please check the domain name.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString))
                .Options;

            using var db = new ApplicationDbContext(options);

            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (userIdClaim == null)
                return Unauthorized("User ID claim missing in token.");

            var userId = int.Parse(userIdClaim.Value);

            var user = db.Users.Find(userId);
            if (user == null)
                return NotFound();

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok("User account deleted.");
        }
    }
}
