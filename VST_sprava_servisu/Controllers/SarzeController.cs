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
    public class SarzeController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Sarze
        public ActionResult Index()
        {
            var servisniZasahPrvekSarze = db.ServisniZasahPrvekSarze.Include(s => s.ServisniZasahPrvek);
            return View(servisniZasahPrvekSarze.ToList());
        }

        // GET: Sarze/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvekSarze servisniZasahPrvekSarze = db.ServisniZasahPrvekSarze.Find(id);
            if (servisniZasahPrvekSarze == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvekSarze);
        }

        // GET: Sarze/Create
        public ActionResult Create()
        {
            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id");
            return View();
        }

        // POST: Sarze/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ServisniZasahPrvekId,Sarze,SAPKod,Sklad,Mnozstvi")] ServisniZasahPrvekSarze servisniZasahPrvekSarze)
        {
            if (ModelState.IsValid)
            {
                db.ServisniZasahPrvekSarze.Add(servisniZasahPrvekSarze);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id", servisniZasahPrvekSarze.ServisniZasahPrvekId);
            return View(servisniZasahPrvekSarze);
        }

        // GET: Sarze/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvekSarze servisniZasahPrvekSarze = db.ServisniZasahPrvekSarze.Find(id);
            if (servisniZasahPrvekSarze == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id", servisniZasahPrvekSarze.ServisniZasahPrvekId);
            return View(servisniZasahPrvekSarze);
        }

        // POST: Sarze/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ServisniZasahPrvekId,Sarze,SAPKod,Sklad,Mnozstvi")] ServisniZasahPrvekSarze servisniZasahPrvekSarze)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servisniZasahPrvekSarze).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id", servisniZasahPrvekSarze.ServisniZasahPrvekId);
            return View(servisniZasahPrvekSarze);
        }

        // GET: Sarze/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvekSarze servisniZasahPrvekSarze = db.ServisniZasahPrvekSarze.Find(id);
            if (servisniZasahPrvekSarze == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvekSarze);
        }

        // POST: Sarze/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServisniZasahPrvekSarze servisniZasahPrvekSarze = db.ServisniZasahPrvekSarze.Find(id);
            db.ServisniZasahPrvekSarze.Remove(servisniZasahPrvekSarze);
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
