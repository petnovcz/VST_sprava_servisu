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
    public class SerioveCisloesController : Controller
    {
        private Model1Container db = new Model1Container();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SerioveCisloesController");

        // GET: SerioveCisloes
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            var serioveCislo = db.SerioveCislo.Include(s => s.Artikl);
            return View(serioveCislo.ToList());
        }

        // GET: SerioveCisloes/Details/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerioveCislo serioveCislo = db.SerioveCislo.Find(id);
            if (serioveCislo == null)
            {
                return HttpNotFound();
            }
            return View(serioveCislo);
        }

        // GET: SerioveCisloes/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            ViewBag.ArtiklId = new SelectList(db.Artikl, "Id", "Nazev");
            return View();
        }

        // POST: SerioveCisloes/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,ArtiklId,DatumVyroby,DatumPosledniTlakoveZkousky,SerioveCislo1")] SerioveCislo serioveCislo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.SerioveCislo.Add(serioveCislo);
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Index");
            }

            ViewBag.ArtiklId = new SelectList(db.Artikl, "Id", "Nazev", serioveCislo.ArtiklId);
            return View(serioveCislo);
        }

        // GET: SerioveCisloes/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id, int umisteni, int provoz, int zakaznik)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerioveCislo serioveCislo = db.SerioveCislo.Find(id);
            if (serioveCislo == null)
            {
                return HttpNotFound();
            }
            serioveCislo.provoz = provoz;
            serioveCislo.umisteni = umisteni;
            serioveCislo.zakaznik = zakaznik;
            ViewBag.ArtiklId = new SelectList(db.Artikl.Where(r => r.SkupinaArtiklu1.Id != 129), "Id", "Nazev", serioveCislo.ArtiklId);
            return View(serioveCislo);
        }

        // POST: SerioveCisloes/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,ArtiklId,DatumVyroby,DatumPosledniTlakoveZkousky,SerioveCislo1")] SerioveCislo serioveCislo, int provoz, int umisteni, int zakaznik)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(serioveCislo).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Details", "Umistenis", new { id = umisteni, provoz, zakaznik });
            }
            ViewBag.ArtiklId = new SelectList(db.Artikl.Where(r => r.SkupinaArtiklu1.Id != 129), "Id", "Nazev", serioveCislo.ArtiklId);
            return RedirectToAction("Details","Umistenis", new { id = umisteni, provoz, zakaznik });
        }

        // GET: SerioveCisloes/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerioveCislo serioveCislo = db.SerioveCislo.Find(id);
            if (serioveCislo == null)
            {
                return HttpNotFound();
            }
            return View(serioveCislo);
        }

        // POST: SerioveCisloes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            SerioveCislo serioveCislo = db.SerioveCislo.Find(id);
            try
            {
                db.SerioveCislo.Remove(serioveCislo);
                db.SaveChanges();
            }
            catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
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
    }
}
