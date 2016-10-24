using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VideoLoaderMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome To The Acme Corporation!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
