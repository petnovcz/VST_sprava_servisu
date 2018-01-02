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
        public ActionResult Index(int ZakaznikId, int ProvozId, int? UmisteniId, int Rok, int Skupina, string Search)
        {
            GenRevizeCust gr = new GenRevizeCust();
            if (Rok == 0) { Rok = DateTime.Now.Year; }
            gr.Rok = Rok;
            gr.ZakaznikId = ZakaznikId;
            gr.UmisteniId = UmisteniId.Value;
            gr.Skupina = Skupina;
            gr.Search = Search;
            gr.ProvozId = ProvozId;
            
            return View(gr);
        }

        public ActionResult Send(int ZakaznikId, int ProvozId, int? UmisteniId, int Rok, int Skupina, string Search, string Nabidka, string Projekt)
        {
            GenRevizeCust gr = new GenRevizeCust();
            gr.Rok = Rok;
            gr.ZakaznikId = ZakaznikId;
            gr.UmisteniId = UmisteniId.Value;
            gr.Skupina = Skupina;
            gr.Search = Search;
            gr.ProvozId = ProvozId;
            gr.Nabidka = Nabidka;
            gr.Projekt = Projekt;
            GenRevizeCust.Run(ZakaznikId, ProvozId, Rok, UmisteniId, Nabidka, Projekt);
            return View(gr);
        }
    }
}