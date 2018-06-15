using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu.Controllers
{
    public class ServisniZasahyController : Controller
    {
        // GET: ServisniZasahy
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index(int? ZakaznikId, string Projekt, DateTime? DatumOd, DateTime? DatumDo, bool? Send, bool? Closed)
        {
            ViewBag.ZakaznikId = new SelectList(Zakaznik.GetAll(), "Id", "NazevZakaznika", ZakaznikId);
            ServisniZasahy sz = new ServisniZasahy();
            sz = ServisniZasahy.GetServisniZasah(ZakaznikId, Projekt,  DatumOd, DatumDo, Send, Closed);
            return View(sz);
        }
    }
}