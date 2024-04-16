using Microsoft.AspNetCore.Mvc;
using PH_Swag.Models;
using PH_Swag.Services;
using System.Diagnostics;

namespace PH_Swag.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        

        private PH_Swag.Models.UserAccess _usr;

        // Viewbags: start
        private int _userID;
        private int _cartID;
        private int _numItems;
        private string _swagBucks;
        private string _cartItemTotal;
        private string _username;
        // Viewbags: end


        public HomeController(ILogger<HomeController> logger, PH_SwagEntity db,
            [FromServices] LoginService ls, [FromServices] CartService cs) : base(db,ls,cs) 
        {
            _logger = logger;
            _usr = ViewBag.UserAccess;

            _userID = ViewBag.UserID;
            _cartID = ViewBag.CartID;
            _numItems = ViewBag.NumItems;
            _swagBucks = ViewBag.SwagBucks.ToString();
            _username = ViewBag.UserName;

        }

       
        public IActionResult Index()
        {
            ViewBag.UserAccess = _usr;
            
            ViewBag.UserID = _userID;
            ViewBag.CartID = _cartID;
            ViewBag.NumItems = _numItems;
            ViewBag.SwagBucks = _swagBucks;
            ViewBag.UserName = _username;

            HttpContext.Session.SetString("FacilityID", "test");
            return View();
        }

        public IActionResult Privacy()
        {
            ViewBag.UserAccess = usr;

            ViewBag.UserID = _userID;
            ViewBag.CartID = _cartID;
            ViewBag.NumItems = _numItems;
            ViewBag.SwagBucks = _swagBucks;
            ViewBag.UserName = _username;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
