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
    [ApiController, Route("Facility/[controller]/[action]")]
    public class TrashCanController : Controller
    {
        //加鉴权就加一个 [Authorize(Roles="Adminstrator")] 比如这就是管理员才能访问。
        [HttpPost]
        public AddResponse Add([FromBody] TrashCan req)
        {
            var resp = new AddResponse {Status = Config.TEST};
            return resp;
        }

        [HttpPost]
        public DeleteResponse Delete([FromBody] TrashCan req)
        {
            var resp = new DeleteResponse { Status=Config.TEST};
            return resp;
        }

        [HttpPost]
        public UpdateResponse Update([FromBody] TrashCan req)
        {
            var resp = new UpdateResponse{Status = Config.TEST};
            return resp;
        }

        [HttpGet]
        public TrashCan Get(string req)
        {
            var resp = new TrashCan();
            return resp;
        }
        [HttpGet]
        public List<TrashCan> GetAll()
        {
            var resp = new List<TrashCan>();
            return resp;
        }
    }
    
    [ApiController, Route("Facility/[controller]/[action]")]
    public class BinSiteController : Controller
    {
        [HttpPost]
        public AddResponse Add([FromBody] BinSite req)
        {
            var resp = new AddResponse {Status = Config.TEST};
            return resp;
        }

        [HttpPost]
        public DeleteResponse Delete([FromBody] BinSite req)
        {
            var resp = new DeleteResponse { Status=Config.TEST};
            return resp;
        }

        [HttpPost]
        public UpdateResponse Update([FromBody] BinSite req)
        {
            var resp = new UpdateResponse{Status = Config.TEST};
            return resp;
        }

        [HttpGet]
        public BinSite Get(string req)
        {
            var resp = new BinSite();
            return resp;
        }
        [HttpGet]
        public List<BinSite> GetAll()
        {
            var resp = new List<BinSite>();
            return resp;
        }
    }
    [ApiController, Route("Facility/[controller]/[action]")]
    public class TruckController : Controller
    {
        [HttpPost]
        public AddResponse Add([FromBody] Truck req)
        {
            var resp = new AddResponse {Status = Config.TEST};
            return resp;
        }

        [HttpPost]
        public DeleteResponse Delete([FromBody] Truck req)
        {
            var resp = new DeleteResponse { Status=Config.TEST};
            return resp;
        }

        [HttpPost]
        public UpdateResponse Update([FromBody] Truck req)
        {
            var resp = new UpdateResponse{Status = Config.TEST};
            return resp;
        }

        [HttpGet]
        public Truck Get(string req)
        {
            var resp = new Truck();
            return resp;
        }
        [HttpGet]
        public List<Truck> GetAll()
        {
            var resp = new List<Truck>();
            return resp;
        }
    }
    [ApiController, Route("Facility/[controller]/[action]")]
    public class PlantController : Controller
    {
        [HttpPost]
        public AddResponse Add([FromBody] Truck req)
        {
            var resp = new AddResponse {Status = Config.TEST};
            return resp;
        }

        [HttpPost]
        public DeleteResponse Delete([FromBody] Truck req)
        {
            var resp = new DeleteResponse { Status=Config.TEST};
            return resp;
        }

        [HttpPost]
        public UpdateResponse Update([FromBody] Truck req)
        {
            var resp = new UpdateResponse{Status = Config.TEST};
            return resp;
        }

        [HttpGet]
        public Plant Get(string req)
        {
            var resp = new Plant();
            return resp;
        }

        [HttpGet]
        public List<Plant> GetAll()
        {
            var resp = new List<Plant>();
            return resp;
        }
    }
}