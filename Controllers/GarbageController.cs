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
using System.Data;
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
            gar_id = $"{dt:yyMMddHHmmss}";
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
        [HttpGet]
        public AddResponse Add(string req)
        {
            var resp = new AddResponse();
            var gar_id = HttpContext.User.Identity.Name+GenerateGarbageID();
            OracleConnection conn = null;
            try
            {
                //INSERT INTO "C##PDCR"."GARBAGE" ("GAR_ID", "GAR_TYPE", "USER_ID", "CREATE_TIME", "TRANS_ID") VALUES ('1234567', '干垃圾', '1952108', TO_DATE('2021-07-19 14:25:55', 'SYYYY-MM-DD HH24:MI:SS'), '11111113');
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO garbage " +
                                  $"VALUES('{gar_id}','{req}','{HttpContext.User.Identity.Name}',TO_DATE('{DateTime.Now:yyyy-MM-dd HH:mm:ss}','yyyy-MM-dd hh24-mi-ss'),null)"; //U表示未处理
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
                                  $"VALUES('{req.gid}','{req.bid}','{DateTime.Now:yyyy-MM-dd HH:mm:ss}')";
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
        [HttpPost] //[ Authorize(Roles = "StationStaff")]
        [Authorize(Roles = "StationStaff")]
        public UpdateResponse Update(string id, int result)
        {
            var resp = new UpdateResponse();
            if (result != 5 && result != 6)
            {
                resp.status = Config.FAIL;
                resp.updateMessage = "更改错误";
            }

            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE garbage " + $"SET gar_result ='{result}'," +
                                  $"finish_time = '{DateTime.Now:yyyy/MM/dd HH:mm:ss}' "
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
        [HttpPost] //
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
        [HttpPost] //[Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Administrator")]
        public DeleteResponse DeleteOldRecord(DateTime req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn(); //TO_DATE('2021-09-01 00:00:00','yyyy-mm-dd hh24:mi:ss')
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM garbage " +
                                  $"WHERE CREATE_TIME<=TO_DATE('{req:yyyy-MM-dd HH:mm:ss}','yyyy-mm-dd hh24:mi:ss')";
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
        /// 一个垃圾运输的声明周期 状态说明：0是已经申请，1是已入桶，2是运输中，3是到达处理站
        /// </summary>
        public struct GarLife
        {
            public string gar_id { get; set; }
            public string type { get; set; }
            public string dustbin_id { get; set; }
            public string plant_name { get; set; }
            public string user_id { get; set; }
            public string truck_id { get; set; }
            public DateTime latest_time { get; set; }
            public int status { get; set; } //0是已经申请，1是已入桶，2是运输中，3是到达处理站
        }

        /// <summary>
        /// 以垃圾编号获取某一个垃圾投递的记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public GarLife Get(string req)
        {
            var resp = new GarLife {status = Config.FAIL};
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    "SELECT " +
                    "USER_ID,GARBAGE.GAR_ID,GARBAGE.GAR_TYPE,CREATE_TIME," + //垃圾创建状态截止 3
                    "THROW.DUSTBIN_ID,THROW_TIME," + //垃圾投放状态截止 5
                    "GARBAGE.TRANS_ID,TRUCK_ID,START_TIME,END_TIME,TRANSPORT.PLANT_NAME " + //垃圾运输状态截止 8,9
                    "FROM garbage " +
                    "LEFT JOIN THROW ON garbage.GAR_ID=THROW.GAR_ID " +
                    "LEFT JOIN TRANSPORT ON TRANSPORT.TRANS_ID=GARBAGE.TRANS_ID " +
                    $"WHERE GARBAGE.GAR_ID='{req}'"; //按照垃圾编号查找
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    resp.user_id = reader.GetString(0);
                    resp.gar_id = reader.GetString(1);
                    resp.type = reader.GetString(2);
                    resp.latest_time = reader.GetDateTime(3);
                    resp.status = 0;
                    if (reader.IsDBNull(5) == false)
                    {
                        resp.dustbin_id = reader.GetString(4);
                        resp.latest_time = reader.GetDateTime(5);
                        resp.status = 1;
                    }

                    if (reader.IsDBNull(8) == false)
                    {
                        resp.truck_id = reader.GetString(7);
                        resp.latest_time = reader.GetDateTime(8);
                        resp.status = 2;
                    }

                    if (reader.IsDBNull(9) == false)
                    {
                        resp.latest_time = reader.GetDateTime(9);
                        resp.plant_name = reader.GetString(10);
                        resp.status = 3;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
        public List<GarLife> GetAll(string req)
        {
            var respList = new List<GarLife>();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT " +
                                  "USER_ID,GARBAGE.GAR_ID,GARBAGE.GAR_TYPE,CREATE_TIME," + //垃圾创建状态截止 3
                                  "THROW.DUSTBIN_ID,THROW_TIME," + //垃圾投放状态截止 5
                                  "GARBAGE.TRANS_ID,TRUCK_ID,START_TIME,END_TIME,TRANSPORT.PLANT_NAME " + //垃圾运输状态截止 8,9
                                  "FROM garbage " +
                                  "LEFT JOIN THROW ON garbage.GAR_ID=THROW.GAR_ID " +
                                  "LEFT JOIN TRANSPORT ON TRANSPORT.TRANS_ID=GARBAGE.TRANS_ID " +
                                  $"WHERE GARBAGE.USER_ID='{req}'"; //按照投递人编号查找
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new GarLife
                    {
                        user_id = reader.GetString(0),
                        gar_id = reader.GetString(1),
                        type = reader.GetString(2),
                        latest_time = reader.GetDateTime(3),
                        status = 0
                    };
                    if (reader.IsDBNull(5) == false)
                    {
                        resp.dustbin_id = reader.GetString(4);
                        resp.latest_time = reader.GetDateTime(5);
                        resp.status = 1;
                    }

                    if (reader.IsDBNull(8) == false)
                    {
                        resp.truck_id = reader.GetString(7);
                        resp.latest_time = reader.GetDateTime(8);
                        resp.status = 2;
                    }

                    if (reader.IsDBNull(9) == false)
                    {
                        resp.latest_time = reader.GetDateTime(9);
                        resp.plant_name = reader.GetString(10);
                        resp.status = 3;
                    }

                    respList.Add(resp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            DBConn.CloseConn(conn);
            return respList;
        }
    }

/// <summary>
    /// 垃圾投递管理
    /// </summary>
    [ApiController, Route("[controller]/[action]")]
    public class ThrowController : Controller
    {
        /*
         * INSERT INTO "C##PDCR"."THROW" ("GAR_ID", "DUSTBIN_ID", "THROW_TIME") VALUES ('09999999', 'D001', TO_DATE('2021-07-16 19:18:47', 'SYYYY-MM-DD HH24:MI:SS'));
         */
        /// <summary>
        /// 投递垃圾
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Watcher")]
        [HttpPost]
        public AddResponse Add([FromBody] ThrowRequest req)
        {
            var resp = new AddResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO THROW " +
                                  $"VALUES('{req.gid}','{req.bid}',TO_DATE('{DateTime.Now:yyyy-MM-dd hh:mm:ss}','yyyy-mm-dd hh24:mi:ss'))";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.addMessage = "投递成功";
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
        /// 撤回投递记录，投递记录不能改，只能删.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Roles = "Watcher,Administrator")]
        [HttpGet]
        public DeleteResponse Delete(string req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM THROW WHERE GAR_ID='{req}'";
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
                resp.deleteMessage = "撤回成功";
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
        /// 投递记录
        /// </summary>
        public struct ThrowRecord
        {
            public string user_id { get; set; }
            public string gar_id { get; set; }
            public string gar_type { get; set; }
            public string dustbin_id { get; set; }
            public DateTime throw_time { get; set; }
        };
        
        /// <summary>
        /// 根据watcher_id取投递记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,Watcher")]
        public List<ThrowRecord> GetThrowRecord(string req)
        {
            var resp = new List<ThrowRecord>();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT " +
                                  "USER_ID,GAR_ID,GAR_TYPE," +
                                  "DUSTBIN_ID,THROW_TIME " +
                                  "FROM GARBAGE NATURAL JOIN THROW NATURAL JOIN DUSTBIN " +
                                  $"WHERE SITE_NAME='{req}'";//按照垃圾站编号查找
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var temp = new ThrowRecord()
                    {
                        user_id = reader.GetString(0),
                        gar_id = reader.GetString(1),
                        gar_type = reader.GetString(2),
                        dustbin_id = reader.GetString(3),
                        throw_time = reader.GetDateTime(4)
                    };
                    resp.Add(temp);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message);}
            DBConn.CloseConn(conn);
            return resp;
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
            public string trans_id { get; set; }
            public string truck_id { get; set; } //垃圾车编号
            public string dustbin_id { get; set; }
            
        };

        private static List<Start> now = new(); //正在运送的还没写进数据库的。

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
        [HttpPost]
        [Authorize(Roles = "Carrier")]
        public Response TransportStart([FromBody] TransportStart req)
        {
            
            //INSERT INTO "C##PDCR"."TRANSPORT" ("TRUCK_ID", "START_TIME", "END_TIME", "PLANT_NAME", "DUSTBIN_ID", "TRANS_ID", "CARRIER_ID") VALUES ('T01', TO_DATE('2021-07-18 19:20:20', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2021-07-18 19:20:28', 'SYYYY-MM-DD HH24:MI:SS'), 'P01', 'D001', '11111111', '3952108');
            var resp = new Response {status = Config.FAIL};
            OracleConnection conn = null;
            try
            {
                string trans_id=HttpContext.User.Identity.Name+$"{DateTime.Now:yyMMddHHmmss}";
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO TRANSPORT " +" (TRUCK_ID, START_TIME, END_TIME, PLANT_NAME, DUSTBIN_ID, TRANS_ID, CARRIER_ID) " +
                                  $"VALUES('{req.truck_id}',TO_DATE('{DateTime.Now:yyyy-MM-dd HH:mm:ss}','SYYYY-MM-DD HH24:MI:SS'),null,null,'{req.dustbin_id}','{trans_id}','{HttpContext.User.Identity.Name}')"; 
                cmd.ExecuteNonQuery();
                resp.status = Config.SUCCESS;
            }
            catch (Exception ex)
            {
                resp.status = Config.FAIL;
                Console.WriteLine(ex.Message);
            }
            DBConn.CloseConn(conn);
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
            var conn = DBConn.OpenConn();
            var cmd = conn.CreateCommand();
            DateTime endTime = DateTime.Now;
            try
            {
                cmd.CommandText = "UPDATE TRANSPORT SET " +
                                  $"END_TIME=TO_DATE('{endTime:yyyy-MM-dd HH:mm:ss}','yyyy-mm-dd hh24:mi:ss'),PLANT_NAME='{req.plant_name}' WHERE END_TIME is null AND TRUCK_ID='{req.truck_id}'";
                Console.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //m_OraTrans.Rollback();
                resp.status = Config.FAIL;
            }

            DBConn.CloseConn(conn);
            return resp;
        }

        [HttpGet]
        [Authorize(Roles = "StationStaff")]
        public Response TransportEnd(string req)
        {
            var resp = new Response {status = Config.SUCCESS};
            var conn = DBConn.OpenConn();
            var cmd = conn.CreateCommand();
            DateTime endTime = DateTime.Now;
            try
            {
                cmd.CommandText = "UPDATE TRANSPORT SET " +
                                  $"END_TIME=TO_DATE('{endTime:yyyy-MM-dd HH:mm:ss}','yyyy-mm-dd hh24:mi:ss')," +
                                  $"PLANT_NAME= (SELECT PLANT_NAME FROM STAFF WHERE STAFF_ID='{HttpContext.User.Identity.Name}')" +
                                  $"WHERE TRANS_ID='{req}'";
                Console.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //m_OraTrans.Rollback();
                resp.status = Config.FAIL;
            }

            DBConn.CloseConn(conn);
            return resp;
        }
        
        // /// <summary>
        // /// 以垃圾编号获取对应垃圾运输记录
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet]
        // [Authorize(Roles = "Administrator")]
        // public Transport Get(string req)
        // {
        // var resp = new Transport();
        //     try
        //
        // {
        //     var conn = DBConn.OpenConn();
        //     var cmd = conn.CreateCommand();
        //     cmd.CommandText = "SELECT* " +
        //                       "FROM transport " +
        //                       $"WHERE gar_id='{req}'";
        //     var reader = cmd.ExecuteReader();
        //     while (reader.Read())
        //     {
        //         resp.gar_id = reader["gar_id"].ToString();
        //         resp.plant_name = reader["plant_name"].ToString();
        //         resp.dustbin_id = reader["dustbin_id"].ToString();
        //         resp.truck_id = reader["truck_id"].ToString();
        //         resp.start_time = Convert.ToDateTime(reader["start_time"].ToString());
        //         resp.end_time = Convert.ToDateTime(reader["end_time"].ToString());
        //     }
        //
        //     DBConn.CloseConn(conn);
        // }
        // catch (Exception ex) {
        // Console.WriteLine(ex.Message);
        // }
        // return resp;
        
        /// <summary>
        /// 检查员查看当前垃圾站的运输记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,Watcher")]
        public List<Transport> WatcherGet(string req)
        {
            var resp = new List<Transport>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TRANS_ID,DUSTBIN_ID,CARRIER_ID,TRUCK_ID,START_TIME,END_TIME,PLANT_NAME " +
                                  "FROM TRANSPORT NATURAL JOIN DUSTBIN " +
                                  $"WHERE SITE_NAME='{req}' " +
                                  "ORDER BY START_TIME DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var temp = new Transport
                    {
                        trans_id = reader.GetString(0),
                        dustbin_id = reader.GetString(1),
                        carrier_id = reader.GetString(2),
                        truck_id = reader.GetString(3),
                        start_time = reader.GetDateTime(4),
                        end_time = reader.GetDateTime(5),
                        plant_name = reader.GetString(6)
                    };
                    resp.Add(temp);
                }

                DBConn.CloseConn(conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return resp;
        }
        
        /// <summary>
        /// 运输员查看自己运输记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,Carrier")]
        public List<Transport> CarrierGet(string req)
        {
            var resp = new List<Transport>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TRANS_ID,DUSTBIN_ID,CARRIER_ID,TRUCK_ID,START_TIME,END_TIME,PLANT_NAME " +
                                  "FROM TRANSPORT " +
                                  $"WHERE CARRIER_ID='{req}' " +
                                  "ORDER BY START_TIME DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var temp = new Transport
                    {
                        trans_id = reader.GetString(0),
                        dustbin_id = reader.GetString(1),
                        carrier_id = reader.GetString(2),
                        truck_id = reader.GetString(3),
                        start_time =  reader.GetDateTime(4),
                        end_time =  reader.IsDBNull(5)? reader.GetDateTime(4): reader.GetDateTime(5),
                        plant_name =reader.IsDBNull(6)?"":reader.GetString(6)
                    };
                    resp.Add(temp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return resp;
        }
        
        /// <summary>
        /// 按照垃圾站查找运输记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,StationStaff")]
        public List<Transport> StaffGet(string req)
        {
            var resp = new List<Transport>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TRANS_ID,DUSTBIN_ID,CARRIER_ID,TRUCK_ID,START_TIME,END_TIME,PLANT_NAME " +
                                  "FROM TRANSPORT " +
                                  $"WHERE PLANT_NAME='{req}' OR PLANT_NAME is null " +
                                  "ORDER BY START_TIME DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var temp = new Transport
                    {
                        trans_id = reader.GetString(0),
                        dustbin_id = reader.GetString(1),
                        carrier_id = reader.GetString(2),
                        truck_id = reader.GetString(3),
                        start_time =  reader.GetDateTime(4),
                        end_time =  reader.IsDBNull(5)? reader.GetDateTime(4): reader.GetDateTime(5),
                        plant_name =reader.IsDBNull(6)?"":reader.GetString(6)
                    };
                    resp.Add(temp);
                }
                DBConn.CloseConn(conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
            var cmd = conn.CreateCommand();
            var dt = DateTime.Now;
            try
            {
                //INSERT INTO "C##PDCR"."VIOLATE_RECORD" ("WATCHER_ID", "REASON", "PUNISHMENT", "VIOLATE_TIME", "GAR_ID") VALUES ('2952108', '123', '1', TO_DATE('2021-07-20 11:35:11', 'SYYYY-MM-DD HH24:MI:SS'), '11111111');
                cmd.CommandText = "INSERT INTO violate_record " +
                                 $"VALUES('{req.watcher_id}','{req.reason}',{req.punishment}," +
                                 $"TO_DATE('{dt:yyyy-MM-dd hh:mm:ss}','yyyy-mm-dd hh24:mi:ss'),'{req.gar_id}')";
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
                                 $"SET reason='{req.reason}',punishment='{req.punishment}' " +                                
                                 $"WHERE gar_id={req.gar_id}";
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
        [HttpGet] //Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Watcher,Administrator")]
        public DeleteResponse Delete(string req)
        {
            var resp = new DeleteResponse();
            OracleConnection conn = null;
            try
            {
                conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM VIOLATE_RECORD " +
                                  $"WHERE gar_id='{req}'";           
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
                                 $"WHERE violate_time<=TO_DATE('{req:yyyy-MM-dd hh-mm-dd}','yyyy-mm-dd hh24-mi-ss')";
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
                cmd.CommandText = "SELECT gar_id,watcher_id,reason,punishment,violate_time " +
                                  "FROM violate_record natural join GARBAGE " +
                                 $"WHERE user_id='{req}'" +
                                 "ORDER BY violate_time DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new ViolateRecord
                    {
                        user_id="",
                        gar_id = reader.GetString(0),
                        watcher_id = reader.GetString(1),
                        reason = reader.GetString(2),
                        punishment = reader.GetInt32(3),
                        violate_time = reader.GetDateTime(4)
                    };
                    respList.Add(resp);
                }
            }
            catch (Exception ex) {Console.WriteLine(ex.Message); }
            return respList;
        }

        
        /// <summary>
        /// 以检查员的ID获取违规记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Watcher,Administrator")]
        public List<ViolateRecord> GetAll(string req)
        {
            var respList = new List<ViolateRecord>();
            try
            {
                var conn = DBConn.OpenConn();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT gar_id,user_id,reason,punishment,violate_time " +
                                  "FROM violate_record natural join GARBAGE " +
                                  $"WHERE watcher_id='{req}'" +
                                  "ORDER BY violate_time DESC";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resp = new ViolateRecord
                    {
                        watcher_id="",
                        gar_id = reader.GetString(0),
                        user_id = reader.GetString(1),
                        reason = reader.GetString(2),
                        punishment = reader.GetInt32(3),
                        violate_time = reader.GetDateTime(4)
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
        [HttpPost] //[ Authorize(Roles = "Administrator")]
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return respList;
        }
    }
}