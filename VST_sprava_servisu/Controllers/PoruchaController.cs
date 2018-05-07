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
    public class PoruchaController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Porucha
        public ActionResult Index()
        {
            var porucha = db.Porucha.Include(p => p.KategoriePoruchy);
            return View(porucha.ToList());
        }

        // GET: Porucha/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Porucha porucha = db.Porucha.Find(id);
            if (porucha == null)
            {
                return HttpNotFound();
            }
            return View(porucha);
        }

        // GET: Porucha/Create
        public ActionResult Create()
        {
            ViewBag.KategoriePoruchyId = new SelectList(db.KategoriePoruchy, "Id", "NazevKategorie");
            var skupina = db.SkupinaArtiklu.ToList();
            SkupinaArtiklu sk = new SkupinaArtiklu();
            sk.Id = 0;
            sk.Skupina = "";
            skupina.Insert(0, sk);
            ViewBag.SkupinaArtikluId = new SelectList(skupina, "Id", "Skupina");
            return View();
        }

        // POST: Porucha/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazevPoruchy,KategoriePoruchyId,SkupinaArtikluId,SIL,Aktivace,OpravnenaAktivace,FalesnaAktivace,VZaruce,UznanaReklamace,PozarucniServis")] Porucha porucha)
        {
            if (ModelState.IsValid)
            {
                if (porucha.SkupinaArtikluId == 0) { porucha.SkupinaArtikluId = null; }
                db.Porucha.Add(porucha);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KategoriePoruchyId = new SelectList(db.KategoriePoruchy, "Id", "NazevKategorie", porucha.KategoriePoruchyId);
            var skupina = db.SkupinaArtiklu.ToList();
            SkupinaArtiklu sk = new SkupinaArtiklu();
            sk.Id = 0;
            sk.Skupina = "";
            skupina.Insert(0, sk);
            ViewBag.SkupinaArtikluId = new SelectList(skupina, "Id", "Skupina", porucha.SkupinaArtikluId);
            return View(porucha);
        }

        // GET: Porucha/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Porucha porucha = db.Porucha.Find(id);
            if (porucha == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriePoruchyId = new SelectList(db.KategoriePoruchy, "Id", "NazevKategorie", porucha.KategoriePoruchyId);
            var skupina = db.SkupinaArtiklu.ToList();
            SkupinaArtiklu sk = new SkupinaArtiklu();
            sk.Id = 0;
            sk.Skupina = "";
            skupina.Insert(0, sk);
            ViewBag.SkupinaArtikluId = new SelectList(skupina, "Id", "Skupina", porucha.SkupinaArtikluId);
            return View(porucha);
        }

        // POST: Porucha/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazevPoruchy,KategoriePoruchyId,SkupinaArtikluId,SIL,Aktivace,OpravnenaAktivace,FalesnaAktivace,VZaruce,UznanaReklamace,PozarucniServis")] Porucha porucha)
        {
            if (ModelState.IsValid)
            {
                if (porucha.SkupinaArtikluId == 0) { porucha.SkupinaArtikluId = null; }
                db.Entry(porucha).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KategoriePoruchyId = new SelectList(db.KategoriePoruchy, "Id", "NazevKategorie", porucha.KategoriePoruchyId);
            var skupina = db.SkupinaArtiklu.ToList();
            SkupinaArtiklu sk = new SkupinaArtiklu();
            sk.Id = 0;
            sk.Skupina = "";
            skupina.Insert(0, sk);
            ViewBag.SkupinaArtikluId = new SelectList(skupina, "Id", "Skupina", porucha.SkupinaArtikluId);
            return View(porucha);
        }

        // GET: Porucha/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Porucha porucha = db.Porucha.Find(id);
            if (porucha == null)
            {
                return HttpNotFound();
            }
            return View(porucha);
        }

        // POST: Porucha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Porucha porucha = db.Porucha.Find(id);
            db.Porucha.Remove(porucha);
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
