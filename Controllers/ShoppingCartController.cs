using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PH_Swag.Models;
using PH_Swag.Services;
using System.Data;

namespace PH_Swag.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private string _curUser;

        private readonly ILogger<HomeController> _logger;
        private PH_Swag.Models.UserAccess _usr;
        private readonly IConfiguration _configuration;
        private PH_SwagEntity _db1;
        private int isValid = 0;

        // Viewbags: start
        private int _userID;
        private int _cartID;
        private int _numItems;
        private string _swagBucks;
        private string _cartItemTotal;
        private string _username;
        // Viewbags: end

        
        public ShoppingCartController(
                   ILogger<HomeController> logger,
                   PH_SwagEntity db,
                   IConfiguration configuration,
               [FromServices] LoginService ls, [FromServices] CartService cs) : base(db, ls, cs)
        {
            _db1 = db;
                _logger = logger;
            _usr = ViewBag.UserAccess;
            _configuration = configuration;

            _userID = ViewBag.UserID;
            _cartID = ViewBag.CartID;
            _numItems = ViewBag.NumItems;
            _swagBucks = ViewBag.SwagBucks.ToString();
            _username = ViewBag.UserName;

        }


        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.UserAccess = _usr;

            ViewBag.UserID = _userID;
            ViewBag.CartID = _cartID;
            ViewBag.NumItems = _numItems;
            ViewBag.SwagBucks = _swagBucks;
            ViewBag.UserName = _username;

            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("CartItem_List", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }

            List<CartItemViewModel> cartItem = dtbl.AsEnumerable()
              .Select(row => new CartItemViewModel()
              {
                  // assuming column 0's type is Nullable<long>
                  Id = row.Field<int>("Id"),
                  prodId = row.Field<int>("prodID"),
                  prodName = String.IsNullOrEmpty(row.Field<string>("prodName"))
                      ? "not found"
                      : row.Field<string>("prodName"),
                  prodDescription = String.IsNullOrEmpty(row.Field<string>("prodDescription"))
                      ? "not found"
                      : row.Field<string>("prodDescription"),
                  quantity = row.Field<int?>("p_quantity").GetValueOrDefault(),
                  prodData = row.Field<byte[]>("prodData"),
                  prodSizes = String.IsNullOrEmpty(row.Field<string>("p_size"))
                      ? "not found"
                      : row.Field<string>("p_size")
              }).ToList();


            var tData = cartItem.ToList();
            

            //return View(product.ToPagedList(1,2));
            return View(tData);

            //return View(product);
        }


        [HttpGet]
        public IActionResult Edit(int? id, int? page, string Order, int? alert, string filter)
        {
            string connstr = _configuration.GetConnectionString("DevConnection");

            // Alert: Start
            ViewBag.Alert = HttpContext.Session.GetString("cartItem_Alert");
            TempData["cartAlert"] = HttpContext.Session.GetString("cartItem_Alert");

            if (HttpContext.Session.GetString("cartItem_Alert") != null)
            {
                HttpContext.Session.SetString("cartItem_Alert", "");
            }

            // Alert: End


            // Used by the _layout.cshtml (outside this view): Start
            ViewBag.UserAccess = _usr;

            ViewBag.UserID = _userID;
            ViewBag.CartID = _cartID;
            ViewBag.NumItems = _numItems;
            ViewBag.SwagBucks = _swagBucks;
            ViewBag.UserName = _username;

            HttpContext.Session.SetString("CartID", _cartID.ToString());

            //drop down (sizes): start
            //var siz = _db1.Sizes.Where(m => m.isActive == 1).ToList();
            ////ViewBag.listSizes = siz.ToList();
            //ViewBag.listSizes = new SelectList(siz.Where(t => t.isActive == 1).OrderBy(t => t.sortOrder).ToList(), "size", "size");
            //drop down (sizes): end


            if (id == null)
            {
                return new StatusCodeResult(1);
            }

            // Load Cart item
            var cartitem = _db1.CartItem.Find(id);
            ViewBag.CartItemID = id;
            if (cartitem == null)
            {  
                return new StatusCodeResult(0); 
            }

            if (cartitem != null)
            {
                // Get product info (Name, Description)
                var prod = _db1.Products.Find(cartitem.prodId);
                if (prod != null)
                {
                    var prodSizes = prod.prodSizes;
                    //drop down (sizes): start
                    if (prodSizes != null)
                    {

                        // Get all available sizes (comma delimited)
                        var siz = _db1.Sizes.Where(m => m.isActive == 1).ToList();
                        // Split into array
                        IEnumerable<string> prodSizesArray = prodSizes.Split(',');
                        // Create a comma delimited of only checked sizes
                        var siz2 = siz.Where(c => prodSizesArray.Contains(c.size));
                        // create a dropdown of only checked sizes
                        ViewBag.listSizes = new SelectList(siz2.ToList(), "size", "size", cartitem.p_Size);


                        ViewBag.Quantity = cartitem.p_quantity;
                    }

                    return View(prod);
                }

            }

            Product prod1 = new Product();
            prod1.id = 0;
            return View(prod1);
        }

        [HttpPost]
        public IActionResult UpdateCart([FromServices] CartService srv, int cartItemID, int prodID, int txtQuantity, string listSizes)
        {

            // Update The cart Here
            var cartResult = 0;

            var pQuant = txtQuantity;
            var pListSize = listSizes;
           // var cartItemId = HttpContext.Session.GetString("cartItemID") ?? "0";

            if (cartItemID == null)
            {
                return RedirectToAction("Detail", "Merchandise", new { id = prodID });
            }

            if (cartItemID != null)
            {
                cartResult = srv.UpdateCartItem(_userID, Convert.ToInt32(cartItemID),  pQuant, pListSize);
            }


            // Redirect to Cart List

            return RedirectToAction("Index", "ShoppingCart", new { id = prodID });



            
        }


        public IActionResult AddSubtractQty([FromServices] CartService srv, int addSub, int cartItemID, int prdId, int quantity, string listSizes)
        {
            var cartResult = 0;
            var pQuant = quantity;

            cartResult = srv.UpdateCartItemQuantity(addSub, _userID, Convert.ToInt32(cartItemID));

            

            return RedirectToAction("Index", "ShoppingCart", new { id = prdId });
        }

        public IActionResult DeleteCartItem(int cartItemID)
        {

            // check to see if this cartItemID belongs to this User/Cart

            var cartItem = _db1.CartItem.FirstOrDefault(x => x.Id == cartItemID);

            if (cartItem != null) {
                    return RedirectToAction("Index", "ShoppingCart");
            }




            ViewBag.CartItemID = cartItemID;
            return View(cartItem);
        }


    }
}
