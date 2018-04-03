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
    public class UmistenisController : Controller
    {
        private Model1Container db = new Model1Container();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("UmistenisController");

        // GET: Umistenis
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Index()
        {
            var umisteni = db.Umisteni
                .Include(u => u.Provoz);
            return View(umisteni.ToList());
        }

        // GET: Umistenis/Details/5
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Details(int? id, int Provoz, int Zakaznik)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Umisteni umisteni = db.Umisteni.Find(id);
            if (umisteni == null)
            {
                return HttpNotFound();
            }
            var x = db.SCProvozu
                .Include(s => s.Status);
            x = x.Where(s => s.StatusId != 2 && s.ProvozId == umisteni.ProvozId && s.Umisteni == umisteni.Id);

            umisteni.SCProvozu = null;
            ViewBag.SCProvozu = x    .ToList();
            ViewBag.Provoz = Provoz;
            ViewBag.Zakaznik = Zakaznik;
            return View(umisteni);
        }

        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Header(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Umisteni umisteni = db.Umisteni.Find(id);
            if (umisteni == null)
            {
                return HttpNotFound();
            }
            
            return View(umisteni);
        }

        // GET: Umistenis/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create(int Provoz, int Zakaznik)
        {
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m=>m.Id == Provoz), "Id", "NazevProvozu",Provoz);
            ViewBag.Provoz = Provoz;
            ViewBag.Zakaznik = Zakaznik;
            return View();
        }

        // POST: Umistenis/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,ProvozId,NazevUmisteni,SamostatnaRevize,UkonceniZaruky")] Umisteni umisteni)
        {
            if (ModelState.IsValid)
            {
                int zakaznik = db.Provoz.Where(m => m.Id == umisteni.ProvozId).Select(m => m.ZakaznikId).FirstOrDefault();
                try
                {
                    db.Umisteni.Add(umisteni);
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Details", "Provozy", new { Id = umisteni.ProvozId });
            }

            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.Id == umisteni.ProvozId), "Id", "NazevProvozu", umisteni.ProvozId);
            return View(umisteni);
        }

        // GET: Umistenis/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id, int Provoz, int Zakaznik)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Umisteni umisteni = db.Umisteni.Find(id);
            if (umisteni == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.Id == umisteni.ProvozId), "Id", "NazevProvozu", umisteni.ProvozId);
            ViewBag.Provoz = Provoz;
            ViewBag.Zakaznik = Zakaznik;
            return View(umisteni);
        }

        // POST: Umistenis/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,ProvozId,NazevUmisteni, SamostatnaRevize,UkonceniZaruky")] Umisteni umisteni)
        {
            if (ModelState.IsValid)
            {
                int zakaznik = db.Provoz.Where(m => m.Id == umisteni.ProvozId).Select(m => m.ZakaznikId).FirstOrDefault();
                try
                {
                    db.Entry(umisteni).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Details","Provozy",new { Id = umisteni.ProvozId });
            }
            ViewBag.ProvozId = new SelectList(db.Provoz.Where(m => m.Id == umisteni.ProvozId), "Id", "NazevProvozu", umisteni.ProvozId);
            return View(umisteni);
        }

        // GET: Umistenis/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Umisteni umisteni = db.Umisteni.Find(id);
            if (umisteni == null)
            {
                return HttpNotFound();
            }
            ViewBag.Provoz = umisteni.ProvozId;
            ViewBag.Zakaznik = db.Provoz.Where(t=>t.Id == umisteni.ProvozId).Select(t=>t.ZakaznikId).FirstOrDefault();
            return View(umisteni);
        }

        // POST: Umistenis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Umisteni umisteni = db.Umisteni.Find(id);
            try
            {
                db.Umisteni.Remove(umisteni);
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
