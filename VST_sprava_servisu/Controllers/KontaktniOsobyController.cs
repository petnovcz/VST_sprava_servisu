using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class KontaktniOsobyController : Controller
    {
        private Model1Container db = new Model1Container();
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: KontaktniOsoby
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index(int Zakaznik)
        {
            var kontakniOsoba = db.KontakniOsoba.Include(k => k.Provoz).Include(k => k.Zakaznik).Where(k=>k.ZakaznikId == Zakaznik);
            ViewBag.Zakaznik = Zakaznik;

            
            return View(kontakniOsoba.ToList());
        }

        // GET: KontaktniOsoby/Details/5
        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create(int Zakaznik)
        {
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m=>m.ZakaznikId == Zakaznik), "Id", "NazevProvozu");
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", Zakaznik);
            ViewBag.Zakaznik = Zakaznik;
            return View();
        }

        // POST: KontaktniOsoby/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,ZakaznikId,JmenoPrijmeni,Pozice,Telefon,Email,SAPId,ProvozId")] KontakniOsoba kontakniOsoba)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.KontakniOsoba.Add(kontakniOsoba);
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Index");
            }

            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.ZakaznikId == kontakniOsoba.ZakaznikId), "Id", "NazevProvozu", kontakniOsoba.ProvozId);
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            return View(kontakniOsoba);
        }

        



        // GET: KontaktniOsoby/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            int Zakaznik = kontakniOsoba.ZakaznikId;
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.ZakaznikId == kontakniOsoba.ZakaznikId), "Id", "NazevProvozu", kontakniOsoba.ProvozId);
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            ViewBag.Zakaznik = Zakaznik;
            return View(kontakniOsoba);
        }

        // POST: KontaktniOsoby/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,ZakaznikId,JmenoPrijmeni,Pozice,Telefon,Email,SAPId,ProvozId")] KontakniOsoba kontakniOsoba)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(kontakniOsoba).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Index", "KontaktniOsoby", new { Zakaznik = kontakniOsoba.ZakaznikId });
            }
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.ZakaznikId == kontakniOsoba.ZakaznikId), "Id", "NazevProvozu", kontakniOsoba.ProvozId);
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            return View(kontakniOsoba);
        }

        // GET: KontaktniOsoby/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            int Zakaznik = kontakniOsoba.ZakaznikId;
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            ViewBag.Zakaznik = Zakaznik;
            return View(kontakniOsoba);
        }

        // POST: KontaktniOsoby/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            int Zakaznik = kontakniOsoba.ZakaznikId;
            try
            {
                db.KontakniOsoba.Remove(kontakniOsoba);
                db.SaveChanges();
            }
            catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            return RedirectToAction("Index", "KontaktniOsoby", new { Zakaznik });
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
