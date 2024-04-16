using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PagedList;
using PH_Swag.Models;
using PH_Swag.Services;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using PH_Swag.Enums;

namespace PH_Swag.Controllers
{
   
    public class MerchandiseController : BaseController
    {

        private readonly ILogger<HomeController> _logger;
        private PH_Swag.Models.UserAccess _usr;
        private readonly IConfiguration _configuration;
        private PH_SwagEntity _db1;

        // Viewbags: start
        private int _userID;
        private int _cartID;
        private int _numItems;
        private string _swagBucks;
        private string _cartItemTotal;
        private string _username;
        // Viewbags: end

        public MerchandiseController(
                ILogger<HomeController> logger, 
                PH_SwagEntity db,
                IConfiguration configuration,
            [FromServices] LoginService ls, [FromServices] CartService cs) : base(db, ls, cs)
        {
            _db1 = db;
            _logger = logger;
            _configuration = configuration;
            _usr = ViewBag.UserAccess;

            _userID = ViewBag.UserID;
            _cartID = ViewBag.CartID;
            _numItems = ViewBag.NumItems;
            _swagBucks = ViewBag.SwagBucks.ToString();
            _username = ViewBag.UserName;

        }

        // List of Products (Client side)
        [HttpGet]
        public IActionResult Index(int? page, string Order, int? alert, string filter)
        {
            string connstr = _configuration.GetConnectionString("DevConnection");

            // Set cartItem alert session to empty
            HttpContext.Session.SetString("cartItem_Alert", "");
            ViewBag.Alert = "";
            TempData["cartAlert"] = "";

            ViewBag.UserAccess = _usr;

            // Used by the _layout.cshtml (outside this view): Start
            ViewBag.UserID = _userID;
            ViewBag.CartID = _cartID;
            ViewBag.NumItems = _numItems;
            ViewBag.SwagBucks = _swagBucks;
            ViewBag.UserName = _username;

            // Used by the _layout.cshtml (outside this view): End

            string viewfilter = (string.IsNullOrEmpty(filter)) ? "Show All" : filter;
            int PageNumber = (page > 0) ? (int)page : 1;
            string SortOrder = string.IsNullOrEmpty(Order) ? "ID" : Order;
            ViewBag.Alert = alert.HasValue ? alert : 0;
            ViewBag.CartItemTotal = _cartItemTotal;

            //ViewData["PatientSort"] = SortOrder == "Patient" ? "Patient_desc" : "Patient";
            //ViewData["CCSSort"] = SortOrder == "CCSName" ? "CCSName_desc" : "CCSName";
            //ViewData["ServiceDateSort"] = SortOrder == "ServiceDate" ? "ServiceDate_desc" : "ServiceDate";
            //ViewData["PickupSort"] = SortOrder == "Pickup" ? "Pickup_desc" : "Pickup";
            //ViewData["DateAddedSort"] = SortOrder == "DateAdded" ? "DateAdded_desc" : "DateAdded";
            //ViewData["AmountSort"] = SortOrder == "Amount" ? "Amount_desc" : "Amount";
            //ViewData["MethodSort"] = SortOrder == "Method" ? "Method_desc" : "Method";
            //ViewData["PickupDateSort"] = SortOrder == "PickupDate" ? "PickupDate_desc" : "PickupDate";

            //ViewBag.SortOrder = SortOrder;
            ViewBag.PageNumber = PageNumber;
            ViewBag.Filter = viewfilter;

            



            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("Product_List", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }

            // dtbl is a type of Datatable, has no paging functionality
            // We need to convert a datatable into a List
            // Convert into a List: Start
            //List<ProductViewModel> product = dtbl.AsEnumerable()
            //    .Select(row => new ProductViewModel()
            //    {
            //        // assuming column 0's type is Nullable<long>
            //        ID = row.Field<int?>(0).GetValueOrDefault(),
            //        prodDescription = String.IsNullOrEmpty(row.Field<string>(1))
            //            ? "not found"
            //            : row.Field<string>(1),
            //        prodPrice = row.Field<int?>(0).GetValueOrDefault(),
            //        catDescription = String.IsNullOrEmpty(row.Field<string>(1))
            //            ? "not found"
            //            : row.Field<string>(1)

            //    }).ToList();

            //List<Product> product = dtbl.AsEnumerable()
            //   .Select(row => new Product()


            List <ProductViewModel> product = dtbl.AsEnumerable()
               .Select(row => new ProductViewModel()
               {
                   // assuming column 0's type is Nullable<long>
                   ID = row.Field<int>("ID"),
                   prodName  = String.IsNullOrEmpty(row.Field<string>("prodName"))
                       ? "not found"
                       : row.Field<string>("prodName"),
                   prodDescription = String.IsNullOrEmpty(row.Field<string>("prodDescription"))
                       ? "not found"
                       : row.Field<string>("prodDescription"),
                   prodPrice = row.Field<double?>("prodPrice").GetValueOrDefault(),
                   catDescription = String.IsNullOrEmpty(row.Field<string>("catDescription"))
                       ? "not found"
                       : row.Field<string>("catDescription"),
                   prodData = row.Field<byte[]>("prodData")

               }).ToList();

            //List<MyType> listName = dataTableName.AsEnumerable().Select(m => new MyType()
            //{
            //    ID = m.Field<string>("ID"),
            //    Description = m.Field<string>("Description"),
            //    Balance = m.Field<double>("Balance"),
            //}).ToList()
            // Convert into a List: End


            //if (!String.IsNullOrEmpty(viewfilter) || viewfilter != "Show All")
            if (viewfilter != "Show All")
            {
                    product = product.Where(t => t.catDescription == viewfilter).ToList();   
            }

            // Drop Down List (for filtering type of SWAG): start
            //ViewBag.Filters = new SelectList(_db.Categories.Where(t => t.isActive == 1).Where(t => t.catName == viewfilter).OrderBy(t => t.sortOrder).ToList(), "catName", "catName", viewfilter);
            ViewBag.Filters = new SelectList(_db1.Categories.Where(t => t.isActive == 1).OrderBy(t => t.sortOrder).ToList(), "catDescription", "catName", viewfilter);
            
            // Drop Down List (for filtering type of SWAG): start




            //Page Numbers: Start
            const int pageSize =6;
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }
            int recsCount = product.Count();
            var pager = new Pager(recsCount, PageNumber, pageSize);
            int recSkip = (PageNumber - 1) * pageSize;
            //Page Numbers: end

            var tData = product.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            //return View(product.ToPagedList(1,2));
            return View(tData);

            //===============================================
            // Example: TO Return a string to the view: Start
            //===============================================
            //string wernString = "Wernher Picato string";
            //return View((object)wernString);

            // In View:
            // @model string
            // <p>@Model</p>
            //===============================================
            // Example: TO Return a string to the view: End
            //===============================================



        }



        [HttpGet]
        public IActionResult Detail(int? id, int? page, string Order, int? alert, string filter)
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

            // prodct = _db1.Products.First(m => m.id == 1);
            //            var prod = _db1.Products.Where(m => m.isActive == 1).ToList();
            // var prod = _db1.Products.Where(m => m.id == 6).ToList();

            var prod = _db1.Products.Find(id);

            if (prod != null)
            {
                var prodSizes = prod.prodSizes;
                //drop down (sizes): start
                if (prodSizes != null)
                {
                    // List<String> list = new List<string>(prodSizes.Split(','));
                    //ViewBag.listSizes = new SelectList(list.ToList(), list[0], list[0]);


                    

                    //ViewBag.listSizes = new SelectList(siz.Where(t => t.isActive == 1).OrderBy(t => t.sortOrder).ToList(), "size", "size");
                    var siz = _db1.Sizes.Where(m => m.isActive == 1).ToList();

                    IEnumerable<string> prodSizesArray = prodSizes.Split(',');
                    var siz2 = siz.Where(c => prodSizesArray.Contains(c.size));
                    ViewBag.listSizes = new SelectList(siz2.ToList(), "size", "size");
                }


                return View(prod);
            }


            Product prod1 = new Product();
            prod1.id = 0;
            return View(prod1);
        }



        [HttpPost]
        public IActionResult AddToCart([FromServices] CartService srv, int prodID, int txtQuantity, string listSizes)
        {
            var cartResult = 0;

            var pQuant = txtQuantity;
            var pListSize = listSizes;
            var cartID = HttpContext.Session.GetString("CartID") ?? "0";

            if (cartID == null) {
                return RedirectToAction("Detail", "Merchandise", new { id = prodID });
            }

            if (cartID != null )
            {
                cartResult = srv.AddProductsToCart(_userID, Convert.ToInt32(cartID), prodID, pQuant, pListSize);
            }


            //if (cartResult > 0) // cartID is successfull
            //{

            //}
            //else {
            //    ViewBag.CartError = "Error adding into cart. Please contact your IT.";
            //}

            //if (cartResult > 0)
            //{
            //    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Success, "Item added to cart.");
            //}
            //else ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item failed to add to cart. Please contact your IT.");


            if (cartResult > 0)
            {
                HttpContext.Session.SetString("cartItem_Alert", CommonServices.ShowAlert(Alerts.Success, "Item added to cart."));
                //ViewBag.Alert = CommonServices.ShowAlert(Alerts.Success, "Item added to cart.");
            }
            else
            {
                HttpContext.Session.SetString("cartItem_Alert", CommonServices.ShowAlert(Alerts.Danger, "Item failed to add to cart. Please contact your IT."));
                //ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item failed to add to cart. Please contact your IT.");

            }

          



            // Create a cart service that will add items to cart item and update the cart.



            // Continue Here


            //if (cartID == 0)
            //{
            //    AddToCartForSession(srv, prdId, 1);
            //}
            //else
            //{
            //    ViewData["ItemCount"] = srv.AddProductsToCart(customerId, prdId, 1);
            //}
            //return PartialView("_CartButton");

            //return View("/Merchandise/Detail/" + prdId.ToString());


            return RedirectToAction("Detail", "Merchandise", new { id = prodID });
            //return View("Merchandise/Detail/" + prodID);
        }

        private void AddToCartForSession([FromServices] CartService srv, int prdId, int quantity)
        {
            //var sessionCart = HttpContext.Session.GetString("Cart");
            //var cart = new Cart();
            //if (sessionCart == null)
            //{
            //    srv.AddCartItemForSession(prdId, quantity, cart);
            //}
            //else
            //{
            //    cart = JsonConvert.DeserializeObject<Cart>(sessionCart);
            //    var prdCart = cart.CartItems.FirstOrDefault(x => x.ProductId == prdId);
            //    srv.AddCartItemForSession(prdId, quantity, cart);
            //}
            //var strCart = JsonConvert.SerializeObject(cart);
            //HttpContext.Session.SetString("Cart", strCart);
            //ViewData["ItemCount"] = cart.Quantity;
        }
    }
}
