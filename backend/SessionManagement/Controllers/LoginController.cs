using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SessionManagement.Interface;
using SessionManagement.Models.DTO;
using SessionManagement.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SessionManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUser service;
        private readonly IConfiguration _config;
        public LoginController(IUser service, IConfiguration _config)
        {
            this._config = _config;
            this.service = service;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(AddUser user)
        {
            var registerUser = await service.RegisterUser(user);

            try
            {
                if (registerUser)
                {
                    return Ok(service);
                }

                return Conflict(registerUser);
            }

            catch(Exception ex)
            {
                return BadRequest("Error: "+ ex.Message);
            }
          
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(UserLogin user)
        {
            var login = await service.LoginUser(user);

            try
            {
                if(login.IsLogin)
                {
                    return Ok(new {token = login.Token, Expiration = login.Expiration});
                }
                    
                return Unauthorized(login.Message);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserDetails")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetUserDetails(string username)
        {
            var usernameDetails = await service.GetUserDetails(username);   

            try
            {
                if(usernameDetails != null)
                {
                    return Ok(usernameDetails);
                }

                return NotFound("No Data");
            }

            catch
            {
                return Unauthorized("Not allowed to view this");
            }
        }

    }
}
