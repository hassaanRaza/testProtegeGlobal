using protechGlobal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace protechGlobal.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            var profileData = this.Session["UserProfile"] as User_PG;
            if(profileData != null)
            {
                return View(profileData);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
    }
}