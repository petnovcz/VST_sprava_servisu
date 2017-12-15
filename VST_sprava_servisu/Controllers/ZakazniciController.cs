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
    public class ZakazniciController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Zakaznici
        public ActionResult Index(int? Region, string Search)
        {
            var zakaznik = db.Zakaznik.Include(z => z.Region).Include(z => z.Jazyk);
            if (Region != null)
            {
                zakaznik = zakaznik.Where(r => r.Region.Skupina == Region);
            }
            if (Search != null)
            {
                zakaznik = zakaznik.Where(r => r.NazevZakaznika.Contains(Search));
            }
            zakaznik = zakaznik.OrderBy(r => r.NazevZakaznika);

            return View(zakaznik.ToList());
        }



        // GET: Zakaznici/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            return View(zakaznik);
        }

        public ActionResult Header(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            return View(zakaznik);
        }



        // GET: Zakaznici/Create
        public ActionResult Create()
        {
            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu");
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku");
            return View();
        }

        // POST: Zakaznici/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazevZakaznika,KodSAP,RegionId,IC,DIC,Telefon,Kontakt,Email,JazykId,Adresa")] Zakaznik zakaznik)
        {
            if (ModelState.IsValid)
            {
                db.Zakaznik.Add(zakaznik);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // GET: Zakaznici/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // POST: Zakaznici/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazevZakaznika,KodSAP,RegionId,IC,DIC,Telefon,Kontakt,Email,JazykId,Adresa")] Zakaznik zakaznik)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zakaznik).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // GET: Zakaznici/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            return View(zakaznik);
        }

        // POST: Zakaznici/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            db.Zakaznik.Remove(zakaznik);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool CreateFromSAPdata(SAPOP sapOP)
        {

            if (ModelState.IsValid)
            {
                Zakaznik zakaznik = new Zakaznik();
                zakaznik.KodSAP = sapOP.CardCode;
                zakaznik.NazevZakaznika = sapOP.CardName;
                zakaznik.Adresa = (sapOP.Address + ", " + sapOP.City + ", " + sapOP.ZipCode + ", " + sapOP.Country);
                zakaznik.DIC = sapOP.LicTradNum;
                zakaznik.IC = sapOP.VatIdUnCmp;
                zakaznik.JazykId = sapOP.JazykId;
                zakaznik.RegionId = sapOP.RegionId;
                zakaznik.Telefon = sapOP.Phone;
                zakaznik.Email = sapOP.Email;
                zakaznik.Kontakt = "d";
                db.Zakaznik.Add(zakaznik);
                db.SaveChanges();
            }




            return true;
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
