using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using VideoLoaderMVC.Models;
using VideoLoaderMVC.Repository.Interfaces;
using VideoLoaderMVC.Repository;

namespace VideoLoaderMVC.Controllers
{
    public class AccountController : Controller
    {
        private IMemberRepository _repository;

        //
        // Constructors
        public AccountController()
        {
            /// Create Video Repository           
            _repository = new EntityMemberRepository();
        }

        public AccountController(IMemberRepository repository)
        {
            this._repository = repository;
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            int userID = 0;

            if (ModelState.IsValid)
            {
                userID = _repository.ValidateMember(model.UserName, model.Password);
                if (userID > 0)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    { 
                        //Set UserID Cookie
                        HttpCookie cookie = new HttpCookie("UserID");
                        if (cookie != null)
                        {
                            cookie.Value = userID.ToString();
                            cookie.Expires = DateTime.Now.AddHours(24);
                            ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                        }
                        else
                        {
                            return Redirect(returnUrl); // something failed, redisplay form
                        }
                        
                        return RedirectToAction("Index", "Video");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // something failed, redisplay form
            return View(model);
        }
                
        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            int recordID = 0;

            if (ModelState.IsValid)
            {
                try
                {
                    recordID = _repository.Create(model);
                    if (recordID > 0)
                    {
                        FormsAuthentication.SetAuthCookie(model.Email, false);

                        //Set UserID Cookie
                        HttpCookie cookie = new HttpCookie("UserID");
                        if (cookie != null)
                        {
                            cookie.Value = recordID.ToString();
                            cookie.Expires = DateTime.Now.AddHours(24);
                            ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                        }
                        
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Error: " + e.ToString());
                }
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //No time to implement this feature                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
