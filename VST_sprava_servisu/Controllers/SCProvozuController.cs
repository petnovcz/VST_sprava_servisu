using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class SCProvozuController : Controller
    {
        private Model1Container db = new Model1Container();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SCProvozuController");

        // GET: SCProvozu
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Index()
        {
            var sCProvozu = db.SCProvozu
                                .Include(s => s.Provoz)
                                .Include(s => s.SerioveCislo)
                                .Include(s => s.Status)
                                .Include(s => s.Umisteni1)
                                ; //.Include(s=> s.SerioveCislo.Artikl)
            return View(sCProvozu.ToList());
        }

        // GET: SCProvozu/Details/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            if (sCProvozu == null)
            {
                return HttpNotFound();
            }
            return View(sCProvozu);
        }

        // GET: SCProvozu/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu");
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1");
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu");
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni");
            return View();
        }

        // POST: SCProvozu/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,ProvozId,SerioveCisloId,StatusId,DatumPrirazeni,DatumPosledniZmeny,DatumVymeny,Umisteni,DatumRevize,DatumBaterie,DatumPyro,DatumTlkZk,Lokace,Znaceni,BaterieArtikl,UpravenaPeriodaRevize,UkonceniZaruky,UpravenaPeriodaBaterie,UpravenaPeriodaPyro,UpravenaPeriodaTlkZk")] SCProvozu sCProvozu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.SCProvozu.Add(sCProvozu);
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Index");
            }

            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", sCProvozu.ProvozId);
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1", sCProvozu.SerioveCisloId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu", sCProvozu.StatusId);
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni", sCProvozu.Umisteni);
            return View(sCProvozu);
        }

        // GET: SCProvozu/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            if (sCProvozu == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", sCProvozu.ProvozId);
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1", sCProvozu.SerioveCisloId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu", sCProvozu.StatusId);
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni", sCProvozu.Umisteni);
            ViewBag.Zakaznik = db.Provoz.Where(p => p.Id == sCProvozu.ProvozId).Select(p => p.ZakaznikId).FirstOrDefault();
            ViewBag.Provoz = sCProvozu.ProvozId;
            ViewBag.BaterieArtikl = new SelectList(db.Artikl.Where(r=>r.SkupinaArtiklu1.Id==2), "Id", "Nazev", sCProvozu.BaterieArtikl);
            return View(sCProvozu);
        }

        // POST: SCProvozu/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,ProvozId,SerioveCisloId,StatusId,DatumPrirazeni,DatumPosledniZmeny,DatumVymeny,Umisteni,DatumRevize,DatumBaterie,DatumPyro,DatumTlkZk,Lokace,Znaceni,Proverit,Baterie,BaterieArtikl,UpravenaPeriodaRevize,UkonceniZaruky,UpravenaPeriodaBaterie,UpravenaPeriodaPyro,UpravenaPeriodaTlkZk")] SCProvozu sCProvozu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(sCProvozu).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                int Provoz = sCProvozu.ProvozId;
                int Umisteni = sCProvozu.Umisteni.Value;
                int Zakaznik = db.Provoz.Where(p => p.Id == Provoz).Select(p => p.ZakaznikId).FirstOrDefault();
                return RedirectToAction("Details","Umistenis", new { id = Umisteni, Provoz, Zakaznik} );
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", sCProvozu.ProvozId);
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1", sCProvozu.SerioveCisloId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu", sCProvozu.StatusId);
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni", sCProvozu.Umisteni);
            ViewBag.Zakaznik = db.Provoz.Where(p => p.Id == sCProvozu.ProvozId).Select(p => p.ZakaznikId).FirstOrDefault();
            ViewBag.Provoz = sCProvozu.ProvozId;
            ViewBag.BaterieArtikl = new SelectList(db.Artikl.Where(r => r.SkupinaArtiklu1.Id == 2), "Id", "Nazev", sCProvozu.BaterieArtikl);
            return View(sCProvozu);
        }

        // GET: SCProvozu/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            if (sCProvozu == null)
            {
                return HttpNotFound();
            }
            return View(sCProvozu);
        }

        // POST: SCProvozu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            var provoz = sCProvozu.ProvozId;
            var zakaznik = sCProvozu.Provoz.ZakaznikId;
            var umisteni = sCProvozu.Umisteni;
            try
            {
                db.SCProvozu.Remove(sCProvozu);
                db.SaveChanges();
            }
            catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            return RedirectToAction("Details","Umistenis",new { Id = umisteni, Provoz = provoz, Zakaznik = zakaznik });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
