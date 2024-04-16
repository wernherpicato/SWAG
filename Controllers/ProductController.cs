using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using PH_Swag.Models;
using PH_Swag.Services;
using System.Collections;
using System.Data;
using System.IO;

namespace PH_Swag.Controllers
{
    public class ProductController : BaseController
    {

        private readonly ILogger<HomeController> _logger;
        private string _curUser;
        
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

        public ProductController(
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


        // List of Products
        public IActionResult Index(int? page, string Order, int? alert, string filter)
        {
            string connstr = _configuration.GetConnectionString("DevConnection");

            // Used by the _layout.cshtml (outside this view): Start
            ViewBag.UserAccess = _usr;

            ViewBag.UserID = _userID;
            ViewBag.CartID = _cartID;
            ViewBag.NumItems = _numItems;
            ViewBag.SwagBucks = _swagBucks;
            ViewBag.UserName = _username;

            // ViewBag.CartItemTotal = _cartItemTotal;
            // Used by the _layout.cshtml (outside this view): End

            string viewfilter = (string.IsNullOrEmpty(filter)) ? "Show All" : filter;
            int PageNumber = (page > 0) ? (int)page : 1;
            string SortOrder = string.IsNullOrEmpty(Order) ? "ID" : Order;
            ViewBag.Alert = alert.HasValue ? alert : 0;


            //ViewData["TitleSort"] = SortOrder == "Name" ? "Name_desc" : "Name";
           

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


            //List<Product> product = dtbl.AsEnumerable()
            //   .Select(row => new Product()
            List<ProductViewModel> product = dtbl.AsEnumerable()
               .Select(row => new ProductViewModel()
               {
                   // assuming column 0's type is Nullable<long>
                   ID = row.Field<int>("ID"),
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
            const int pageSize = 3;
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
        public IActionResult Create()
        {
            // Used by the _layout.cshtml (outside this view): Start
            ViewBag.AccountName = _curUser;
            ViewBag.UserAccess = _usr;
            // Used by the _layout.cshtml (outside this view): End

            

            //checkboxes: start
            var siz = _db1.Sizes.Where(m => m.isActive ==1).ToList();
            ViewBag.listSizes = siz.ToList();
            //checkboxes: end

            //checkboxes: start
            //List<CheckboxModel> list = new List<CheckboxModel>()
            //{   
            //    new CheckboxModel {Text = "ZZZL", Value="1"},
            //    new CheckboxModel {Text = "ZZZL", Value="2"},
            //    new CheckboxModel {Text = "ZZZL", Value="3"},
            //    new CheckboxModel {Text = "ZZZL", Value="4"}
            //};

            ViewBag.Sizes = _db1.Sizes.OrderBy(t => t.id).ToList();
            List<CheckboxModel> list = new List<CheckboxModel>();

            //For new products: start

            for (int i = 0; i < siz.Count; i++)
            {
                list.Add(new CheckboxModel
                {
                    Value = siz[i].id.ToString(),
                    Text = siz[i].size,
                    Checked = false
                });
            }
            //For new products: End

            // Use this when editing a product: start
            //string sizeSelected = "XS,S,M,L,XL";
            //string[] sizes = sizeSelected.Split(new[] { "," }, StringSplitOptions.None);
            //bool sizeChecked = false;

            //for (int i = 0; i < siz.Count; i++)
            //{
            //    sizeChecked = false;
            //    for (int j = 0; j < sizes.Length; j++)
            //    {
            //        if (sizes[j].ToString() == siz[i].size.ToString() )
            //        {
            //            sizeChecked = sizeChecked == false ? true : false;  
            //        }
            //    }

            //    list.Add(new CheckboxModel {
            //        Value = siz[i].id.ToString(),
            //        Text = siz[i].size,
            //        Checked = sizeChecked
            //    });
            //}
            // Use this when editing a product: End


            ProductViewModel pvm = new ProductViewModel();
            pvm.Checkboxes = list;

            //if (siz!= null)
            //{
            //    for (int i = 0; i < siz.Count; i++)
            //    {
            //        if (i == 0)
            //            pvm.Checkboxes.Add(new CheckboxModel
            //            {
            //                Value = siz[i].id.ToString(),
            //                Text = siz[i].size,
            //                Checked = true
            //            });
            //        else
            //            pvm.Checkboxes.Add(new CheckboxModel
            //            {
            //                Value = siz[i].id.ToString(),
            //                Text = siz[i].size,
            //                Checked = false
            //            });
            //    }
            //}

             
          
           

           
            //checkboxes: End



            return View(pvm);

        }

        private int ValidateFields(ProductViewModel product)
        {
            isValid = 0;
            if (product.prodName != null && product.prodDescription != null && product.sizeIDs != null)
            {
                isValid = 1;
            }
            return isValid;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Create([Bind("prodDescription")] ProductViewModel product,
        //    IFormFile file)
        public IActionResult Create([Bind("prodDescription,prodName,sizeIDs")] ProductViewModel product, IFormCollection formval)
        {
            int newProductID = 0;
            // Used by the _layout.cshtml (outside this view): Start
            ViewBag.AccountName = _curUser;
            ViewBag.UserAccess = _usr;
            // Used by the _layout.cshtml (outside this view): End

            //checkboxes: start
            var db = _db1.Sizes.ToList();
            ViewBag.listSizes = db.ToList();
            //checkboxes: end

            var i = ValidateFields(product);

            if(i == 1)
            {
                //Insert into Product Table, get newly inserted ID, and redirect to the Image/Insert: start
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "INSERT INTO Product (prodName, prodDescription, prodSizes) output INSERTED.ID VALUES(@prodName, @prodDescription, @prodSizes)";
                        cmd.Parameters.AddWithValue("@prodName", product.prodName);
                        cmd.Parameters.AddWithValue("@prodDescription", product.prodDescription);
                        cmd.Parameters.AddWithValue("@prodSizes", product.sizeIDs);
                        cmd.Connection = con;
                        con.Open();
                        newProductID = (int)cmd.ExecuteScalar();
                        if (con.State == System.Data.ConnectionState.Open)
                            con.Close();
                    }

                    if (newProductID > 0 ) {
                        HttpContext.Session.SetString("newProductID", newProductID.ToString());
                    }
                }
                //Insert into Product Table, get newly inserted ID, and redirect to the Image/Insert: end
            }






           

            //return RedirectToRoute("Images/Create");
            return RedirectToAction("Edit","Image", new { alert = HttpContext.Session.GetString("newProductID") });


            //ModelState.Clear();

            //ProductViewModel model = new ProductViewModel();
            //model.prodName = formval["prodName"];
            //model.prodDescription = formval["prodDescription"];
            //if (formval["sizeIDs"].Count > 0 )
            //{
            //    model.sizeIDs = formval["sizeIDs"].ToString();
            //}


            //TryValidateModel(model);    

            ////==================================: Start
            if (ModelState.IsValid)
            {

            }

            //return RedirectToAction("Index", new { alert = 1 });
            //==================================: End


            //==================================: Start
            //if (ModelState.IsValid)
            //{

            //    var formFile = product.Photo;
            //    if (formFile == null || formFile.Length == 0)
            //    {
            //        ModelState.AddModelError("", "Uploaded file is empty or null.");
            //        return View(product);
            //    }

            //    string fileName = formFile.FileName;
            //    string type = formFile.ContentType;
            //    byte[] bytes = null;
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        product.Photo.CopyTo(ms);
            //        bytes = ms.ToArray();
            //    }
            //    using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            //    {
            //        using (SqlCommand cmd = new SqlCommand())
            //        {
            //            cmd.CommandText = "INSERT INTO dbo.Product(prodCategoryID, prodDescription, fileName, fileContentType, prodData) " +
            //                             " VALUES (@prodCategoryID,@prodDescription, @Name, @ContentType, @Data)";
            //            cmd.Parameters.AddWithValue("@prodCategoryID", 0);
            //            cmd.Parameters.AddWithValue("@prodDescription", product.prodDescription);
            //            cmd.Parameters.AddWithValue("@Name", fileName);
            //            cmd.Parameters.AddWithValue("@ContentType", type);
            //            cmd.Parameters.AddWithValue("@Data", bytes);
            //            cmd.Connection = con;
            //            con.Open();
            //            cmd.ExecuteNonQuery();
            //            con.Close();
            //        }
            //    }
            //    Redirect("/Product");
            //}

            //return RedirectToAction("Index", new { alert = 1 });
            //==================================: End

            //=========================
            //if (ModelState.IsValid)
            //{
            //    //var formFile = postedFiles.Photo;
            //    //if (formFile == null || formFile.Length == 0)
            //    //{
            //    //    ModelState.AddModelError("", "Uploaded file is empty or null.");
            //    //    return View(product);
            //    //}
            //    //string fileName = formFile.FileName;
            //    //string type = formFile.ContentType;

            //    //byte[] bytes = null;
            //    //using (MemoryStream ms = new MemoryStream())
            //    //{
            //    //    product.Photo.CopyTo(ms);
            //    //    bytes = ms.ToArray();
            //    //}
            //    //using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            //    //{
            //    //    using (SqlCommand cmd = new SqlCommand())
            //    //    {
            //    //        cmd.CommandText = "INSERT INTO dbo.Product(prodDescription, fileName, fileContentType, prodData) VALUES (@prodDescription, @Name, @ContentType, @Data)";
            //    //        cmd.Parameters.AddWithValue("@prodDescription", product.prodDescription);
            //    //        cmd.Parameters.AddWithValue("@Name", fileName);
            //    //        cmd.Parameters.AddWithValue("@ContentType", type);
            //    //        cmd.Parameters.AddWithValue("@Data", bytes);
            //    //        cmd.Connection = con;
            //    //        con.Open();
            //    //        cmd.ExecuteNonQuery();
            //    //        con.Close();
            //    //    }
            //    //}
            //}
            //============================


            //foreach (IFormFile postedFile in postedFiles)
            //{
            //    string fileName = Path.GetFileName(postedFile.FileName);
            //    string type = postedFile.ContentType;
            //    byte[] bytes = null;
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        postedFile.CopyTo(ms);
            //        bytes = ms.ToArray();

            //    product.Photo.CopyTo(ms);
            //    }





            //if (ModelState.IsValid)
            //    {


            //        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            //        {
            //            using (SqlCommand cmd = new SqlCommand())
            //            {
            //                cmd.CommandText = "INSERT INTO dbo.Product(prodDescription, fileName, fileContentType, prodData) VALUES (@prodDescription, @Name, @ContentType, @Data)";
            //                cmd.Parameters.AddWithValue("@prodDescription", product.prodDescription);
            //                cmd.Parameters.AddWithValue("@Name", fileName);
            //                cmd.Parameters.AddWithValue("@ContentType", type);
            //                cmd.Parameters.AddWithValue("@Data", bytes);
            //                cmd.Connection = con;
            //                con.Open();
            //                cmd.ExecuteNonQuery();
            //                con.Close();
            //            }
            //        }
            //    }


            //return View(product,",2");
        }

            // Save image to the database



           
        }

        


    //private static List<Product> PopulateFiles(IConfiguration _configuration)
    //{
    //    List<Product> files = new List<Product>();


    //    //DataTable dtbl = new DataTable();
    //    //using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
    //    //{
    //    //    sqlConnection.Open();
    //    //    SqlDataAdapter sqlDa = new SqlDataAdapter("Product_List", sqlConnection);
    //    //    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
    //    //    sqlDa.Fill(dtbl);
    //    //}


    //    using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
    //    {
    //        //string query = "SELECT * FROM tblFiles WHERE ContentType = 'image/jpeg' ORDER BY Id DESC";
    //        string query = "Product_List";
    //        using (SqlCommand cmd = new SqlCommand(query))
    //        {
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Connection = con;
    //            con.Open();
    //            using (SqlDataReader sdr = cmd.ExecuteReader())
    //            {
    //                while (sdr.Read())
    //                {
    //                    files.Add(new Product
    //                    {
    //                        ID = Convert.ToInt32(sdr["Id"]),
    //                        prodDescription = sdr["Name"].ToString(),
    //                        prodData = (byte[])sdr["Data"]
    //                    });
    //                }
    //            }
    //            con.Close();
    //        }
    //    }

    //    return files;
    //}
}

