using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu.Controllers;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu
{
    public class GenerovaniReviziController : Controller
    {
        // GET: GenerovaniRevizi
        private string connectionString = @"Data Source=sql;Initial Catalog=SBO_TEST;User ID=sa;Password=*2012Versino";

        public ActionResult Index()
        {
            GRFiltr gRFiltr = new GRFiltr();
            ViewBag.Rok = Rok.ThisYear();
            ViewBag.Region = new SelectList(Region.GetAll(), "Id", "NazevRegionu");
            ViewBag.Zakaznik = new SelectList(Zakaznik.GetAll(), "Id", "NazevZakaznika");
            

            return View(gRFiltr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Rok,Region,Zakaznik")] GRFiltr grfiltr)
        {
            
            

            
            ViewBag.Rok = grfiltr.Rok;
            ViewBag.Region = new SelectList(Region.GetAll(), "Id", "NazevRegionu", grfiltr.Region);
            ViewBag.Zakaznik = new SelectList(Zakaznik.GetAll(), "Id", "NazevZakaznika", grfiltr.Zakaznik);
            

            return View(grfiltr);
        }

        public ActionResult Provoz()
        {
            List<VypocetPlanuRevizi> list = VypocetPlanuRevizi.Run(connectionString);
            return View(list);
        }


    }
}