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
    public class TrashController : Controller
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
        
    }
}