using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Web;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PH_Swag.Models;
using PH_Swag.Services;

namespace PH_Swag.Controllers
{
    public class BaseController : Controller
    {
        
        public bool bAuditActivity = true;

        // for LAB deployment:
        // 1. set bTestMode above to true
        // 2. change build config to debug
        // 3. in web.config, change connection profile to LAB (EHTESTAPP2)
        // 4. use publish folderprofile-Lab

        public string FileServerPath = "\\\\phfile01\\Vol1_PH\\APPLIC\\GasCard\\Uploads";
        
        //private readonly PH_Swag.Models.PH_SwagEntity db = new PH_Swag.Models.PH_SwagEntity();
        public PH_Swag.Models.UserAccess? usr;

        // My code
        //protected PH_SwagEntity db => (PH_SwagEntity)HttpContext.RequestServices.GetService(typeof(PH_SwagEntity));

        public readonly PH_SwagEntity _db;
        public  int userID;




        public BaseController(PH_SwagEntity db, [FromServices] LoginService ls, [FromServices] CartService cs)
        {
            _db = db;
            userID = 0;

            usr = new PH_Swag.Models.UserAccess()
            {
                UserName = ViewBag.AccountName = GetUserName(),
                Level1 = false,
                Level2 = false,
                Level3 = false,
                Level4 = false,
                Level5 = false,
                Level6 = false
            };


            ViewBag.ShowMenu = true;
            if (usr.UserName.Length > 0)
            {
                ViewBag.AccountName = usr.UserName;
                if (_db.UserAccesses.Any(t => t.UserName == usr.UserName && t.VoidFlag == false))
                {
                    //usr = db.UserAccesses.FirstOrDefault(t => t.UserName == usr.UserName && t.VoidFlag == false);
                    usr = _db.UserAccesses.FirstOrDefault(t => t.UserName == usr.UserName && t.VoidFlag == false);
                }
            }
            else
            {
                ViewBag.UserName = "Unknown User";
            }
            ViewBag.UserAccess = usr;
            


            //ViewBag.CartItemTotal = "12";
            //============================================================================================


            //============================ Get SwagBucks:  Uisng SQL: Start
            ////1) Get current user Logged on
            //var username = GetUserName();
            //var swagBucks= 0;
            ////2) Get Swag Bucks
            //if (username !=null)
            //{
            //    swagBucks = cs.GetSwagBucks(username);
            //    userID = cs._userID;


            //}
            //ViewBag.UserID = userID.ToString();
            //ViewBag.SwagBucks = swagBucks.ToString();
            //============================ Get SwagBucks:  Uisng SQL: End



            // =================== Get Global Info (userID, username, cartID, #items in cart, swagBucks: start
            var username = GetUserName();
            var initVal = 0;
            if (username != null)
            {
                initVal = cs.Initialize(username);
                if(cs._isRecFound > 0)
                {
                    ViewBag.UserID = cs._userID; 
                    ViewBag.UserName = username;
                    ViewBag.NumItems = cs._numItems;
                    ViewBag.CartID = cs._cartID;
                    ViewBag.SwagBucks = cs._swagBucks;
                }
            }

            // =================== Get Global Info (userID, username, cartID, #items in cart, swagBucks: start




        }

        public string GetUserName()
        {
            string strAccountName = "";
            //var strADUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string strADUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //string strADUser = System.Web.HttpContext.Current.User.Identity.Name;
            //string? strADUser =  User?.Identity?.Name?.Split("\\");
            int iPos = strADUser.IndexOf('\\');
            if (iPos > 0)
            {
                strAccountName = strADUser.Substring(iPos + 1).ToUpper();
            }
            return strAccountName;
        }

        //public int AddAuditTrail(string st, int? i)
        //{
        //    if (bAuditActivity)
        //    {
        //        var at = new GasCard.Models.AuditTrail()
        //        {
        //            Activity = st.Length > 50 ? st.Substring(0, 50) : st,
        //            DateUpdated = DateTime.Now,
        //            Username = usr.UserName,
        //            RecordID = i.HasValue ? i : 0
        //        };
        //        db.AuditTrails.Add(at);
        //        db.SaveChanges();
        //        return at.ID;
        //    }
        //    return 0;
        //}


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
