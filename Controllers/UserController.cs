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
using Microsoft.Extensions.Options;
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
            if (true)
            {
                resp.status = Config.SUCCESS;
                resp.token = GenerateJWT(req.username, "administrator");
            }
            
            return resp;
        }

        private string GenerateJWT(string username, string role)
        {
            var algorithm = SecurityAlgorithms.HmacSha256;
            var claims = new[]
            {
                //new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name,username),
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
    [ApiController]
    [Route("User/[controller]")]
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
        public RegisterResponse Register([FromBody] CRRegisterRequest req)
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

        [ApiController]
        [Route("User/[controller]")]
        public class UpdateController : Controller
        {
            [HttpPost("GarbageMan")]
            public UpdateResponse Register([FromBody] GMRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }

            [HttpPost("Watcher")]
            public UpdateResponse Register([FromBody] WCRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }

            [HttpPost("TransportPersonnel")]
            public UpdateResponse Register([FromBody] CRRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }

            [HttpPost("StationStaff")]
            public UpdateResponse Register([FromBody] SSRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }
        }

    }
    [ApiController]
    [Route("User/[controller]")]
    public class UpdateController : Controller 
    {
        [HttpPost("GarbageMan")] 
        public Response Update([FromBody] GarbageMan req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        [HttpPost("Watcher")] 
        public Response Update([FromBody] Watcher req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        [HttpPost("TransportPersonnel")] 
        public Response Update([FromBody] Carrier req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }

        [HttpPost("StationStaff")]
        public Response Update([FromBody] StationStaff req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
    }
    [ApiController]
    [Route("User/[controller]")]
    public class GetInformationController : Controller                    
    {                                                             
        [HttpGet("GarbageMan")]                                  
        public GarbageMan Get([FromBody] GarbageMan req)       
        {                                                         
            var resp = new GarbageMan();       
            return resp;                                          
        }

        [HttpGet("Watcher")]                                     
        public Watcher Get([FromBody] Watcher req)          
        {                                                         
            var resp = new Watcher();       
            return resp;                                          
        }                                                         
        [HttpGet("TransportPersonnel")]                          
        public Carrier Get([FromBody] Carrier req)          
        {                                                         
            var resp = new Carrier();       
            return resp;                                          
        }                                                         
                                                              
        [HttpGet("StationStaff")]                                
        public StationStaff Get([FromBody] StationStaff req)     
        {                                                         
            var resp = new StationStaff();       
            return resp;                                          
        }
        /*
         * 下面就是一个鉴权并获取当前用户信息的实例
         */
        [Authorize(Roles="administrator")]//只允许administraotr进来
        [HttpGet("Test")]
        public string test()
        {
            if (Response.HttpContext.User.Identity != null) return Response.HttpContext.User.Identity.Name;//Response.HttpContext.User.Identity.Name 对应了你的claim里的东西。
            return "NotFound";
        }
    }                                                             
}