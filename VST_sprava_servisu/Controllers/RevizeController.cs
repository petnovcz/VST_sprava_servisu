using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace VST_sprava_servisu
{
    public class RevizeController : Controller
    {
        private Model1Container db = new Model1Container();
        private string connectionString = @"Data Source=sql;Initial Catalog=SBO_TEST;User ID=sa;Password=*2012Versino";

        // GET: Revize
        public ActionResult Index()
        {
            var revize = db.Revize.Include(r => r.Provoz).Include(r => r.StatusRevize);
            return View(revize.ToList());
        }

        // GET: Revize/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            
            return View(revize);
        }

        // GET: Revize/Create
        public ActionResult Create()
        {
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu");
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize");
            return View();
        }

        // POST: Revize/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti")] Revize revize)
        {
            if (ModelState.IsValid)
            {
                db.Revize.Add(revize);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // GET: Revize/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // POST: Revize/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti")] Revize revize)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details","Revize",new { Id = revize.Id});
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // GET: Revize/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            return View(revize);
        }

        // POST: Revize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Revize revize = db.Revize.Find(id);
            db.Revize.Remove(revize);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult MonthViewHeader(int Rok, int Mesic)
        {
            ViewBag.ThisYear = DateTime.Now.Year;
            ViewBag.NextYear = DateTime.Now.Year + 1;
            ViewBag.Rok = Rok;
            ViewBag.Mesic = Mesic;
            return View();
        }
        public ActionResult Nahled (int? Rok, int? Mesic)
        {
            if (Rok == null) { Rok = System.DateTime.Now.Year; }
            if (Mesic == null) { Mesic = System.DateTime.Now.Month; }
            
            DateTime date1 = new DateTime(Rok.Value, Mesic.Value, 1);
            var x = (int)date1.DayOfWeek - 1;

            var startOfMonth = new DateTime(date1.Year, date1.Month, 1);
            var DaysInMonth = DateTime.DaysInMonth(date1.Year, date1.Month);

            ViewBag.Rok = Rok;
            ViewBag.Mesic = Mesic;
            ViewBag.X = x;
            ViewBag.DaysInMonth = DaysInMonth;
            return View();
        }

        public ActionResult DateView(int? Rok, int? Mesic, int? Den)
        {
            
            if (Rok == null) { Rok = System.DateTime.Now.Year; }
            if (Mesic == null) { Mesic = System.DateTime.Now.Month; }
            if (Den == null) { Mesic = System.DateTime.Now.Day; }

            List<Revize> list = new List<Revize>();
            list = Revize.GetByDate(Mesic.Value, Rok.Value, Den.Value);


            return View(list);
        }

        // GET: Revize/Edit/5
        public ActionResult Replan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Replan([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti")] Revize revize)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Nahled", "Revize", new { Rok = revize.DatumRevize.Year, Mesic = revize.DatumRevize.Month });
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        public ActionResult TiskZaznamuOKontrole(int Id)
        {
            List<VypocetPlanuRevizi> list = VypocetPlanuRevizi.Run(connectionString);
            ReportDocument rd = new ReportDocument();
            // Your .rpt file path will be below
            rd.Load(Path.Combine(Server.MapPath("~/Servis.rpt")));
            //set dataset to the report viewer.
            rd.SetParameterValue("Id@", Id);
            //rd.ParameterFields.
            rd.SetDatabaseLogon("sa", "*2012Versino",
                               "SQL", "SBO", false);
            ;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            str.Seek(0, SeekOrigin.Begin);
            string savedFilename = string.Format("Revize_{0}.pdf", Id);

            return File(str, "application/pdf", savedFilename);
        }

        public void OpenPDF(int Id)
        {
            ReportDocument Rel = new ReportDocument();
            Rel.Load(Path.Combine(Server.MapPath("~/Servis.rpt")));
            Rel.SetParameterValue("Id@", Id);
            //rd.ParameterFields.
            Rel.SetDatabaseLogon("sa", "*2012Versino",
                               "SQL", "Servis", false);
            
            BinaryReader stream = new BinaryReader(Rel.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
            Response.Flush();
            Response.Close();
        }



    }
}
