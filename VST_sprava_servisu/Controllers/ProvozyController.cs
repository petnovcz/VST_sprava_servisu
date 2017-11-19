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
    public class ProvozyController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Provozy
        public PartialViewResult Index(int Zakaznik)
        {
            var provoz = db.Provoz.Include(p => p.Zakaznik).Where(p=>p.ZakaznikId == Zakaznik);
            ViewBag.Zakaznik = Zakaznik;
            return PartialView(provoz.ToList());
        }

        // GET: Provozy/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provoz provoz = db.Provoz.Find(id);
            if (provoz == null)
            {
                return HttpNotFound();
            }
            return View(provoz);
        }

        // GET: Provozy/Create
        [HttpGet]
        public ActionResult Create(int Zakaznik)
        {
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika");
            ViewBag.Zakaznik = Zakaznik;
            return View();
        }

        // POST: Provozy/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ZakaznikId,NazevProvozu,OddeleniVybuchu,PotlaceniVybuchu,OdlehceniVybuchu,AdresaProvozu")] Provoz provoz)
        {
            int Zakaznik = provoz.ZakaznikId;
            if (ModelState.IsValid)
            {
                //int Zakaznik = provoz.ZakaznikId;
                db.Provoz.Add(provoz);
                db.SaveChanges();
                //return RedirectToAction("Index");
                var provozy = db.Provoz.Include(p => p.Zakaznik).Where(p => p.ZakaznikId == Zakaznik);
                ViewBag.Zakaznik = Zakaznik;
                return RedirectToAction("Details","Zakaznici",new {Id = Zakaznik });
            }
            ViewBag.Zakaznik = Zakaznik;
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", provoz.ZakaznikId);
            return View(provoz);
        }





        // GET: Provozy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provoz provoz = db.Provoz.Find(id);
            if (provoz == null)
            {
                return HttpNotFound();
            }
            ViewBag.Zakaznik = provoz.ZakaznikId;
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", provoz.ZakaznikId);
            return View(provoz);
        }

        // POST: Provozy/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ZakaznikId,NazevProvozu,OddeleniVybuchu,PotlaceniVybuchu,OdlehceniVybuchu,AdresaProvozu")] Provoz provoz)
        {
            int zakaznik = provoz.ZakaznikId;
            if (ModelState.IsValid)
            {
                
                db.Entry(provoz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Zakaznici", new { Id = zakaznik });
            }
            ViewBag.Zakaznik = zakaznik;
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", provoz.ZakaznikId);
            return View(provoz);
        }

        // GET: Provozy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provoz provoz = db.Provoz.Find(id);
            if (provoz == null)
            {
                return HttpNotFound();
            }
            ViewBag.Zakaznik = provoz.ZakaznikId;
            return View(provoz);
        }

        // POST: Provozy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Provoz provoz = db.Provoz.Find(id);
            int zakaznik = provoz.ZakaznikId;
            db.Provoz.Remove(provoz);
            db.SaveChanges();
            return RedirectToAction("Details", "Zakaznici", new { Id = zakaznik });
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
