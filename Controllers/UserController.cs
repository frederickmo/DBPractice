using Microsoft.AspNetCore.Mvc;

namespace DBPractice.Controllers
{
    public class UserController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}