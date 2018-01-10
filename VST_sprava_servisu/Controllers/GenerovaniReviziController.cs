using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Reporting;
using System.IO;
using CrystalDecisions.Shared;
using System.Configuration;

namespace VST_sprava_servisu
{
    public class GenerovaniReviziController : Controller
    {
        // GET: GenerovaniRevizi
        //private string connectionString = @"Data Source=sql;Initial Catalog=SBO;User ID=sa;Password=*2012Versino";
        private string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;

        private Model1Container db = new Model1Container();

        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index([Bind(Include = "Rok,Region,Zakaznik")] GRFiltr grfiltr)
        {
            ViewBag.Rok = grfiltr.Rok;
            ViewBag.Region = new SelectList(Region.GetAll(), "Id", "NazevRegionu", grfiltr.Region);
            ViewBag.Zakaznik = new SelectList(Zakaznik.GetAll(), "Id", "NazevZakaznika", grfiltr.Zakaznik);
            return View(grfiltr);
        }

        /// <summary>
        /// Spuštění výpočtu revizí
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult RunGenerator(int Year)
        {
            List<VypocetPlanuRevizi> list = VypocetPlanuRevizi.Run(connectionString, Year);
            return RedirectToAction("Nahled", "Revize", null);
        }
    }
}