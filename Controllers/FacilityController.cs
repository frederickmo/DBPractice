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
using Oracle.ManagedDataAccess.Client;
using System.Web.Http.Cors;

namespace DBPractice.Controllers
{
    ///<summary>
    /// 垃圾桶相关的api接口
    /// 请注意垃圾桶的属性 condition是char类型的
    /// </summary>>
    [ApiController, Route("Facility/[controller]/[action]")]
    public class TrashCanController : Controller
    {
        /// <summary>　　
        /// 垃圾桶的添加
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public AddResponse Add([FromBody] TrashCan req)
        {
            var resp = new AddResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO dustbin " +
                                        $"VALUES('{req.id}','{req.type}','{req.condition}','{req.siteName}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.addMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 垃圾桶的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Roles="Administrator")]
        [HttpPost]
        public DeleteResponse Delete([FromBody] TrashCan req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM dustbin " +
                                        $"WHERE dustbin_id='{req.id}'";
                if(cmd.ExecuteNonQuery()==0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到指定ID的垃圾桶";
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
        /// 垃圾桶的状态更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Roles="Administrator,Watcher")]
        [HttpPost]
        public UpdateResponse Update([FromBody] TrashCan req)
        {
            var resp = new UpdateResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                if (!string.IsNullOrEmpty(req.siteName))
                {
                    cmd.CommandText = "UPDATE dustbin " +
                                        $"SET site_name='{req.siteName}'" +
                                        $"WHERE dustbin_id='{req.id}'";
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        resp.status = Config.FAIL;
                        resp.updateMessage = "未找到指定ID的垃圾桶";
                        DBConn.CloseConn(conn);
                        return resp;
                    }
                }
                if (req.condition != ' ')
                {
                    cmd.CommandText = "UPDATE dustbin " +
                                        $"SET condition='{req.condition}'" +
                                        $"WHERE dustbin_id='{req.id}'";                    
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
        /// 获取某一个垃圾桶的属性
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Roles="Administrator,Watcher")]
        [HttpGet]
        public TrashCan Get(string req)
        {
            var resp = new TrashCan();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM dustbin " +
                                 $"WHERE dustbin_id='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.id = reader["dustbin_id"].ToString();
                    resp.type = reader["dustbin_type"].ToString();
                    resp.condition = Convert.ToChar(reader["condition"].ToString() ?? throw new InvalidOperationException());
                    resp.siteName = reader["site_name"].ToString();
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return resp;
        }
        /// <summary>
        /// 获取指定垃圾站所有垃圾桶一个列表
        /// </summary>
        /// <returns></returns>       
        [HttpGet]
        [Authorize(Roles = "Administrator,Watcher")]
        public List<TrashCan> GetSiteAll(string req)
        {
            var respList = new List<TrashCan>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM dustbin " +
                                 $"WHERE site_name='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new TrashCan
                    {
                        id = reader["dustbin_id"].ToString(),
                        type = reader["dustbin_type"].ToString(),
                        condition = Convert.ToChar(reader["condition"].ToString() ?? throw new InvalidOperationException()),
                        siteName = reader["site_name"].ToString()
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return respList;
        }
        /// <summary>
        /// 获取所有垃圾桶一个列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<TrashCan> GetAll()
        {
            var respList = new List<TrashCan>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM dustbin ";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new TrashCan
                    {
                        id = reader["dustbin_id"].ToString(),
                        type = reader["dustbin_type"].ToString(),
                        condition = Convert.ToChar(reader["condition"].ToString() ??
                                                   throw new InvalidOperationException()),
                        siteName = reader["site_name"].ToString()
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return respList;
        }
    }
    /// <summary>
    /// 垃圾站
    /// </summary>
    [ApiController, Route("Facility/[controller]/[action]")]
    public class BinSiteController : Controller
    {
        /// <summary>
        /// 垃圾站的添加
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public AddResponse Add([FromBody] BinSite req)
        {
            var resp = new AddResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO binsite " +
                                  $"VALUES('{req.name}','{req.location}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.addMessage = ex.Message;
            }

            DBConn.CloseConn(conn);
            return resp;
        }

        /// <summary>
        /// 垃圾站的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse Delete([FromBody] BinSite req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM binsite " +
                                  $"WHERE site_name='{req.name}'";

                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到指定ID";
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
        /// 垃圾站的属性更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public UpdateResponse Update([FromBody] BinSite req)
        {
            var resp = new UpdateResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE binsite " + $"SET bin_location='{req.location}'" +
                                 $"WHERE site_name='{req.name}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到指定ID";
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
        /// 获取一个垃圾站的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public BinSite Get(string req)
        {
            var resp = new BinSite();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM binsite " +
                                  $"WHERE site_name='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.name = reader["site_name"].ToString();
                    resp.location = reader["bin_location"].ToString();
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return resp;
        }
        /// <summary>
        /// 获取所有垃圾站的一个列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<BinSite> GetAll()
        {
            var respList = new List<BinSite>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM binsite";
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new BinSite
                    {
                        name = reader["site_name"].ToString(), location = reader["bin_location"].ToString()
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return respList;
        }
    }
    /// <summary>
    /// 垃圾车
    /// </summary>
    [ApiController, Route("Facility/[controller]/[action]")]
    public class TruckController : Controller
    {
        /// <summary>
        /// 垃圾车的添加
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public AddResponse Add([FromBody] Truck req)
        {
            var resp = new AddResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO truck " +
                                        $"VALUES('{req.id}','{req.condition}','{req.carrierID}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.addMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 垃圾车的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse Delete([FromBody] Truck req)
        {
            {
                var resp = new DeleteResponse();
                OracleConnection conn = null;
                try
                {
                    conn = DBConn.OpenConn();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM truck " +
                                            $"WHERE truck_id ='{req.id}'";
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        resp.status = Config.FAIL;
                        resp.deleteMessage = "未找到指定ID";
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
        }
        /// <summary>
        /// 垃圾车的状态更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator,Carrier")]
        public UpdateResponse Update([FromBody] Truck req)
        {
            var resp = new UpdateResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE truck " + $"SET condition ='{req.condition}'" +
                                  $"WHERE truck_id ='{req.id}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到指定ID";
                    DBConn.CloseConn(conn);
                    return resp;
                }
                if (!string.IsNullOrEmpty(req.carrierID))
                {
                    cmd.CommandText = "UPDATE truck " + $"SET carrier_id ='{req.carrierID}'" +
                    $"WHERE truck_id ='{req.id}'";
                    cmd.ExecuteNonQuery();
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
        /// 获取某一个垃圾车的属性
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,Carrier")]
        public Truck Get(string req)
        {
            var resp = new Truck();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM truck " +
                                 $"WHERE truck_id='{req}'";
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.id = reader["truck_id"].ToString();
                    resp.condition = Convert.ToChar(reader["condition"].ToString() ?? throw new InvalidOperationException());
                    resp.carrierID = reader["carrier_id"].ToString();
                }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return resp;
        }
        /// <summary>
        /// 获取所有垃圾车的一个列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,Carrier")]
        public List<Truck> GetAll()
        {
            var respList = new List<Truck>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM truck";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new Truck
                    {
                        id = reader["truck_id"].ToString(),
                        condition = Convert.ToChar(reader["condition"].ToString() ?? throw new InvalidOperationException()),
                        carrierID = reader["carrier_id"].ToString()
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return respList;
        }
    }
    /// <summary>
    /// 垃圾处理站
    /// </summary>
    [ApiController, Route("Facility/[controller]/[action]")]
    public class PlantController : Controller
    {
        /// <summary>
        /// 垃圾处理站的添加
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public AddResponse Add([FromBody] Plant req)
        {
            var resp = new AddResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO plant " +
                                        $"VALUES('{req.name}','{req.address}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.addMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 垃圾处理站的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse Delete([FromBody] Plant req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM plant " +
                                        $"WHERE plant_name ='{req.name}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "未找到指定ID";
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
        /// 垃圾处理站的更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public UpdateResponse Update([FromBody] Plant req)
        {
            var resp = new UpdateResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE plant " + $"SET address ='{req.address}' " +
                $"WHERE plant_name ='{req.name}'";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "未找到指定ID";
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
        /// 获取某一个垃圾处理站的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,StationStaff")]
        public Plant Get(string req)
        {
            var resp = new Plant();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM plant " +
                                 $"WHERE plant_name='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.name = reader["plant_name"].ToString();
                    resp.address = reader["address"].ToString();
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return resp;
        }
        /// <summary>
        /// 获取所有垃圾处理站的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public List<Plant> GetAll()
        {
            var respList = new List<Plant>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM plant";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new Plant
                    {
                        name = reader["plant_name"].ToString(), address = reader["address"].ToString()
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            return respList;
        }
    }
}