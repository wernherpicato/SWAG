using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PH_Swag.Models;
using PH_Swag.Services;
using System.IO;

namespace PH_Swag.Controllers
{

    public class ImageController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private string _curUser;
        private PH_Swag.Models.UserAccess _usr;
        private readonly IConfiguration _configuration;
        private PH_SwagEntity _db1;

        private int productID = 0;


        public ImageController(
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
        public IActionResult Index()
        {
            ViewBag.AccountName = _curUser;
            ViewBag.UserAccess = _usr;


            Image tb = new Image();

            return View(tb);
        }


        public IActionResult Create()
        {
            ViewBag.AccountName = _curUser;
            ViewBag.UserAccess = _usr;

            ViewBag.newProductID = HttpContext.Session.GetString("newProductID");

            return View();
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewBag.AccountName = _curUser;
            ViewBag.UserAccess = _usr;

            int localID = 0;

            if (id != null)
            {
                localID = (int)id;
            }

            if (localID == 0)
            {
                return RedirectToAction("Index", "Home");
            }


            if (HttpContext.Session.GetString("newProductID") != null)
            {
                localID = Int32.Parse(HttpContext.Session.GetString("newProductID"));
            }

            // if there is no localID passed, redirect to home page.
            if (localID == 0) { return RedirectToAction("Index", "Home"); }

            return View(PopulateFiles(_configuration, localID));
        }



        [HttpPost]
        public IActionResult Edit(int? id, List<IFormFile> postedFiles)
        {
            int localID = 0;

            if (id != null)
            {
                localID = (int)id;
            }

            if (HttpContext.Session.GetString("newProductID") != null)
            {
                localID = Int32.Parse(HttpContext.Session.GetString("newProductID"));
            }

            // if there is no localID passed, redirect to home page.
            if (localID == 0) { return RedirectToAction("Index", "Home"); }


            if (localID != null)
            {
                // Upload files: start
                foreach (IFormFile postedFile in postedFiles)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string type = postedFile.ContentType;
                    byte[] bytes = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        postedFile.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                    using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "INSERT INTO Images (productID, prodImageName, prodImageContentType, prodData) VALUES (@productID, @prodImageName, @prodImageContentType, @prodData)";
                            cmd.Parameters.AddWithValue("@productID", localID);
                            cmd.Parameters.AddWithValue("@prodImageName", fileName);
                            cmd.Parameters.AddWithValue("@prodImageContentType", type);
                            cmd.Parameters.AddWithValue("@prodData", bytes);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                // Upload files: End
            }




            return View(PopulateFiles(_configuration, localID));
        }

        private static List<Image> PopulateFiles(IConfiguration iconfig, int id)
        {
            List<Image> files = new List<Image>();



            //using (SqlConnection con = new SqlConnection(iconfig.GetConnectionString("EHTESTAPP2")))
            using (SqlConnection con = new SqlConnection(iconfig.GetConnectionString("DevConnection")))
            {
                //string query = "SELECT * FROM tblFiles WHERE ContentType = 'image/jpeg' ORDER BY Id DESC";
                string query = "SELECT id, productID, prodImageName, prodImageContentType, prodData FROM Images WHERE  productID =  " + id.ToString() + " and prodImageContentType like 'image/%' ORDER BY Id DESC";


                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new Image
                            {
                                productID = Convert.ToInt32(sdr["id"]),
                                prodImageName = sdr["prodImageName"].ToString(),
                                prodData = (byte[])sdr["prodData"]
                            });
                        }
                    }
                    con.Close();
                }
            }

            return files;
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                ViewBag.newProductID = id;
            }

            if (HttpContext.Session.GetString("newProductID") != null)
            {
                ViewBag.newProductID = HttpContext.Session.GetString("newProductID");
            }


            //Delete Image Here: Start

            //Delete Image Here: End


            return View(PopulateFiles(_configuration, ViewBag.newProductID));
        }
    }
}
