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

namespace VST_sprava_servisu
{
    public class GenerovaniReviziController : Controller
    {
        // GET: GenerovaniRevizi
        private string connectionString = @"Data Source=sql;Initial Catalog=SBO_TEST;User ID=sa;Password=*2012Versino";
        private Model1Container db = new Model1Container();
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
            ReportDocument rd = new ReportDocument();
            // Your .rpt file path will be below
            rd.Load(Path.Combine(Server.MapPath("~/CrystalReport2.rpt")));
            //set dataset to the report viewer.
            rd.SetParameterValue("DocKey@", "1");
            rd.SetParameterValue("ObjectId@", "15");
            //rd.ParameterFields.
            rd.SetDatabaseLogon("sa", "*2012Versino",
                               "SQL", "SBO", false);
            ;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            str.Seek(0, SeekOrigin.Begin);
            string savedFilename = string.Format("OrderListing_{0}", DateTime.Now);

            return File(str, "application/pdf", savedFilename);
        }

        

    }
}