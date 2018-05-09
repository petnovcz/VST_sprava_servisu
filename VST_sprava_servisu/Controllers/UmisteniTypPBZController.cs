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
    public class UmisteniTypPBZController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: UmisteniTypPBZ
        public ActionResult Index()
        {
            var umisteniTypPBZ = db.UmisteniTypPBZ.Include(u => u.TypPBZ).Include(u => u.Umisteni);
            return View(umisteniTypPBZ.ToList());
        }

        // GET: UmisteniTypPBZ/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UmisteniTypPBZ umisteniTypPBZ = db.UmisteniTypPBZ.Find(id);
            if (umisteniTypPBZ == null)
            {
                return HttpNotFound();
            }
            return View(umisteniTypPBZ);
        }

        // GET: UmisteniTypPBZ/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create(int Zakaznik , int Provoz ,int Id ,int TypPBZ )
        {
            UmisteniTypPBZ umisteniTypPBZ = new UmisteniTypPBZ();
            umisteniTypPBZ.TypPBZId = TypPBZ;
            umisteniTypPBZ.UmisteniId = Id;
            db.UmisteniTypPBZ.Add(umisteniTypPBZ);
            db.SaveChanges();
            return RedirectToAction("Details","Umistenis",new { Zakaznik = Zakaznik, Provoz = Provoz, Id = Id });
        }

        // POST: UmisteniTypPBZ/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TypPBZId,UmisteniId")] UmisteniTypPBZ umisteniTypPBZ)
        {
            if (ModelState.IsValid)
            {
                db.UmisteniTypPBZ.Add(umisteniTypPBZ);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TypPBZId = new SelectList(db.TypPBZ, "Id", "NazevPBZ", umisteniTypPBZ.TypPBZId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", umisteniTypPBZ.UmisteniId);
            return View(umisteniTypPBZ);
        }

        // GET: UmisteniTypPBZ/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UmisteniTypPBZ umisteniTypPBZ = db.UmisteniTypPBZ.Find(id);
            if (umisteniTypPBZ == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypPBZId = new SelectList(db.TypPBZ, "Id", "NazevPBZ", umisteniTypPBZ.TypPBZId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", umisteniTypPBZ.UmisteniId);
            return View(umisteniTypPBZ);
        }

        // POST: UmisteniTypPBZ/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TypPBZId,UmisteniId")] UmisteniTypPBZ umisteniTypPBZ)
        {
            if (ModelState.IsValid)
            {
                db.Entry(umisteniTypPBZ).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypPBZId = new SelectList(db.TypPBZ, "Id", "NazevPBZ", umisteniTypPBZ.TypPBZId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", umisteniTypPBZ.UmisteniId);
            return View(umisteniTypPBZ);
        }

        // GET: UmisteniTypPBZ/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int Zakaznik, int Provoz, int Id, int TypPBZ)
        {
            
            UmisteniTypPBZ umisteniTypPBZ = db.UmisteniTypPBZ.Where(t=>t.TypPBZId == TypPBZ && t.UmisteniId == Id).First();
            db.UmisteniTypPBZ.Remove(umisteniTypPBZ);
            db.SaveChanges();
            return RedirectToAction("Details", "Umistenis", new { Zakaznik = Zakaznik, Provoz = Provoz, Id = Id });
        }

        // POST: UmisteniTypPBZ/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UmisteniTypPBZ umisteniTypPBZ = db.UmisteniTypPBZ.Find(id);
            
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
