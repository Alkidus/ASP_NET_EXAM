using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WritPlat.Models;
using System;
//using System.Web.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Security;

namespace WritPlat.Controllers
{
    public class UserController : Controller
    {
        UserContext db;
        public UserController(UserContext context)
        {
            this.db = context;
            if(db.User.Count() == 0)
            {
                User admin = new User { Email = "admin@gmail.com", UserName = "admin", Password = "admin" };
                db.User.Add(admin);
                db.SaveChanges();
            }
        }
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return Redirect("/home");

            return View(new Models.User());
        }

        [HttpPost]
        public ActionResult Register(Models.User user)
        {
            if (User.Identity.IsAuthenticated) return Redirect("/home");

            //using (ReadBookDB db = new ReadBookDB())
            //{
                if (db.User.Any(x => x.UserName == user.UserName))
                {
                    ViewBag.ErrorMessage = "User Name Already Exist";
                    return View("Register", user);
                }
                if (db.User.Any(x => x.Email == user.Email))
                {
                    ViewBag.ErrorMessage = "Email Already Exist";
                    return View("Register", user);
                }

                db.User.Add(user);
                db.SaveChanges();
            //}

            ViewBag.SuccessMessage = "Registration Suссessful.";
            //FormsAuthentication.SetAuthCookie(user.UserName, false);
            return Redirect("/home");
        }


        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return Redirect("/home");

            return View(new User());
        }

        [HttpPost]
        public ActionResult Login(Models.User _user, string ReturnUrl)
        {
            if (User.Identity.IsAuthenticated) return Redirect("/home");

            //using (ReadBookDB db = new ReadBookDB())
            //{
                var user = db.User.FirstOrDefault(x => x.UserName == _user.UserName);
                if (user == null || user.UserName == "DELETED")
                {
                    ViewBag.ErrorMessage = "Login not existed";
                    return View();
                }
                if (user.Password != _user.Password)
                {
                    ViewBag.ErrorMessage = "Wrong password";
                    return View();
                }
            //}

            //FormsAuthentication.SetAuthCookie(_user.UserName, false);
            if (ReturnUrl == null || ReturnUrl == String.Empty)
            {
                return Redirect("/home");
            }
            return Redirect(ReturnUrl);
        }

        public ActionResult Profile(string id)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
                var user = new Models.User();
                GetCurrentUser(out Models.User currUser);
                if (Int32.TryParse(id, out int ID))
                {
                    user = db.User.FirstOrDefault(x => x.UserId == ID);
                    if (user != null)
                    {
                        if (user.UserId == currUser.UserId)
                        {
                            ViewBag.IsCurrentUser = true;
                        }
                        else
                        {
                            ViewBag.IsCurrentUser = false;
                        }

                        return View(user);
                    }
                }
                else if (User.Identity.IsAuthenticated)
                {
                    ViewBag.IsCurrentUser = true;
                    return Redirect("/user/profile/" + currUser.UserId);
                }
            //}
            return Redirect("/shared/error");
        }

        //[Authorize]
        public ActionResult EditProfile()
        {
            return View(new Models.EditUser());
        }

        [HttpPost, ActionName("EditProfileEmail")]
        //[Authorize]
        public ActionResult EditProfileEmail(Models.EditUser _user)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
                if (db.User.Any(x => x.Email == _user.Email))
                {
                    ViewBag.ErrorMessage = "Email Already Exist.";
                    return View("EditProfile");
                }

                if (_user.CurrentEmailPassword != GetCurrentUser(out Models.User currUser).Password)
                {
                    ViewBag.ErrorMessage = "Wrong password";
                    return View("EditProfile");
                }

                ViewBag.SuccessMessage = "Email changed.";
                currUser.Email = _user.Email;
                //currUser.ConfirmPassword = _user.CurrentEmailPassword;
                currUser.Password = _user.CurrentEmailPassword;
                db.Entry(currUser).State = EntityState.Modified;
                db.SaveChanges();
            //}
            return View("EditProfile");
        }

        [HttpPost, ActionName("EditProfilePassword")]
        //[Authorize]
        public ActionResult EditProfilePassword(Models.EditUser _user)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
                if (_user.CurrentPassword != GetCurrentUser(out Models.User currUser).Password)
                {
                    ViewBag.ErrorMessage = "Wrong password";
                    return View("EditProfile");
                }

                ViewBag.SuccessMessage = "Password changed.";
                currUser.Password = _user.NewPassword;
                //currUser.ConfirmPassword = _user.NewPassword;
                db.Entry(currUser).State = EntityState.Modified;
                db.SaveChanges();
            //}
            return View("EditProfile");
        }

        //[Authorize]
        public ActionResult LogOut()
        {
            //FormsAuthentication.SignOut();
            return Redirect("/home");
        }


        //[Authorize]
        public ActionResult DeleteProfile()
        {
            return View(new EditUser());
        }

        [HttpPost]
        //[Authorize]
        public ActionResult DeleteProfile(EditUser user)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
                if (GetCurrentUser(out Models.User currUser).Password == user.CurrentPassword)
                {
                    //FormsAuthentication.SignOut();
                    currUser.UserName = "DELETED";
                    currUser.Password = "D";
                    //currUser.ConfirmPassword = "D";
                    currUser.Email = "D";
                    db.Entry(currUser).State = EntityState.Modified;
                    db.SaveChanges();
                    return Redirect("/home");
                }
            //}

            ViewBag.ErrorMessage = "Wrong password";
            return View(new EditUser());
        }

        public Models.User GetCurrentUser()
        {
            var user = new Models.User();
            //using (ReadBookDB db = new ReadBookDB())
            //{
                GetCurrentUser(out user);
            //}
            return user;
        }

        public Models.User GetCurrentUser(out Models.User user)
        {
            if (User.Identity.IsAuthenticated)
            {
                return user = db.User.FirstOrDefault(x => x.UserName == User.Identity.Name);
            }
            return user = new Models.User();
        }
    }
}
