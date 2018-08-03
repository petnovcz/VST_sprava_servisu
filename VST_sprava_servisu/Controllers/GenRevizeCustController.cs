using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu.Controllers
{
    public class GenRevizeCustController : Controller
    {
        // GET: GenRevizeCust
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index(int ZakaznikId, int ProvozId, int? UmisteniId, int Rok, int Skupina, string Search)
        {
            GenRevizeCust gr = new GenRevizeCust();
            if (Rok == 0) { Rok = DateTime.Now.Year; }
            gr.Rok = Rok;
            gr.ZakaznikId = ZakaznikId;
            try
            {
                gr.UmisteniId = UmisteniId.Value;
            }
            catch (Exception ex) { }
            gr.Skupina = Skupina;
            gr.Search = Search;
            gr.ProvozId = ProvozId;

            //ViewBag.VozidloId = new SelectList(db.Vozidlo, "Id", "NazevVozidla", 1);
            ViewBag.Projekt = new SelectList(gr.ProjektList, "Code", "Name",null); 
            ViewBag.Nabidka = new SelectList(gr.NabidkaList, "Code", "Name", null);

            return View(gr);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Send(int ZakaznikId, int ProvozId, int? UmisteniId, int Rok, int Skupina, string Search, string Nabidka, string Projekt)
        {
            GenRevizeCust gr = new GenRevizeCust();
            gr.Rok = Rok;
            gr.ZakaznikId = ZakaznikId;
            try
            {
                gr.UmisteniId = UmisteniId.Value;
            }
            catch (Exception ex) { }
            gr.Skupina = Skupina;
            gr.Search = Search;
            gr.ProvozId = ProvozId;
            gr.Nabidka = Nabidka;
            gr.Projekt = Projekt;
            GenRevizeCust.Run(ZakaznikId, ProvozId, Rok, UmisteniId, Nabidka, Projekt);
            return RedirectToAction("Index","Revize",new { Zakaznik = ZakaznikId });
        }
    }
}