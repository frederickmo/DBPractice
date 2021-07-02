using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DBPractice.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DBPractice.Controllers
{
    [ApiController]
    [Route("User/[controller]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost]
        public LoginResponse Login([FromBody] LoginRequest req)
        {
            var resp = new LoginResponse {token = "", role = "", status = Config.TEST};

            return resp;
        }

        private string GenerateJWT(string username, string role)
        {
            var algorithm = SecurityAlgorithms.HmacSha256;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role),
            };
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var signingCredentials = new SigningCredentials(secretKey, algorithm);
            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"], //Issuer
                _configuration["JWT:Audience"], //Audience
                claims, //Claims,
                DateTime.Now, //notBefore
                DateTime.Now.AddDays(1), //expires
                signingCredentials
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }

    public class RegisterController : Controller 
    {
        [HttpPost("GarbageMan")] 
        public RegisterResponse Register([FromBody] GMRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
        [HttpPost("Watcher")] 
        public RegisterResponse Register([FromBody] WCRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
        [HttpPost("TransportPersonnel")] 
        public RegisterResponse Register([FromBody] TPRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
        [HttpPost("StationStaff")] 
        public RegisterResponse Register([FromBody] SSRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
    }
    
    [ApiController]
    [Route("User/[controller]")]
    public class UpdateController : Controller 
    {
        [HttpPost]
        public UpdateResponse Update([FromBody] UpdateRequest req)
        {
            var resp = new UpdateResponse {Status = Config.TEST};
            return resp;
        }
    }
        
    [ApiController]
    [Route("User")]
    public class UserController : Controller 
    {
        [HttpGet]
        public User GetUser(string token)
        {
            var user = new User();
            
            return user;
        }
    }
}