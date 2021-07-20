using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Oracle.ManagedDataAccess.Client;
// ReSharper disable InterpolatedStringExpressionIsNotIFormattable
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
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="configuration"></param>
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 登录用户id以第一位区分角色
        /// 1:GarbageMan;
        /// 2:Watcher;
        /// 3:Carrier;
        /// 4:StationStaff;
        /// 5:Administrator
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public LoginResponse Login([FromBody] LoginRequest req)
        {
            var resp = new LoginResponse { token = "", role = "", status = Config.FAIL };
            if (string.IsNullOrEmpty(req.userID) || string.IsNullOrEmpty(req.password))
            {
                resp.status = Config.EMPTY;
                return resp;
            }
            var crypt = DBConn.MD5Encrypt16(req.password);
            var garbageMan = $"SELECT* FROM alluser WHERE user_id='{req.userID}' and user_password='{crypt}'";
            var watcher = $"SELECT* FROM watcher WHERE watcher_id='{req.userID}' and watcher_password='{crypt}'";
            var carrier = $"SELECT* FROM carrier WHERE carrier_id='{req.userID}' and carrier_password='{crypt}'";
            var stationStaff = $"SELECT* FROM staff WHERE staff_id='{req.userID}' and staff_password='{crypt}'";
            var administrator = $"SELECT* FROM administrator  WHERE adm_id  ='{req.userID}' and adm_password='{crypt}'";
            var conn = DBConn.OpenConn();
            switch(req.userID[0])
            {
                case '1':
                    if (FindUser(conn, garbageMan))
                    {
                        resp.token = GenerateJWT(req.userID, "GarbageMan");
                        resp.status = Config.SUCCESS;
                        resp.role = "GarbageMan";
                        resp.loginMessage = "登录成功";
                        return resp;
                    }
                    break;
                case '2':
                    if (FindUser(conn, watcher))
                    {
                        resp.token = GenerateJWT(req.userID, "Watcher");
                        resp.status = Config.SUCCESS;
                        resp.role = "Watcher";
                        resp.loginMessage = "登录成功";
                        DBConn.CloseConn(conn);
                        return resp;
                    }
                    break;
                case '3':
                    if (FindUser(conn, carrier))
                    {
                        resp.token = GenerateJWT(req.userID, "Carrier");
                        resp.status = Config.SUCCESS;
                        resp.role = "Carrier";
                        resp.loginMessage = "登录成功";
                        DBConn.CloseConn(conn);
                        return resp;
                    }
                    break;
                case '4':
                    if (FindUser(conn, stationStaff))
                    {
                        resp.token = GenerateJWT(req.userID, "StationStaff");
                        resp.status = Config.SUCCESS;
                        resp.role = "StationStaff";
                        resp.loginMessage = "登录成功";
                        DBConn.CloseConn(conn);
                        return resp;
                    }
                    break;                                   
                case '5':
                    if (FindUser(conn, administrator))
                    {
                        resp.token = GenerateJWT(req.userID, "Administrator");
                        resp.status = Config.SUCCESS;
                        resp.role = "Administrator";
                        resp.loginMessage = "登录成功";
                        DBConn.CloseConn(conn);
                        return resp;
                    }
                    break;
                default:
                    resp.status = Config.FAIL;
                    resp.loginMessage = "登录失败，角色的ID格式不正确";
                    DBConn.CloseConn(conn);
                    return resp;                   
            }
            resp.loginMessage = "登录失败，用户名或密码错误";
            resp.status = Config.FAIL;
            DBConn.CloseConn(conn);
            return resp;
        }
        private bool FindUser(OracleConnection conn, string command)
        {
            var k = 0;
            try
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = command;
                var reader = cmd.ExecuteReader();
                while (reader.Read()) { k++; }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            if (k == 1) return true;
            else return false;
        }
        private string GenerateJWT(string username, string role)
        {
            const string algorithm = SecurityAlgorithms.HmacSha256;
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
            var resp = new RegisterResponse();
            if (string.IsNullOrEmpty(req.garbageman.id) || 
                string.IsNullOrEmpty(req.garbageman.name)||
                string.IsNullOrEmpty(req.garbageman.address)||
                string.IsNullOrEmpty(req.garbageman.password)||
                string.IsNullOrEmpty(req.garbageman.phonenumber)
                )
            {
                resp.status = Config.EMPTY;
                return resp;
            }
            var crypt = DBConn.MD5Encrypt16(req.garbageman.password);//加密密码
            if(string.IsNullOrEmpty(req.garbageman.id)||req.garbageman.id[0]!='1')
            {
                resp.status = Config.FAIL;
                resp.registerMessage = "投放人ID格式不正确，须以1开头";
                return resp;
            }
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO alluser " +
                                 $"VALUES('{req.garbageman.id}','{req.garbageman.name}','{crypt}','{req.garbageman.phonenumber}'," +
                                 $"'{req.garbageman.address}','0')";//初始积分默认为0
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.registerMessage = "注册成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.registerMessage = ex.Message;//返回异常提示（如数据库连接失败，ID重复等）
            }
            DBConn.CloseConn(conn);
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
            
            var resp = new RegisterResponse();
            if (string.IsNullOrEmpty(req.watcher.id) || 
                string.IsNullOrEmpty(req.watcher.name)||
                string.IsNullOrEmpty(req.watcher.address)||
                string.IsNullOrEmpty(req.watcher.password)||
                string.IsNullOrEmpty(req.watcher.phonenumber)
            )
            {
                resp.status = Config.EMPTY;
                return resp;
            }
            string crypt = DBConn.MD5Encrypt16(req.watcher.password);//加密密码
            if (req.watcher.id[0] != '2')
            {
                resp.status = Config.FAIL;
                resp.registerMessage = "看管人ID格式不正确，须以2开头";
                return resp;
            }
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO watcher " +
                                 $"VALUES('{req.watcher.id}','{req.watcher.name}','{req.watcher.phonenumber}','{crypt}'," +
                                 $"'{req.watcher.address}')";//初始积分默认为0
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.registerMessage = "注册成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.registerMessage = ex.Message;//返回异常提示（如数据库连接失败，ID重复等）
            }
            DBConn.CloseConn(conn);
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
            var resp = new RegisterResponse();
            if (string.IsNullOrEmpty(req.transportpersonnel.id) || 
                string.IsNullOrEmpty(req.transportpersonnel.name)||
                string.IsNullOrEmpty(req.transportpersonnel.password)||
                string.IsNullOrEmpty(req.transportpersonnel.phonenumber)
            )
            {
                resp.status = Config.EMPTY;
                return resp;
            }
            var crypt = DBConn.MD5Encrypt16(req.transportpersonnel.password);//加密密码
            if (req.transportpersonnel.id[0] != '3')
            {
                resp.status = Config.FAIL;
                resp.registerMessage = "运输人ID格式不正确，须以3开头";
                return resp;
            }
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO carrier " +
                                 $"VALUES('{req.transportpersonnel.id}','{req.transportpersonnel.name}'," +
                                 $"'{req.transportpersonnel.phonenumber}','{crypt}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.registerMessage = "注册成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.registerMessage = ex.Message;//返回异常提示（如数据库连接失败，ID重复等）
            }
            DBConn.CloseConn(conn);
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
            var resp = new RegisterResponse();
            if (string.IsNullOrEmpty(req.stationstaff.id) || 
                string.IsNullOrEmpty(req.stationstaff.name)||
                string.IsNullOrEmpty(req.stationstaff.password)||
                string.IsNullOrEmpty(req.stationstaff.phonenumber)||
                string.IsNullOrEmpty(req.stationstaff.plantname)
            )
            {
                resp.status = Config.EMPTY;
                return resp;
            }
            var crypt = DBConn.MD5Encrypt16(req.stationstaff.password);//加密密码
            if (req.stationstaff.id[0] != '4')
            {
                resp.status = Config.FAIL;
                resp.registerMessage = "处理人ID格式不正确，须以4开头";
                return resp;
            }
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO staff " +
                                 $"VALUES('{req.stationstaff.id}','{req.stationstaff.name}'," +
                                 $"'{req.stationstaff.phonenumber}','{req.stationstaff.plantname}','{crypt}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.registerMessage = "注册成功";
                
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.registerMessage = ex.Message;//返回异常提示（如数据库连接失败，ID重复等）
            }
            DBConn.CloseConn(conn);
            return resp;
        }
/*
        /// <summary>
        /// 管理人员信息注册
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Administrator")]
        public RegisterResponse Register([FromBody] ADRegisterRequest req)
        {
            var crypt = DBConn.MD5Encrypt16(req.administrator.password);//加密密码
            var resp = new RegisterResponse();
            if (req.administrator.id[0] != '5')
            {
                resp.status = Config.FAIL;
                resp.registerMessage = "管理员ID格式不正确，须以5开头";
                return resp;
            }
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO administrator " +
                                 $"VALUES('{req.administrator.id}','{req.administrator.name}'," +
                                 $"'{req.administrator.phonenumber}','{crypt}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.registerMessage = "注册成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.registerMessage = ex.Message;//返回异常提示（如数据库连接失败，ID重复等）
            }
            DBConn.CloseConn(conn);
            return resp;
        }*/
    }

    /// <summary>
    /// 用户属性更新(用户必须再次输入密码后才能更新相关信息)
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
        [Authorize(Roles = "Administrator,GarbageMan")]
        public UpdateResponse Update([FromBody] GarbageMan req)
        {
            var resp = new UpdateResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                //判断传入请求是否有空值，若有则不进行修改
                //DBConn.MD5Encrypt16(garbageman.password)
                var connectString = "UPDATE alluser " + $"SET user_id='{HttpContext.User.Identity.Name}'" +
                                    (req.name != "" ? $",user_name='{req.name}'" : "")+
                                    (req.password != "" ? $",user_password='{DBConn.MD5Encrypt16(req.password)}'" : "") +
                                    (req.phonenumber != "" ? $",phone_num='{req.phonenumber}'" : "")+
                                    (req.address != "" ? $",address='{req.address}' " : "")+
                                    $"WHERE user_id='{HttpContext.User.Identity.Name}'";
                cmd.CommandText = connectString;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.updateMessage = "更改成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.updateMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 垃圾站监察员状态更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Watcher")]
        [Authorize(Roles = "Administrator,Watcher")]
        public UpdateResponse Update([FromBody] Watcher req)
        {
            var resp = new UpdateResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                var connectString = "UPDATE WATCHER " + $"SET WATCHER_ID='{HttpContext.User.Identity.Name}'" +
                                    (req.name != "" ? $",WATCHER_NAME='{req.name}'" : "")+
                                    (req.password != "" ? $",WATCHER_PASSWORD='{DBConn.MD5Encrypt16(req.password)}'" : "") +
                                    (req.phonenumber != "" ? $",PHONE_NUM='{req.phonenumber}'" : "")+
                                    (req.address != "" ? $",ADDRESS='{req.address}' " : "")+
                                    $"WHERE WATCHER_ID='{HttpContext.User.Identity.Name}'";
                //判断传入请求是否有null值，若有则不进行修改
                cmd.CommandText = connectString;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.updateMessage = "更改成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.updateMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 运输员的状态更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Carrier")]
        [Authorize(Roles = "Administrator,Carrier")]
        public UpdateResponse Update([FromBody] CRUpdateRequest req) 
        {
            var resp = new UpdateResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM carrier " +
                                 $"WHERE carrier_id='{req.transportpersonnel.id}'";
                OracleDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (DBConn.MD5Encrypt16(req.password) != reader["carrier_password"].ToString())
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "密码错误";
                    DBConn.CloseConn(conn);
                    return resp;
                }

                var info = new Carrier
                {
                    id = reader["carrier_id"].ToString(),
                    name = !string.IsNullOrEmpty(req.transportpersonnel.name)
                        ? req.transportpersonnel.name
                        : reader["carrier_name"].ToString(),
                    password = !string.IsNullOrEmpty(req.transportpersonnel.password)
                        ? DBConn.MD5Encrypt16(req.transportpersonnel.password)
                        : reader["carrier_password"].ToString(),
                    phonenumber = !string.IsNullOrEmpty(req.transportpersonnel.phonenumber)
                        ? req.transportpersonnel.phonenumber
                        : reader["phone_num"].ToString()
                };
                //判断传入请求是否有null值，若有则不进行修改
                cmd.CommandText = "UPDATE carrier " +
                                  $"SET carrier_name='{info.name}',carrier_password='{info.password}',phone_num='{info.phonenumber}' " +
                                  $"WHERE carrier_id='{info.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.updateMessage = "更改成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.updateMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 垃圾处理站员工的更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("StationStaff")]
        [Authorize(Roles = "Administrator,StationStaff")]
        public UpdateResponse Update([FromBody] SSUpdateRequest req)
        {
            var resp = new UpdateResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM staff " +
                                 $"WHERE staff_id='{req.stationstaff.id}'";
                OracleDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (DBConn.MD5Encrypt16(req.password) != reader["staff_password"].ToString())
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "密码错误";
                    DBConn.CloseConn(conn);
                    return resp;
                }

                var info = new StationStaff
                {
                    id = reader["staff_id"].ToString(),
                    name = !string.IsNullOrEmpty(req.stationstaff.name)
                        ? req.stationstaff.name
                        : reader["staff_name"].ToString(),
                    password = !string.IsNullOrEmpty(req.stationstaff.password)
                        ? DBConn.MD5Encrypt16(req.stationstaff.password)
                        : reader["staff_password"].ToString(),
                    phonenumber = !string.IsNullOrEmpty(req.stationstaff.phonenumber)
                        ? req.stationstaff.phonenumber
                        : reader["phone_num"].ToString(),
                    plantname = !string.IsNullOrEmpty(req.stationstaff.plantname)
                        ? req.stationstaff.plantname
                        : reader["plant_name"].ToString()
                };
                //判断传入请求是否有null值，若有则不进行修改
                cmd.CommandText = "UPDATE staff " +
                                  $"SET staff_name='{info.name}',staff_password='{info.password}'," +
                                  $"phone_num='{info.phonenumber}',plant_name='{info.plantname}' " +
                                  $"WHERE staff_id='{info.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.updateMessage = "更改成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.updateMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 管理人员的更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Administrator")]
        [Authorize(Roles = "Administrator")]
        public UpdateResponse Update([FromBody] ADUpdateRequest req)
        {
            var resp = new UpdateResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM administrator " +
                                 $"WHERE adm_id='{req.administrator.id}'";
                OracleDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (DBConn.MD5Encrypt16(req.password) != reader["adm_password"].ToString())
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "密码错误";
                    DBConn.CloseConn(conn);
                    return resp;
                }

                var info = new Administrator
                {
                    id = reader["adm_id"].ToString(),
                    name = !string.IsNullOrEmpty(req.administrator.name)
                        ? req.administrator.name
                        : reader["adm_name"].ToString(),
                    password = !string.IsNullOrEmpty(req.administrator.password)
                        ? DBConn.MD5Encrypt16(req.administrator.password)
                        : reader["adm_password"].ToString(),
                    phonenumber = !string.IsNullOrEmpty(req.administrator.phonenumber)
                        ? req.administrator.phonenumber
                        : reader["phone_num"].ToString()
                };
                //判断传入请求是否有null值，若有则不进行修改
                cmd.CommandText = "UPDATE administrator " +
                                  $"SET adm_name='{info.name}',adm_password='{info.password}'," +
                                  $"phone_num='{info.phonenumber}' " +
                                  $"WHERE adm_id='{info.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.updateMessage = "更改成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.updateMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
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
        /// 注销普通用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteGarbageMan(string req="")
        {
            var resp = new DeleteResponse { status = Config.FAIL };
            if (req == "")
            {
                resp.status = Config.EMPTY;
                resp.deleteMessage = "输入为空";
                return resp;
            }
            if (req[0] != '1' && req[0] != '2' && req[0] != '3' && req[0] != '4' && req[0] != '5')
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = "用户类型错误";
                return resp;
            }
            OracleConnection conn = null;
            conn = DBConn.OpenConn();
            var cmd = conn.CreateCommand();
            cmd.CommandText = req[0] switch
            {
                '1' => "DELETE FROM alluser " + $"WHERE user_id='{req}'",
                '2' => "DELETE FROM watcher " + $"WHERE watcher_id='{req}'",
                '3' => "DELETE FROM carrier " + $"WHERE carrier_id='{req}'",
                '4' => "DELETE FROM carrier " + $"WHERE carrier_id='{req}'",
                _ => cmd.CommandText
            };
            try
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.deleteMessage = "删除成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /*
        /// <summary>
        /// 注销监察员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Watcher")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteWatcher([FromBody] DeleteRequest req)
        {
            var resp = new DeleteResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* FROM watcher " +
                                 $"WHERE watcher_id='{req.id}'";
                OracleDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (DBConn.MD5Encrypt16(req.password) != reader["watcher_password"].ToString())//判断密码是否正确
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "密码错误";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                cmd.CommandText = "DELETE FROM watcher " +
                                 $"WHERE watcher_id='{req.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.deleteMessage = "删除成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 注销运输人员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Carrier")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteCarrier([FromBody] DeleteRequest req)
        {
            var resp = new DeleteResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* FROM carrier " +
                                 $"WHERE carrier_id='{req.id}'";
                var reader = cmd.ExecuteReader();
                reader.Read();
                if (DBConn.MD5Encrypt16(req.password) != reader["carrier_password"].ToString())//判断密码是否正确
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "密码错误";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                cmd.CommandText = "DELETE FROM carrier " +
                                 $"WHERE carrier_id='{req.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.deleteMessage = "删除成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 注销垃圾处理站员工
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("StationStaff")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteStationStaff([FromBody] DeleteRequest req)
        {
            var resp = new DeleteResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* FROM staff " +
                                 $"WHERE staff_id='{req.id}'";
                var reader = cmd.ExecuteReader();
                reader.Read();
                if (DBConn.MD5Encrypt16(req.password) != reader["staff_password"].ToString())//判断密码是否正确
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "密码错误";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                cmd.CommandText = "DELETE FROM staff " +
                                 $"WHERE staff_id='{req.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.deleteMessage = "删除成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 注销管理员
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Administrator")]
        public DeleteResponse DeleteAdministrator([FromBody] DeleteRequest req)
        {
            var resp = new DeleteResponse { status = Config.FAIL };
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* FROM administrator " +
                                 $"WHERE adm_id='{req.id}'";
                var reader = cmd.ExecuteReader();
                reader.Read();
                if (DBConn.MD5Encrypt16(req.password) != reader["adm_password"].ToString())//判断密码是否正确
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "密码错误";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                cmd.CommandText = "DELETE FROM administrator " +
                                 $"WHERE adm_id='{req.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.deleteMessage = "删除成功";
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }*/
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
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("GarbageMan")]
        [Authorize(Roles = "Administrator,GarbageMan")]
        public GarbageMan GetGarbageMan(string req)
        {
            var resp = new GarbageMan();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM alluser " +
                                 $"WHERE user_id='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.id = reader["user_id"].ToString();
                    resp.name = reader["user_name"].ToString();
                    resp.phonenumber = reader["phone_num"].ToString();
                    resp.address = reader["address"].ToString();
                    resp.credit = Convert.ToInt32(reader["cre_points"]);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex);}
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 所有垃圾投递人员的信息查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllGarbageMan")]
        public List<GarbageMan> GetAllGarbageMan()
        {
            var respList = new List<GarbageMan>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM alluser";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new GarbageMan
                    {
                        id = reader["user_id"].ToString(),
                        name = reader["user_name"].ToString(),
                        phonenumber = reader["phone_num"].ToString(),
                        address = reader["address"].ToString(),
                        credit = Convert.ToInt32(reader["cre_points"])
                    };
                    respList.Add(resp);
                }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return respList;
        }
        /// <summary>
        /// 监察员信息的查看
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("Watcher")]
        [Authorize(Roles = "Administrator,Watcher")]
        public Watcher GetWatcher(string req)
        {
            var resp = new Watcher();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM watcher " +
                                 $"WHERE watcher_id='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.id = reader["watcher_id"].ToString();
                    resp.name = reader["watcher_name"].ToString();
                    resp.phonenumber = reader["phone_num"].ToString();
                    resp.address = reader["address"].ToString();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 所有监察员的信息查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllWatcher")]
        public List<Watcher> GetAllWatcher()
        {
            var respList = new List<Watcher>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM watcher";
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new Watcher
                    {
                        id = reader["watcher_id"].ToString(),
                        name = reader["watcher_name"].ToString(),
                        phonenumber = reader["phone_num"].ToString(),
                        address = reader["address"].ToString()
                    };
                    respList.Add(resp);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return respList;
        }
        /// <summary>
        /// 运输人员信息的查看 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("Carrier")]
        [Authorize(Roles = "Administrator,Carrier")]
        public Carrier GetCarrier(string req)
        {
            var resp = new Carrier();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM carrier " +
                                 $"WHERE carrier_id='{req}'";
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.id = reader["carrier_id"].ToString();
                    resp.name = reader["carrier_name"].ToString();
                    resp.phonenumber = reader["phone_num"].ToString();
                }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 所有运输人员的信息查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllCarrier")]
        public List<Carrier> GetAllCarrier()
        {
            var respList = new List<Carrier>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM carrier";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new Carrier
                    {
                        id = reader["carrier_id"].ToString(),
                        name = reader["carrier_name"].ToString(),
                        phonenumber = reader["phone_num"].ToString()
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return respList;
        }
        /// <summary>
        /// 垃圾处理站员工信息的查看
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("StationStaff")]
        [Authorize(Roles = "Administrator,StationStaff")]
        public StationStaff GetStationStaff(string req)
        {
            var resp = new StationStaff();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM staff " +
                                 $"WHERE staff_id='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.id = reader["staff_id"].ToString();
                    resp.name = reader["staff_name"].ToString();
                    resp.phonenumber = reader["phone_num"].ToString();
                    resp.plantname = reader["plant_name"].ToString();
                }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 所有垃圾处理站员工信息的查看
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllStationStaff")]
        public List<StationStaff> GetAllStationStaff()
        {
            var respList = new List<StationStaff>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM staff";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new StationStaff
                    {
                        id = reader["staff_id"].ToString(),
                        name = reader["staff_name"].ToString(),
                        phonenumber = reader["phone_num"].ToString(),
                        plantname = reader["plant_name"].ToString()
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return respList;
        }
        /// <summary>
        /// 垃圾管理员工信息的查看 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("Administrator")]
        [Authorize(Roles = "Administrator")]
        public Administrator GetAdministrator(string req)
        {
            var resp = new Administrator();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM administrator " +
                                 $"WHERE adm_id='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.id = reader["adm_id"].ToString();
                    resp.name = reader["adm_name"].ToString();
                    resp.phonenumber = reader["phone_num"].ToString();
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }

            return resp;
        }
    }

    /// <summary>
    /// 对watcher的工作安排，该类中的接口函数只能由Administrator访问
    /// </summary>
    [ApiController]
    [Route("User/[controller]/[action]")]
    public class DutyArrangeController : Controller
    {
        /// <summary>
        /// 增加工作安排
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        //[Authorize(Roles ="Administrator")]
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public AddResponse Add([FromBody] DutyArrange req)
        {
            var resp = new AddResponse();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO dutyarrange " +
                                 $"VALUES('{req.watcher_id}','{req.site_name}'," +
                                 $"'{req.start_time:yyyy/MM/dd HH:mm:ss}'," +
                                 $"'{req.end_time:yyyy/MM/dd HH:mm:ss}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.addMessage = "添加成功";
                DBConn.CloseConn(conn);
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.addMessage = ex.Message;
            }
            return resp;
        }
        /// <summary>
        /// 删除工作安排
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        //[Authorize(Roles ="Administrator")]
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse Delete([FromBody] DutyArrange req)
        {
            var resp = new DeleteResponse();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM dutyarrange " +
                                 $"WHERE watcher_id='{req.watcher_id}' AND site_name='{req.site_name}' AND start_time='{req.start_time}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到符合条件的指定行";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                resp.status = Config.SUCCESS;
                resp.deleteMessage = "删除成功";
                DBConn.CloseConn(conn);
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = ex.Message;
            }

            return resp;
        }
        /// <summary>
        /// 查找一个watcher的系列工作安排，输入为watcher_id，此接口可被watcher访问
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>     
        [HttpGet]
        [Authorize(Roles = "Administrator,Watcher")]
        public List<DutyArrange> Get(string req)
        {
            var resp = new List<DutyArrange>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* FROM dutyarrange " +
                                 $"WHERE watcher_id='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var da = new DutyArrange
                    {
                        watcher_id = reader["watcher_id"].ToString(),
                        site_name = reader["site_name"].ToString(),
                        start_time = reader["start_time"].ToString(),
                        end_time = reader["end_time"].ToString()
                    };
                    resp.Add(da);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return resp;
        }
    }
}