using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Controllers
{
    [ApiController]
    [Route("api/user/forgetpassword")]
    public class User_04_ForgetPassword_Controller : ControllerBase
    {
        private readonly IUser_04_ForgetPassword_Service _service;

        public User_04_ForgetPassword_Controller(IUser_04_ForgetPassword_Service service)
        {
            _service = service;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestReset([FromBody] User_04_ForgetPassword_Request_DTO req)
        {
            var result = await _service.RequestResetAsync(req);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] User_04_ForgetPassword_Reset_DTO req)
        {
            var result = await _service.ResetPasswordAsync(req);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
