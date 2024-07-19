using Microsoft.AspNetCore.Mvc;

namespace LokerMVC.Controllers
{
    [Route("[controller]/[action]")]
    public class ErrorController : Controller
    {
        public IActionResult Index(string message)
        {

            ViewBag.message = message;
            return View();
        }
    }
}
