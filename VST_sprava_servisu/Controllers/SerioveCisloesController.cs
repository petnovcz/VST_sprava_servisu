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

        // GET: SerioveCisloes
        public ActionResult Index()
        {
            var serioveCislo = db.SerioveCislo.Include(s => s.Artikl);
            return View(serioveCislo.ToList());
        }

        // GET: SerioveCisloes/Details/5
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
        public ActionResult Create([Bind(Include = "Id,ArtiklId,DatumVyroby,DatumPosledniTlakoveZkousky,SerioveCislo1")] SerioveCislo serioveCislo)
        {
            if (ModelState.IsValid)
            {
                db.SerioveCislo.Add(serioveCislo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArtiklId = new SelectList(db.Artikl, "Id", "Nazev", serioveCislo.ArtiklId);
            return View(serioveCislo);
        }

        // GET: SerioveCisloes/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.ArtiklId = new SelectList(db.Artikl, "Id", "Nazev", serioveCislo.ArtiklId);
            return View(serioveCislo);
        }

        // POST: SerioveCisloes/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ArtiklId,DatumVyroby,DatumPosledniTlakoveZkousky,SerioveCislo1")] SerioveCislo serioveCislo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serioveCislo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArtiklId = new SelectList(db.Artikl, "Id", "Nazev", serioveCislo.ArtiklId);
            return View(serioveCislo);
        }

        // GET: SerioveCisloes/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            SerioveCislo serioveCislo = db.SerioveCislo.Find(id);
            db.SerioveCislo.Remove(serioveCislo);
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
    }
}
