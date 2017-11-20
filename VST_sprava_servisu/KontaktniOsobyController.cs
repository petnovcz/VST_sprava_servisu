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
    public class KontaktniOsobyController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: KontaktniOsoby
        public ActionResult Index(int Zakaznik)
        {
            var kontakniOsoba = db.KontakniOsoba.Include(k => k.Zakaznik).Include(k => k.Provoz);
            kontakniOsoba.Where(m => m.ZakaznikId == Zakaznik);
            return View(kontakniOsoba.ToList());
        }

        // GET: KontaktniOsoby/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            return View(kontakniOsoba);
        }

        // GET: KontaktniOsoby/Create
        public ActionResult Create(int Zakaznik)
        {
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", Zakaznik);
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m=>m.ZakaznikId == Zakaznik), "Id", "NazevProvozu");
            return View();
        }

        // POST: KontaktniOsoby/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ZakaznikId,JmenoPrijmeni,Pozice,Telefon,Email,SAPId,ProvozId")] KontakniOsoba kontakniOsoba)
        {
            if (ModelState.IsValid)
            {
                db.KontakniOsoba.Add(kontakniOsoba);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.ZakaznikId == kontakniOsoba.ZakaznikId), "Id", "NazevProvozu", kontakniOsoba.ProvozId);
            return View(kontakniOsoba);
        }

        // GET: KontaktniOsoby/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.ZakaznikId == kontakniOsoba.ZakaznikId), "Id", "NazevProvozu", kontakniOsoba.ProvozId);
            return View(kontakniOsoba);
        }

        // POST: KontaktniOsoby/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ZakaznikId,JmenoPrijmeni,Pozice,Telefon,Email,SAPId,ProvozId")] KontakniOsoba kontakniOsoba)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kontakniOsoba).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.ZakaznikId == kontakniOsoba.ZakaznikId), "Id", "NazevProvozu", kontakniOsoba.ProvozId);
            return View(kontakniOsoba);
        }

        // GET: KontaktniOsoby/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            return View(kontakniOsoba);
        }

        // POST: KontaktniOsoby/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            db.KontakniOsoba.Remove(kontakniOsoba);
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
