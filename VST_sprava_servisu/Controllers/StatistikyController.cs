using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu.Controllers
{
    public class StatistikyController : Controller
    {
        // GET: Statistiky
        [Authorize(Roles = "Administrator,Vedení")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Administrator,Vedení")]
        public ActionResult CelkovyManazerskyPrehled(DateTime? DatumOd, DateTime? DatumDo)
        {
            CelkovyManazerskyPrehled cmp = new CelkovyManazerskyPrehled();
            if (DatumOd == null) { DatumOd = new DateTime(DateTime.Now.Year, 1, 1); }
            if (DatumDo == null) { DatumDo = new DateTime(DateTime.Now.Year, 12, 31); }
            cmp.DatumOd = DatumOd.Value;
            cmp.DatumDo = DatumDo.Value;
            return View(cmp);
        }
    }
}