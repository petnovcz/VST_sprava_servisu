using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;


namespace VST_sprava_servisu.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Administrator,Manager,Uživatel,SIL,Vedení")]
        public ActionResult Index()
        {
            return RedirectToAction("Nahled","Revize",null);
        }
        [Authorize(Roles = "Administrator,Manager,Uživatel,SIL,Vedení")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize(Roles = "Administrator,Manager,Uživatel,SIL,Vedení")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }

    
}