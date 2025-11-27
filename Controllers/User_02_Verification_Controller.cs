using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Controllers
{
    [ApiController]
    [Route("api/user/verify")]
    public class User_02_Verification_Controller : ControllerBase
    {
        private readonly IUser_02_Verification_Service _service;

        public User_02_Verification_Controller(IUser_02_Verification_Service service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Verify([FromBody] User_02_Verification_Request_DTO req)
        {
            var result = await _service.VerifyAsync(req);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }
    }
}
