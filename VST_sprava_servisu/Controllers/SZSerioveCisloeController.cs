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
    public class SZSerioveCisloeController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: SZSerioveCisloe
        public ActionResult Index(int Id)
        {
            var servisniZasahPrvekSerioveCislo = db.ServisniZasahPrvekSerioveCislo.Include(s => s.ServisniZasahPrvek).Where(s=>s.ServisniZasahPrvekId == Id);
            return View(servisniZasahPrvekSerioveCislo.ToList());
        }

        // GET: SZSerioveCisloe/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvekSerioveCislo servisniZasahPrvekSerioveCislo = db.ServisniZasahPrvekSerioveCislo.Find(id);
            if (servisniZasahPrvekSerioveCislo == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvekSerioveCislo);
        }

        // GET: SZSerioveCisloe/Create
        public ActionResult Create()
        {
            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id");
            return View();
        }

        // POST: SZSerioveCisloe/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ServisniZasahPrvekId,SerioveCislo,SAPKod,Sklad")] ServisniZasahPrvekSerioveCislo servisniZasahPrvekSerioveCislo)
        {
            if (ModelState.IsValid)
            {
                db.ServisniZasahPrvekSerioveCislo.Add(servisniZasahPrvekSerioveCislo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id", servisniZasahPrvekSerioveCislo.ServisniZasahPrvekId);
            return View(servisniZasahPrvekSerioveCislo);
        }

        // GET: SZSerioveCisloe/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvekSerioveCislo servisniZasahPrvekSerioveCislo = db.ServisniZasahPrvekSerioveCislo.Find(id);
            if (servisniZasahPrvekSerioveCislo == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id", servisniZasahPrvekSerioveCislo.ServisniZasahPrvekId);
            return View(servisniZasahPrvekSerioveCislo);
        }

        // POST: SZSerioveCisloe/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ServisniZasahPrvekId,SerioveCislo,SAPKod,Sklad")] ServisniZasahPrvekSerioveCislo servisniZasahPrvekSerioveCislo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servisniZasahPrvekSerioveCislo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServisniZasahPrvekId = new SelectList(db.ServisniZasahPrvek, "Id", "Id", servisniZasahPrvekSerioveCislo.ServisniZasahPrvekId);
            return View(servisniZasahPrvekSerioveCislo);
        }

        // GET: SZSerioveCisloe/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvekSerioveCislo servisniZasahPrvekSerioveCislo = db.ServisniZasahPrvekSerioveCislo.Find(id);
            if (servisniZasahPrvekSerioveCislo == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvekSerioveCislo);
        }

        // POST: SZSerioveCisloe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServisniZasahPrvekSerioveCislo servisniZasahPrvekSerioveCislo = db.ServisniZasahPrvekSerioveCislo.Find(id);
            db.ServisniZasahPrvekSerioveCislo.Remove(servisniZasahPrvekSerioveCislo);
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
