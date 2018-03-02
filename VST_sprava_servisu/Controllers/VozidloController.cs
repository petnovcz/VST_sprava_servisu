using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu;

namespace VST_sprava_servisu.Controllers
{
    public class VozidloController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Vozidlo
        public ActionResult Index()
        {
            return View(db.Vozidlo.ToList());
        }

        // GET: Vozidlo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vozidlo vozidlo = db.Vozidlo.Find(id);
            if (vozidlo == null)
            {
                return HttpNotFound();
            }
            return View(vozidlo);
        }

        // GET: Vozidlo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vozidlo/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazevVozidla,CenaZaKm")] Vozidlo vozidlo)
        {
            if (ModelState.IsValid)
            {
                db.Vozidlo.Add(vozidlo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vozidlo);
        }

        // GET: Vozidlo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vozidlo vozidlo = db.Vozidlo.Find(id);
            if (vozidlo == null)
            {
                return HttpNotFound();
            }
            return View(vozidlo);
        }

        // POST: Vozidlo/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazevVozidla,CenaZaKm")] Vozidlo vozidlo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vozidlo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vozidlo);
        }

        // GET: Vozidlo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vozidlo vozidlo = db.Vozidlo.Find(id);
            if (vozidlo == null)
            {
                return HttpNotFound();
            }
            return View(vozidlo);
        }

        // POST: Vozidlo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vozidlo vozidlo = db.Vozidlo.Find(id);
            db.Vozidlo.Remove(vozidlo);
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
