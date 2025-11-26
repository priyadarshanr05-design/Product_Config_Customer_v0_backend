using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services;

namespace Product_Config_Customer_v0.Controllers
{
    [ApiController]
    [Route("api/user/verify")]
    public class User_02_Verification_Controller : ControllerBase
    {
        private readonly User_02_Verification_Service _service;

        public User_02_Verification_Controller(User_02_Verification_Service service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Verify([FromBody] User_02_Verification_Request_DTO req)
        {
            var result = _service.Verify(req);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
