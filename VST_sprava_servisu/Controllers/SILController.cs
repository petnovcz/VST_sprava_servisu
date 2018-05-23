using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;


namespace VST_sprava_servisu
{
    public class SILController : Controller
    {
        // GET: SIL
        [Authorize(Roles = "Administrator,SIL")]
        public ActionResult Index(int? Rok)
        {
            SIL sil = new SIL();
            if (Rok == null) { Rok = DateTime.Now.Year; }
            sil.Rok = Rok.Value;



            return View(sil);
        }
    }
}