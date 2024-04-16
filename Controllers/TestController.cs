using Microsoft.AspNetCore.Mvc;

namespace PH_Swag.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
