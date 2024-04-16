
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PH_Swag.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using PH_Swag.Services;


namespace PH_Swag.Controllers
{
    public class TestLoginController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private string _curUser;
        private PH_Swag.Models.UserAccess _usr;
        private readonly IConfiguration _configuration;
        private PH_SwagEntity _db1;

        public TestLoginController(
                ILogger<HomeController> logger,
                PH_SwagEntity db,
                IConfiguration configuration,
            [FromServices] LoginService ls, [FromServices] CartService cs) : base(db, ls, cs)
        {
            _db1 = db;
            _logger = logger;
            _curUser = ViewBag.AccountName;
            _usr = ViewBag.UserAccess;
            _configuration = configuration;


        }



        // List of Products
        public IActionResult Index(int? page, string Order, int? alert, string filter)
        {
            string connstr = _configuration.GetConnectionString("DevConnection");

            // Used by the _layout.cshtml (outside this view): Start
            ViewBag.AccountName = _curUser;
            ViewBag.UserAccess = _usr;
            int a = 0;
            // Used by the _layout.cshtml (outside this view): End

            string viewfilter = (string.IsNullOrEmpty(filter)) ? "Show All" : filter;
            int PageNumber = (page > 0) ? (int)page : 1;
            string SortOrder = string.IsNullOrEmpty(Order) ? "ID" : Order;
            ViewBag.Alert = alert.HasValue ? alert : 0;


            ViewData["PatientSort"] = SortOrder == "Patient" ? "Patient_desc" : "Patient";
            ViewData["CCSSort"] = SortOrder == "CCSName" ? "CCSName_desc" : "CCSName";
            ViewData["ServiceDateSort"] = SortOrder == "ServiceDate" ? "ServiceDate_desc" : "ServiceDate";
            ViewData["PickupSort"] = SortOrder == "Pickup" ? "Pickup_desc" : "Pickup";
            ViewData["DateAddedSort"] = SortOrder == "DateAdded" ? "DateAdded_desc" : "DateAdded";
            ViewData["AmountSort"] = SortOrder == "Amount" ? "Amount_desc" : "Amount";
            ViewData["MethodSort"] = SortOrder == "Method" ? "Method_desc" : "Method";
            ViewData["PickupDateSort"] = SortOrder == "PickupDate" ? "PickupDate_desc" : "PickupDate";

            ViewBag.SortOrder = SortOrder;
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
                       : row.Field<string>("catDescription")

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
            const int pageSize = 1;
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


        public IActionResult Create()
        {
            ProductViewModel loginViewModel = new ProductViewModel();
            //loginViewModel.prodDescription = String.Empty;
            return View(loginViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(int id, [Bind("test")] ProductViewModel productViewModel)
        {
            return View(productViewModel);

            //LoginModel loginViewModel = new LoginModel();
            //loginViewModel.strLoginMessage = "";
            //isLogin = 0;

            ////BookAddOrEdit
            //if (ModelState.IsValid)
            //{
            //    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            //    {
            //        string strSql = "";

            //        strSql = "Select DISTINCT " +
            //                " o.UDF_ONLINE_PASSWORD, f.FACILITY_ID, gp.DESIG_EMPLOYEE, dba.CD_EMPLOYEE.LAST_NAME, dba.CD_EMPLOYEE.FIRST_NAME, " +
            //                " dba.CD_EMPLOYEE.PHONE, dba.CD_EMPLOYEE.OFFICE_PHONE,  dba.CD_EMPLOYEE.EMAIL, " +
            //                " f.FACILITY_NAME, f.SITE_ADDRESS, f.CITY,  f.CERS_ID, f.ADDRESS2 " +
            //            " FROM dba.TB_CORE_OWNER As o INNER JOIN " +
            //                " dba.TB_CORE_FACILITY As f On o.OWNER_ID = f.FACILITY_OWNER_ID INNER JOIN " +
            //                " dba.TB_CORE_GENERAL_PROGRAM As gp On f.FACILITY_ID = gp.FACILITY_ID INNER JOIN " +
            //                " dba.CD_EMPLOYEE On gp.DESIG_EMPLOYEE = dba.CD_EMPLOYEE.EMPLOYEE_ID " +
            //             " WHERE (gp.PE In ('CS00')) AND (gp.CURRENT_STATUS NOT IN ('02')) AND (f.CERS_ID = '" + loginModel.username + "')";

            //        sqlConnection.Open();
            //        SqlCommand sqlCmd = new SqlCommand(strSql, sqlConnection);
            //        sqlCmd.CommandType = System.Data.CommandType.Text;

            //        loginViewModel.strLoginMessage = "Login Failed";
            //        try
            //        {
            //            using (SqlDataReader reader = sqlCmd.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    if (reader[0].ToString().Length > 0)
            //                    {
            //                        if (reader[0].ToString().ToUpper() == loginModel.password.ToUpper())
            //                        {
            //                            loginViewModel.strLoginMessage = "Login Success";
            //                            // Setting A Session
            //                            HttpContext.Session.SetString("FacilityID", reader[1].ToString());
            //                            isLogin = 1;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        loginViewModel.strLoginMessage = "Login Failed";
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception)
            //        {

            //            throw;
            //        }
            //    }

            //    //return RedirectToAction(nameof(Index));
            //    //return RedirectToAction("Index", "Login", loginViewModel);
            //    // return View(loginViewModel);
            //}

            //if (isLogin == 1)
            //{
            //    return RedirectToAction("Index", "MainMenu");
            //}
            //return View(loginViewModel);


        }




        //[NonAction]
        //private LoginModel FetchBookByID(int? id)
        //{
        //    LoginModel bookViewModel = new LoginModel();
        //    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
        //    {
        //        DataTable dtbl = new DataTable();
        //        sqlConnection.Open();
        //        SqlDataAdapter sqlDa = new SqlDataAdapter("BookViewByID", sqlConnection);
        //        sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
        //        sqlDa.SelectCommand.Parameters.AddWithValue("BookID", id);
        //        sqlDa.Fill(dtbl);
        //        if (dtbl.Rows.Count == 1)
        //        {
        //            bookViewModel.BookID = Convert.ToInt32(dtbl.Rows[0]["BookID"].ToString());
        //            bookViewModel.Title = dtbl.Rows[0]["Title"].ToString();
        //            bookViewModel.Author = dtbl.Rows[0]["Author"].ToString();
        //            bookViewModel.Price = Convert.ToInt32(dtbl.Rows[0]["Price"].ToString());
        //        }
        //        return bookViewModel;
        //    }


        //}

    }
}
