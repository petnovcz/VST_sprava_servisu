using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu.Controllers
{
    public class NastaveniController : Controller
    {
        // GET: Nastaveni
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View();
        }
    }
}