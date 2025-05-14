using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoginRequest loginRequest)
        {
            return Unauthorized();
            // Validate the login credentials (Replace this with your validation logic)
            if (IsValidLogin(loginRequest.Username, loginRequest.Password))
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecurityKey"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    null,
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: credentials));

                return Ok(token);
            }

            return Unauthorized(); // Or handle invalid login in a way that fits your application
        }

        private bool IsValidLogin(string username, string password)
        {
            // Example logic to check against hardcoded values (Replace with database check)
            return username == "shindeyash1999.calyx@gmail.com" && password == "Yash@2528";
        }

    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

