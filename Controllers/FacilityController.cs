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
    ///<summary>
    /// 垃圾桶相关的api接口
    /// 请注意垃圾桶的属性 condition是char类型的
    /// </summary>>
    // [Authorize(Roles="Adminstrator")]
    [ApiController, Route("Facility/[controller]/[action]")]
    public class TrashCanController : Controller
    {
        /// <summary>
        /// 垃圾桶的添加
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        //加鉴权就加一个 [Authorize(Roles="Adminstrator")] 比如这就是管理员才能访问。
        [HttpPost]
        public AddResponse Add([FromBody] TrashCan req)
        {
            var resp = new AddResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾桶的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse Delete([FromBody] TrashCan req)
        {
            var resp = new DeleteResponse { status=Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾桶的状态更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public UpdateResponse Update([FromBody] TrashCan req)
        {
            var resp = new UpdateResponse{status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 获取某一个垃圾桶的属性
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public TrashCan Get(string req)
        {
            var resp = new TrashCan();
            return resp;
        }
        /// <summary>
        /// 获取所有垃圾桶一个列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<TrashCan> GetAll()
        {
            var resp = new List<TrashCan>();
            return resp;
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
        public AddResponse Add([FromBody] BinSite req)
        {
            var resp = new AddResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾站的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse Delete([FromBody] BinSite req)
        {
            var resp = new DeleteResponse { status=Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾站的属性更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public UpdateResponse Update([FromBody] BinSite req)
        {
            var resp = new UpdateResponse{status = Config.TEST};
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
            return resp;
        }
        /// <summary>
        /// 获取所有垃圾站的一个列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<BinSite> GetAll()
        {
            var resp = new List<BinSite>();
            return resp;
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
        public AddResponse Add([FromBody] Truck req)
        {
            var resp = new AddResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾车的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse Delete([FromBody] Truck req)
        {
            var resp = new DeleteResponse { status=Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾车的状态更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public UpdateResponse Update([FromBody] Truck req)
        {
            var resp = new UpdateResponse{status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 获取某一个垃圾车的属性
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public Truck Get(string req)
        {
            var resp = new Truck();
            return resp;
        }
        /// <summary>
        /// 获取所有垃圾车的一个列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Truck> GetAll()
        {
            var resp = new List<Truck>();
            return resp;
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
        public AddResponse Add([FromBody] Truck req)
        {
            var resp = new AddResponse {status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾处理站的删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse Delete([FromBody] Truck req)
        {
            var resp = new DeleteResponse { status=Config.TEST};
            return resp;
        }
        /// <summary>
        /// 垃圾处理站的更新
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public UpdateResponse Update([FromBody] Truck req)
        {
            var resp = new UpdateResponse{status = Config.TEST};
            return resp;
        }
        /// <summary>
        /// 获取某一个垃圾处理站的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public Plant Get(string req)
        {
            var resp = new Plant();
            return resp;
        }
        /// <summary>
        /// 获取所有垃圾处理站的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Plant> GetAll()
        {
            var resp = new List<Plant>();
            return resp;
        }
    }
}