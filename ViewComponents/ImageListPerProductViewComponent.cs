using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PH_Swag.Models;

namespace PH_Swag.ViewComponents
{
    public class ImageListPerProductViewComponent : ViewComponent
    {
        private readonly PH_SwagEntity db;
        private readonly IConfiguration iconfig;

        public ImageListPerProductViewComponent(PH_SwagEntity db, IConfiguration iconfig)
        {
            this.db = db;
            this.iconfig = iconfig;
        }


        public IViewComponentResult Invoke(int? prodID = 0)
        {
            //var result = db.Images.ToList();
            //return View(result);

            List<Image> files = new List<Image>();
            using (SqlConnection con = new SqlConnection(iconfig.GetConnectionString("DevConnection")))
            {
                //string query = "SELECT * FROM tblFiles WHERE ContentType = 'image/jpeg' ORDER BY Id DESC";
                string query = "SELECT top 5 Id, prodImageName, prodData FROM Images WHERE productID = " + prodID + " and prodImageContentType like 'image/%' ORDER BY id DESC";
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
                                ID = Convert.ToInt32(sdr["Id"]),
                                prodImageName = sdr["prodImageName"].ToString(),
                                prodData = (byte[])sdr["prodData"]
                            });
                        }
                    }
                    con.Close();
                }
            }

            return View(files);
        }
    }
}
