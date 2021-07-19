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
using System.Security.Cryptography;
using Oracle.ManagedDataAccess.Client;
using System.Collections;
using System.Linq.Expressions;

namespace DBPractice.Controllers
{

    /// <summary>
    /// 垃圾有关的api接口
    /// </summary>
    [ApiController, Route("[controller]/[action]")]
    public class GarbageController : Controller
    {
        private string GenerateGarbageID()
        {
            string gar_id = null;
            var dt = DateTime.Now;
            gar_id = $"{dt:MMddHHmmss}";           
            return gar_id;
        }
        /*
         * 垃圾分为可回收垃圾、干垃圾、湿垃圾、有害垃圾，可回收垃圾须装入一个袋中并贴上一个条形码
         * 利用可回收垃圾的条形码可以读出投放人的id
         * 只有可回收垃圾需要更新垃圾处理的结果信息，此更新由stationStaff完成
         */
        /// <summary>
        /// 添加垃圾，由Watcher提供信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Roles = "GarbageMan")]
        [HttpPost]            
        public AddResponse Add([FromBody] Garbage req)
        {
            var resp = new AddResponse();
            req.gar_id = GenerateGarbageID();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO garbage " +
                                        $"VALUES('{req.gar_id}','{req.type}','{req.user_id}','{DateTime.Now:yyyy-MM-dd hh:mm:ss}')";//U表示未处理
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
        /// 投递垃圾
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Watcher")]
        [HttpPost]
        public Response Throw([FromBody] ThrowRequest req)
        {
            var resp = new Response();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO THROW " +
                                  $"VALUES('{req.gid}','{req.bid}','{DateTime.Now:yyyy-MM-dd hh:mm:ss}')";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 垃圾状态的更新，由StationStaff来提供垃圾处理结果信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost]//[ Authorize(Roles = "StationStaff")]
        [Authorize(Roles = "StationStaff")]
        public UpdateResponse Update(string id,int result)
        {

            var resp = new UpdateResponse();
            if (result != 5&&result!=6)
            {
                resp.status = Config.FAIL;
                resp.updateMessage = "更改错误";
            }
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE garbage " + $"SET gar_result ='{result}'," + $"finish_time = '{DateTime.Now:yyyy/MM/dd HH:mm:ss}' "
                                  + $"WHERE gar_id ='{id}'";
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
        /// 删除对应编号的垃圾
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]//
        [Authorize(Roles = "Administrator")]
        public DeleteResponse Delete(string req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM garbage " +
                                        $"WHERE gar_id ='{req}'";
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
        /// 为了防止数据库垃圾记录过多，管理人员可定期清理早于某个时间的记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]//[Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteOldRecord(DateTime req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM garbage " +
                                        $"WHERE finish_time<='{req:yyyy-MM-dd hh:mm:ss}'";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.deleteMessage = ex.Message;
            }
            DBConn.CloseConn(conn);
            return resp;
        }

        public struct GarLife
        {
            public string gar_id { get; set; }
            public string type { get; set; }
            public string dustbin_id { get; set; }
            public string plant_name { get; set; }
            public string user_id { get; set; }
            public DateTime latest_time { get; set; }
            public int status { get; set; }
        }
        /// <summary>
        /// 以垃圾编号获取某一个垃圾投递的记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public GarLife Get(string req)
        {
            var resp = new GarLife();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "from GARBAGE NATURAL join THROW " +
                                 $"WHERE gar_id='{req}'"; 
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.gar_id = reader.GetString(0);
                    resp.type = reader.GetString(1);
                    resp.plant_name = reader.GetString(3);
                    resp.dustbin_id = reader.GetString(4);
                    resp.user_id = reader.GetString(5);
                    resp.finish_time = reader.GetDateTime(6);
                }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 以投放人的id获取垃圾投放记录列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,GarbageMan")]
        public List<Garbage> GetAll(string req)
        {
            var respList = new List<Garbage>();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM garbage " + $"WHERE user_id ='{req}'" +                               
                                 $"ORDER BY finish_time DESC";//以时间进行排序 
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new Garbage
                    {
                        gar_id = reader.GetString(0),
                        type = reader.GetString(1),
                        gar_result = reader.GetInt32(2),
                        plant_name = reader.GetString(3),
                        dustbin_id = reader.GetString(4),
                        user_id = reader.GetString(5),
                        finish_time = reader.GetDateTime(6)
                    };
                    respList.Add(resp);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            DBConn.CloseConn(conn);
            return respList;
        }
    }
    

    ///<summary>
    ///有关所有垃圾运输的api
    ///请注意垃圾只有结束运输的时候才写入数据库，否则放在缓存now
    ///</summary>
    [ApiController, Route("[controller]/[action]")]
    public class TransportController : Controller
    {
        struct Start
        {
            public string truck_id { get; set; }//垃圾车编号
            public DateTime start_time { get; set; }
            public string dustbin_id { get; set; }
            public string carrier_id { get; set; }
        };
        private static List<Start> now = new();//正在运送的还没写进数据库的。

        private Transport _newT = new Transport();
        /*
         * 这个是由运输员开始的，然后直接入now
         */
        /// <summary>
        /// 垃圾开始运输的行为
        /// 开始运输的行为会储存到缓存中，垃圾状态更新为运输中
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]//
        [Authorize(Roles = "Carrier")]
        public Response TransportStart([FromBody] TransportStart req)
        {
            var resp = new Response {status = Config.FAIL};
            try
            {
                 if(now.FindIndex((Start x) => x.carrier_id == HttpContext.User.Identity.Name&&x.dustbin_id==req.dustbin_id)!=-1)
                    return resp;
            }
            catch (Exception)
            {
                // ignored
            }
            var newT = new Start{truck_id = req.truck_id, start_time = DateTime.Now, dustbin_id = req.dustbin_id,carrier_id = HttpContext.User.Identity.Name};
            now.Add(newT);
            resp.status = Config.SUCCESS;
            return resp;
        }

        /// <summary>
        /// 垃圾结束运输到达垃圾处理站
        /// 从缓存中取出相应的开始运输记录并结合写入数据库
        /// trigger触发更新垃圾状态为到达垃圾处理站
        /// 只能由垃圾处理站人员访问本接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost] 
        [Authorize(Roles = "StationStaff")]
        public Response TransportEnd([FromBody] TransportEnd req)
        {
            var resp = new Response {status = Config.SUCCESS};
            var newT = new Transport();
            var conn = DBConn.OpenConn();
            try
            {
                var records = now.FindAll((Start x) => x.truck_id == req.truck_id);
                if (records.Count == 0)
                {
                    resp.status = Config.FAIL;
                    Console.WriteLine(records.Count);
                    return resp;
                }
                var cmd = conn.CreateCommand();
                foreach (var record in records)
                {
                    cmd.CommandText = "SELECT* FROM " +
                                      $"dustbin NATURAL JOIN garbage " +
                                      $"WHERE dustbin_id='{record.dustbin_id}'";
                    var reader = cmd.ExecuteReader();
                    try
                    {
                        var m_OraTrans = conn.BeginTransaction(); //创建事务对象
                        while (reader.Read())
                        {
                            var temp = new Transport
                            {
                                gar_id = reader["gar_id"].ToString(),
                                end_time = DateTime.Now,
                                plant_name = req.plant_name,
                                dustbin_id = record.dustbin_id,
                                start_time = record.start_time,
                                truck_id =record.truck_id
                            };
                            cmd.CommandText = "INSERT INTO transport " +
                                              $"VALUES('{temp.gar_id}','{temp.truck_id}','{temp.start_time:yyyy/MM/dd HH:mm:ss}'," +
                                              $"'{temp.end_time:yyyy/MM/dd HH:mm:ss}'," +
                                              $"'{temp.plant_name}','{temp.dustbin_id}')";
                            Console.WriteLine("INSERT INTO transport " +
                                              $"VALUES('{temp.gar_id}','{temp.truck_id}','{temp.start_time:yyyy/MM/dd HH:mm:ss}'," +
                                              $"'{temp.end_time:yyyy/MM/dd HH:mm:ss}'," +
                                              $"'{temp.plant_name}','{temp.dustbin_id}')");
                            cmd.ExecuteNonQuery();
                        }
                        m_OraTrans.Commit();
                        now.Remove(record);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        //m_OraTrans.Rollback();
                        resp.status = Config.FAIL;
                    }

                }
            }
            catch (Exception)
            {
                resp.status = Config.FAIL;
                DBConn.CloseConn(conn);
                return resp;
            }
            DBConn.CloseConn(conn);
            return resp;
        }

        /// <summary>
        /// 以垃圾编号获取对应垃圾运输记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public Transport Get(string req)
        {
            var resp = new Transport();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM transport " +
                                 $"WHERE gar_id='{req}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resp.gar_id = reader["gar_id"].ToString();
                    resp.plant_name = reader["plant_name"].ToString();
                    resp.dustbin_id = reader["dustbin_id"].ToString();
                    resp.truck_id = reader["truck_id"].ToString();
                    resp.start_time = Convert.ToDateTime(reader["start_time"].ToString());
                    resp.end_time = Convert.ToDateTime(reader["end_time"].ToString());
                }   
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return resp;
        }
        /// <summary>
        /// 以垃圾站名字获取垃圾运输记录
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "administrator")]
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public List<Transport> GetAll(string req)
        {
            var resp = new List<Transport>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM transport " +
                                 $"WHERE plant_name='{req}'" +                                
                                 $"ORDER BY end_time DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var t = new Transport
                    {
                        gar_id = reader["gar_id"].ToString(),
                        plant_name = reader["plant_name"].ToString(),
                        dustbin_id = reader["dustbin_id"].ToString(),
                        truck_id = reader["truck_id"].ToString(),
                        start_time = Convert.ToDateTime(reader["start_time"].ToString()),
                        end_time = Convert.ToDateTime(reader["end_time"].ToString())
                    };
                    resp.Add(t);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) {Console.WriteLine(ex.Message);}
            return resp;
        }
    }
    /// <summary>
    /// 有关违规记录处理的API
    /// </summary>
    [ApiController, Route("[controller]/[action]")]
    public class ViolateRecordController : Controller
    {
        /// <summary>
        /// 增加违规记录，同时对应修改用户积分credit，这里用事务保证一致性
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]//Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Watcher")]
        public AddResponse Add([FromBody] ViolateRecord req)
        {
            var resp = new AddResponse();
            OracleConnection conn = null;
            conn = DBConn.OpenConn();
            OracleTransaction m_OraTrans = conn.BeginTransaction();//创建事务对象
            var cmd = conn.CreateCommand();
            var dt = DateTime.Now;
            try
            {
                cmd.CommandText = "INSERT INTO violate_record " +
                                 $"VALUES('{req.user_id}','{req.watcher_id}','{req.reason}',{req.punishment}," +
                                 $"'{dt:yyyy-MM-dd hh:mm:ss}')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE alluser " +
                                 $"SET cre_points=cre_points-{req.punishment} " +
                                 $"WHERE user_id ='{req.user_id}'";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                m_OraTrans.Commit();
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                resp.addMessage = ex.Message;
                m_OraTrans.Rollback();
            }
            DBConn.CloseConn(conn);
            return resp;
        }
        /// <summary>
        /// 修改违规记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost] //Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Administrator,Watcher")]
        public UpdateResponse Update([FromBody] ViolateRecord req)
        {
            var resp = new UpdateResponse();           
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE violate_record " +
                                 $"SET user_id='{req.user_id}',watcher_id='{req.watcher_id}',reason='{req.reason}',punishment='{req.punishment}' " +                                
                                 $"WHERE user_id='{req.user_id}' AND watcher_id='{req.watcher_id}' AND violate_time='{req.violate_time:yyyy-MM-dd hh-mm-ss}'";
                var k = cmd.ExecuteNonQuery();
                if(k==1)
                {
                    resp.status = Config.SUCCESS;
                    resp.updateMessage = "更新成功";
                }
                else
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "更新失败，未找到指定记录";
                }               
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
        /// 删除违规记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost] //Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse Delete([FromBody] ViolateRecord req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM violate_record " +
                                  $"WHERE user_id='{req.user_id}' AND watcher_id='{req.watcher_id}' AND violate_time='{req.violate_time:yyyy-MM-dd hh-mm-ss}'";           
                int k = cmd.ExecuteNonQuery();
                if (k == 1)
                {
                    resp.status = Config.SUCCESS;
                    resp.deleteMessage = "删除成功";
                }
                else
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "删除失败，未找到指定记录";
                }
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
        /// 为了防止数据库垃圾记录过多，管理人员可定期清理那些存放时间很长的垃圾记录或违规记录
        /// </summary>
        /// <returns></returns>
        [HttpPost] //, Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteOldRecord(DateTime req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM violate_record " +
                                 $"WHERE violate_time<='{req:yyyy-MM-dd hh-mm-dd}'";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
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
        /// 得到对应投放人编号的违规记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "GarbageMan,Administrator")]
        public List<ViolateRecord> Get(string req)
        {
            var respList = new List<ViolateRecord>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM violate_record " +
                                 $"WHERE user_id='{req}'" +
                                 $"ORDER BY violate_time DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new ViolateRecord
                    {
                        user_id = reader["user_id"].ToString(),
                        watcher_id = reader["watcher_id"].ToString(),
                        reason = reader["reason"].ToString(),
                        punishment = Convert.ToInt32(reader["punishment"].ToString())
                    };
                    respList.Add(resp);
                }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return respList;
        }
    }

        /// <summary>
        /// 有关Carrier和StationStaff交互记录的API
        /// </summary>
        [ApiController, Route("[controller]/[action]")]
        public class InteractController : Controller
        {
            /// <summary>
            /// 增加交互记录
            /// </summary>
            /// <param name="req"></param>
            /// <returns></returns>
            [HttpPost, Authorize(Roles = "StationStaff")]
            public AddResponse Add([FromBody] Interact req)
            {
                var resp = new AddResponse();
                OracleConnection conn = null;
                try
                {
                DateTime dt = DateTime.Now;
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO interact " +
                                     $"VALUES('{req.carrier_id}','{req.staff_id}','{dt:yyyy/MM/dd HH:mm:ss}','{req.interact_result}')";
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
        /// 修改交互记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Roles = "Administrator,StationStaff")]
        public UpdateResponse Update([FromBody] Interact req)
        {
            var resp = new UpdateResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE interact " +
                                 $"SET interact_result='{req.interact_result}' " +
                                 $"WHERE carrier_id='{req.carrier_id}' AND staff_id='{req.staff_id}' AND interact_time='{req.interact_time:yyyy/MM/dd HH:mm:ss}'";
                var k = cmd.ExecuteNonQuery();
                if (k == 1)
                {
                    resp.status = Config.SUCCESS;
                    resp.updateMessage = "更新成功";
                }
                else
                {
                    resp.status = Config.FAIL;
                    resp.updateMessage = "更新失败，未找到指定记录";
                }
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
        /// 删除交互记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Roles = "Administrator")]
        public DeleteResponse Delete([FromBody] Interact req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM interact " +
                                 $"WHERE carrier_id ='{req.carrier_id}' AND staff_id ='{req.staff_id}' AND interact_time='{req.interact_time}'";
                var k = cmd.ExecuteNonQuery();
                if (k == 1)
                {
                    resp.status = Config.SUCCESS;
                    resp.deleteMessage = "删除成功";
                }
                else
                {
                    resp.status = Config.FAIL;
                    resp.deleteMessage = "删除失败，未找到指定记录";
                }
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
        /// 防止数据库垃圾记录过多,管理人员可清理某个时间之前的所有交互记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]//[ Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteOldRecord(DueTime req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                string month = "00", day = "00", hour = "00", minute = "00", second = "00";
                if (req.month is > 0 and < 10) month = '0' + req.month.ToString();
                else month = req.month.ToString();
                if (req.day is > 0 and < 10) day = '0' + req.day.ToString();
                else day = req.day.ToString();
                if (req.hour is >= 0 and < 10) hour = '0' + req.hour.ToString();
                else hour = req.hour.ToString();
                if (req.minute is >= 0 and < 10) minute = '0' + req.minute.ToString();
                else minute = req.minute.ToString();
                if (req.second is >= 0 and < 10) second = '0' + req.second.ToString();
                else second = req.second.ToString();
                var time = req.year + "/" + month + "/" + day + ' ' + hour + ':' + minute + ':' + second;
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM interact " +
                                 $"WHERE interact_time<='{time}'";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
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
        /// 得到对应处理人员编号的交互记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,StationStaff")]
        public List<Interact> Get(string req)
        {
            var respList = new List<Interact>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT* " +
                                  "FROM interact " +
                                 $"WHERE staff_id='{req}'" +
                                 $"ORDER BY interact_time DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new Interact
                    {
                        carrier_id = reader["carrier_id"].ToString(),
                        staff_id = reader["staff_id"].ToString(),
                        interact_time = reader["interact_time"].ToString(),
                        interact_result = Convert.ToChar(reader["interact_result"].ToString() ?? string.Empty)
                    };
                    respList.Add(resp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return respList;
        }
    }
}