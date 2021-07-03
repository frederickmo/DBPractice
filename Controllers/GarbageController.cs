using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DBPractice.Models;
using Microsoft.AspNetCore.Authorization;

namespace DBPractice.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class GarbageController : Controller
    {
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
        /*
         * 垃圾撤回只能在未到站的时候撤回
         */
        [HttpPost]
        public DeleteResponse Delete([FromBody] Garbage req)
        {
            var resp = new DeleteResponse {status = Config.TEST};
            return resp;
        }
        /*
         * 更新只能更新状态
         * 当然如果写一个专门的request类比较好写
         */
        [HttpPost]
        public UpdateResponse Update([FromBody] Garbage req)
        {
            var resp = new UpdateResponse {status = Config.TEST};
            return resp;
        }
        /*
         * 这个我也不清楚干嘛，但反正要有一个get...XS
         */
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
        [HttpGet]
        public List<Garbage> GetRecords(int type)
        {
            var resp = new List<Garbage>();
            return resp;
        }
    }
    
    [ApiController, Route("[controller]/[action]")]
    public class TransportController : Controller
    {
        private List<TransportRequest> now { set; get; }//正在运送的还没写进数据库的。

        [HttpPost]
        public Response Throw([FromBody] ThrowRequest req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        /*
         * 这个是由运输员开始的，然后直接入now
         */
        [HttpPost]
        public Response TransportStart([FromBody] TransportRequest req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }
        
        /*
         * 这个只能给垃圾站的人访问
         * 从now里找到然后完整的写进去，顺便更新状态
         */
        [HttpPost]
        public Response TransportEnd([FromBody] TransportRequest req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }

        [HttpPost]
        public Response Finish([FromBody] FinishRequest req)
        {
            var resp = new Response {status = Config.TEST};
            return resp;
        }

        /*
         * 未完成的运输，从now里读
         * 已完成的运输，从数据库里读
         * meaning of type's value:
         * all              return all TransportRequests about the carrier or plant
         * finished         return TransportRequests that have been finished
         * transporting     return TransportRequests that have not been finished
         */
        [HttpGet]
        public List<TransportRequest> GetAll(string type)
        {
            var resp = new List<TransportRequest>();
            return resp;
        }
        
        
    }
}