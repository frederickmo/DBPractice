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
    /// <summary>
    /// 登录
    /// 返回JWT token
    /// </summary>
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
    /// <summary>
    /// 注册
    /// </summary>
    [ApiController]
    [Route("User/[controller]")]
    public class RegisterController : Controller 
    {
        /// <summary>
        /// 垃圾投递者的注册
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("GarbageMan")] 
        public RegisterResponse Register([FromBody] GMRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾站检查员注册，注意鉴权
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Watcher")] 
        public RegisterResponse Register([FromBody] WCRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 运输员注册，注意鉴权
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Carrier")] 
        public RegisterResponse Register([FromBody] CRRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾处理站员工录入
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("StationStaff")] 
        public RegisterResponse Register([FromBody] SSRegisterRequest req)
        {
            var resp = new RegisterResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 用户属性更新
        /// </summary>
        [ApiController]
        [Route("User/[controller]")]
        public class UpdateController : Controller
        {
            /// <summary>
            /// 垃圾投递人状态更新
            /// </summary>
            /// <param name="req"></param>
            /// <returns></returns>
            [HttpPost("GarbageMan")]
            public UpdateResponse Register([FromBody] GMRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }
            /// <summary>
            /// 垃圾站监察员状态更新
            /// </summary>
            /// <param name="req"></param>
            /// <returns></returns>
            [HttpPost("Watcher")]
            public UpdateResponse Register([FromBody] WCRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }
            /// <summary>
            /// 运输员的状态更新
            /// </summary>
            /// <param name="req"></param>
            /// <returns></returns>
            [HttpPost("Carrier")]
            public UpdateResponse Register([FromBody] CRRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }
            /// <summary>
            /// 垃圾处理站员工的更新
            /// </summary>
            /// <param name="req"></param>
            /// <returns></returns>
            [HttpPost("StationStaff")]
            public UpdateResponse Register([FromBody] SSRegisterRequest req)
            {
                var resp = new UpdateResponse {status = Config.TEST};
                return resp;
            }
        }

    }
    /// <summary>
    /// 用户注销
    /// </summary>
    [ApiController]
    [Route("User/[controller]")]
    public class DeleteController : Controller 
    {
        /// <summary>
        /// 注销垃圾投递人员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("GarbageMan")] 
        public Response Delete([FromBody] GarbageMan req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 注销监察员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Watcher")] 
        public Response Delete([FromBody] Watcher req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 注销运输人员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Carrier")] 
        public Response Delete([FromBody] Carrier req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 注销垃圾处理站员工
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("StationStaff")]
        public Response Delete([FromBody] StationStaff req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
    }
    /// <summary>
    /// 用户信息的获取
    /// </summary>
    [ApiController]
    [Route("User/[controller]")]
    public class GetInformationController : Controller                    
    {        
        /// <summary>
        /// 垃圾投递人员的信息查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("GarbageMan")]                                  
        public GarbageMan GetGarbageMan()       
        {                                                         
            var resp = new GarbageMan();       
            return resp;                                          
        }
        /// <summary>
        /// 监察员信息的查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("Watcher")]                                     
        public Watcher GetWatcher()          
        {                                                         
            var resp = new Watcher();       
            return resp;                                          
        }   
        /// <summary>
        /// 运输人员信息的查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("Carrier")]                          
        public Carrier GetCarrier()          
        {                                                         
            var resp = new Carrier();       
            return resp;                                          
        }                                                         
        /// <summary>
        /// 垃圾处理站员工信息的查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("StationStaff")]                                
        public StationStaff GetStationStaff()     
        {                                                         
            var resp = new StationStaff();       
            return resp;                                          
        }
        /*
         * 下面就是一个鉴权并获取当前用户信息的实例
         */
        /// <summary>
        /// 小测试
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles="administrator")]//只允许administrator进来
        [HttpGet("Test")]
        public string test()
        {
            if (Response.HttpContext.User.Identity != null) return Response.HttpContext.User.Identity.Name;//Response.HttpContext.User.Identity.Name 对应了你的claim里的东西。
            return "NotFound";
        }
    }                                                             
}