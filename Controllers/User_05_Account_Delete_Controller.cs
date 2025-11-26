using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;
using System.Security.Claims;

namespace Product_Config_Customer_v0.Controllers
{
    [ApiController]
    [Route("api/user/delete")]
    public class User_05_Account_Delete_Controller : ControllerBase
    {
        private readonly User_05_Account_Delete_Service _service;
        private readonly IUser_Login_TenantProvider _tenantProvider;

        public User_05_Account_Delete_Controller(
            User_05_Account_Delete_Service service,
            IUser_Login_TenantProvider tenantProvider)
        {
            _service = service;
            _tenantProvider = tenantProvider;
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var tenant = User.Claims.FirstOrDefault(c => c.Type == "tenant")?.Value;

            if (tenant == null)
                return Unauthorized("Tenant missing.");

            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("User ID claim missing in token.");

            var userId = int.Parse(userIdClaim);

            var result = await _service.DeleteAsync(tenant, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
