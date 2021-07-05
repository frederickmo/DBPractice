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
    /// 垃圾有关的api接口
    /// </summary>
    [ApiController, Route("[controller]/[action]")]
    public class GarbageController : Controller
    {
        /// <summary>
        /// 添加垃圾，由用户申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /*
         * 垃圾状态说明（个人想法）：
         * 垃圾注册之后第一时间状态是：未到站，然后生成一个二维码
         * 到了垃圾桶那里被watcher扫一下，然后分为进站和不进站（不进站可以扣分）：状态变为 已退回（此时finishtime更新） 或者 进桶 
         * 之后就跟快递一样。
         */
        [HttpPost]
        public AddResponse Add([FromBody]Garbage req)
        {
            var resp = new AddResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 撤回垃圾，只有在垃圾状态为未到站的情况下才可以由用户进行撤回
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse Delete([FromBody] Garbage req)
        {
            var resp = new DeleteResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾状态的更新，但其实大部分垃圾状态的更新应该交给trigger来完成，本接口只是有备无患
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public UpdateResponse Update([FromBody] Garbage req)
        {
            var resp = new UpdateResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 获取某一个垃圾投递的记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Garbage> Get()
        {
            var resp = new List<Garbage>();
            return resp;
        }
        /*
         * type 表示请求记录的类型
         * 0        违规的
         * 1        成功投递的
         */

        /// <summary>
        /// 获取所有垃圾投递的记录
        /// type 表示请求记录的类型
        /// 0        违规的
        /// 1        成功投递的
        /// 2        所有记录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Garbage> GetRecords(int type)
        {
            var resp = new List<Garbage>();
            return resp;
        }
    }
    /// <summary>
    /// 有关所有垃圾运输的api
    /// 请注意垃圾只有结束运输的时候才写入数据库，否则放在缓存now里
    /// </summary>
    [ApiController, Route("[controller]/[action]")]
    public class TransportController : Controller
    {
        private List<TransportRequest> now { set; get; }//正在运送的还没写进数据库的。
        /// <summary>
        /// 投递垃圾的行为
        /// 投递行为发生之后，生成一条违规记录或者投递记录
        /// 并且trigger触发更新垃圾的状态
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public Response Throw([FromBody] ThrowRequest req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
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
        public Response TransportStart([FromBody] TransportRequest req)
        {
            var resp = new Response {status = Config.TEST};
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
        public Response TransportEnd([FromBody] TransportRequest req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾处理完毕，更新垃圾状态
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public Response Finish([FromBody] FinishRequest req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 获取垃圾运输记录
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "administrator")]
        [HttpGet]
        public List<TransportRequest> GetAll(string type)
        {
            List<TransportRequest> resp = new List<TransportRequest>();
            
            var temp = new TransportRequest {pid = "123", sid = "123", tid = "123"};
            var temp1 = new TransportRequest {pid = "1234", sid = "1234", tid = "1234"};
            resp.Add(temp);
            resp.Add(temp);
            return resp;
        }
    }
}