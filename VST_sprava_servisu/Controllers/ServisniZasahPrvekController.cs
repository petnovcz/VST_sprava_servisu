using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu.Controllers
{
    public class ServisniZasahPrvekController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: ServisniZasahPrvek
        public ActionResult Index()
        {
            var servisniZasahPrvek = db.ServisniZasahPrvek.Include(s => s.Artikl).Include(s => s.Porucha).Include(s => s.ServisniZasah);
            return View(servisniZasahPrvek.ToList());
        }

        // GET: ServisniZasahPrvek/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            if (servisniZasahPrvek == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvek);
        }

        public ActionResult Add(int Id)
        {
            ServisniZasahPrvek szp = new ServisniZasahPrvek();
            szp.ServisniZasahId = Id;
            ServisniZasah sz = new ServisniZasah();
            sz = db.ServisniZasah.Where(t => t.Id == Id).FirstOrDefault();
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu.Where(t=>t.ProvozId == sz.ProvozId && t.Umisteni == sz.UmisteniId), "Id", "Znaceni");          
            return View(szp);
        }


        // GET: ServisniZasahPrvek/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int ServisniZasahId, int? SCProvozuID)
        {
            ServisniZasahPrvek szp = new ServisniZasahPrvek();
            szp.ServisniZasahId = ServisniZasahId;
            szp.SCProvozuID = SCProvozuID;
            ViewBag.ArtiklID = new SelectList(db.Artikl.Where(t=>t.SkupinaArtiklu == 129), "Id", "Nazev");
            ViewBag.PoruchaID = new SelectList(db.Porucha, "Id", "NazevPoruchy");
            
            
            return View("Create",szp);
        }

        // POST: ServisniZasahPrvek/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ServisniZasahId,SCProvozuID,PoruchaID,ArtiklID,Pocet,CenaZaKus,CenaCelkem")] ServisniZasahPrvek servisniZasahPrvek)
        {
            if (ModelState.IsValid)
            {
                ServisniZasah sz = new ServisniZasah();
                sz = db.ServisniZasah.Where(t => t.Id == servisniZasahPrvek.ServisniZasahId).FirstOrDefault();
                CenaArtikluZakaznik caz = new CenaArtikluZakaznik();
                caz = CenaArtikluZakaznik.GetCena(servisniZasahPrvek.ArtiklID.Value, sz.ZakaznikID);
                decimal cena;
                if (caz.ZCCena != null) { cena = caz.ZCCena; } else { cena = caz.CenikCena; }
                servisniZasahPrvek.CenaZaKus = cena;
                db.ServisniZasahPrvek.Add(servisniZasahPrvek);
                db.SaveChanges();
                return RedirectToAction("Details","ServisniZasah", new { Id = servisniZasahPrvek.ServisniZasahId});
            }

            ViewBag.ArtiklID = new SelectList(db.Artikl, "Id", "Nazev", servisniZasahPrvek.ArtiklID);
            ViewBag.PoruchaID = new SelectList(db.Porucha, "Id", "NazevPoruchy", servisniZasahPrvek.PoruchaID);
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu, "Id", "Lokace", servisniZasahPrvek.SCProvozuID);
            ViewBag.ServisniZasahId = new SelectList(db.ServisniZasah, "Id", "Odkud", servisniZasahPrvek.ServisniZasahId);
            return View(servisniZasahPrvek);
        }

        // GET: ServisniZasahPrvek/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            if (servisniZasahPrvek == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtiklID = new SelectList(db.Artikl, "Id", "Nazev", servisniZasahPrvek.ArtiklID);
            ViewBag.PoruchaID = new SelectList(db.Porucha, "Id", "NazevPoruchy", servisniZasahPrvek.PoruchaID);
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu, "Id", "Lokace", servisniZasahPrvek.SCProvozuID);
            ViewBag.ServisniZasahId = new SelectList(db.ServisniZasah, "Id", "Odkud", servisniZasahPrvek.ServisniZasahId);
            return View(servisniZasahPrvek);
        }

        // POST: ServisniZasahPrvek/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ServisniZasahId,SCProvozuID,PoruchaID,ArtiklID,Pocet,CenaZaKus,CenaCelkem")] ServisniZasahPrvek servisniZasahPrvek)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servisniZasahPrvek).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details","ServisniZasah",new { Id = servisniZasahPrvek.ServisniZasahId});
            }
            ViewBag.ArtiklID = new SelectList(db.Artikl, "Id", "Nazev", servisniZasahPrvek.ArtiklID);
            ViewBag.PoruchaID = new SelectList(db.Porucha, "Id", "NazevPoruchy", servisniZasahPrvek.PoruchaID);
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu, "Id", "Lokace", servisniZasahPrvek.SCProvozuID);
            ViewBag.ServisniZasahId = new SelectList(db.ServisniZasah, "Id", "Odkud", servisniZasahPrvek.ServisniZasahId);
            return View(servisniZasahPrvek);
        }

        // GET: ServisniZasahPrvek/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            if (servisniZasahPrvek == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvek);
        }

        // POST: ServisniZasahPrvek/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            db.ServisniZasahPrvek.Remove(servisniZasahPrvek);
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
