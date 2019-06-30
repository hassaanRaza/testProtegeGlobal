using protechGlobal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace protechGlobal.Controllers
{
    public class AccountController : Controller
    {
        ProtechGlobalEntities db = new ProtechGlobalEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            var UserFromSession = this.Session["UserProfile"] as User_PG;
            if (UserFromSession != null)
            {
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Login()
        {
            var UserFromSession = this.Session["UserProfile"] as User_PG;
            if(UserFromSession != null)
            {
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                return View();
            }
            
        }
        public class ResultModel
        {
            public bool status;
            public string message;
        }
        public JsonResult AddUser(User_PG model)
        {
            var checkUserAlreadyExistsOrNot = db.User_PG.Where(a => a.Email.ToLower() == model.Email.ToLower()).FirstOrDefault();
            if (checkUserAlreadyExistsOrNot == null)
            {
                //Encode password//
                string encodePassword = EncodePasswordToBase64(model.Password);
                model.Password = encodePassword;
                //End
                //Add new user
                db.User_PG.Add(model);
                db.SaveChanges();
                return Json(new ResultModel { status = true, message = "User added successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ResultModel { status = false, message = "User with this email is already exists!"}, JsonRequestBehavior.AllowGet);
            }
            
        }
        public JsonResult LoginUser(User_PG model)
        {
            //Encode Password//
            string encodePassword = EncodePasswordToBase64(model.Password);
            model.Password = encodePassword;
            //End//
            var user = db.User_PG.Where(a => 
            (a.Email.ToLower() == model.Email.ToLower() || a.Username.ToLower() == model.Email.ToLower())
            && a.Password == model.Password).FirstOrDefault();

            if(user != null)
            {
                var UserInSession = new User_PG
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Age = user.Age
                };
                this.Session["UserProfile"] = UserInSession;
                return Json(new ResultModel { status = true, message = "Login sucessfull." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ResultModel { status = false, message = "Email or password is invalid." }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session["UserProfile"] = null;
            return RedirectToAction("Login", "Account");
        }
        //this function Convert to Encord your Password 
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        //this function Convert to Decord your Password
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

    }
}