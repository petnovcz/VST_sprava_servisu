using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu
{
    public class VymenyLahvisController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VymenyLahvis
        public ActionResult Index(int RevizeId)
        {
            return View(db.VymenyLahvis.Where(r=>r.Revize == RevizeId).ToList());
        }

        // GET: VymenyLahvis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VymenyLahvi vymenyLahvi = db.VymenyLahvis.Find(id);
            if (vymenyLahvi == null)
            {
                return HttpNotFound();
            }
            return View(vymenyLahvi);
        }

        // GET: VymenyLahvis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VymenyLahvis/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SCProvozuPuvodni,SCProvozuNova,SCLahve,DatumVymeny,Revize")] VymenyLahvi vymenyLahvi)
        {
            if (ModelState.IsValid)
            {
                db.VymenyLahvis.Add(vymenyLahvi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vymenyLahvi);
        }

        // GET: VymenyLahvis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VymenyLahvi vymenyLahvi = db.VymenyLahvis.Find(id);
            if (vymenyLahvi == null)
            {
                return HttpNotFound();
            }
            return View(vymenyLahvi);
        }

        // POST: VymenyLahvis/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SCProvozuPuvodni,SCProvozuNova,SCLahve,DatumVymeny,Revize")] VymenyLahvi vymenyLahvi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vymenyLahvi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vymenyLahvi);
        }

        // GET: VymenyLahvis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VymenyLahvi vymenyLahvi = db.VymenyLahvis.Find(id);
            if (vymenyLahvi == null)
            {
                return HttpNotFound();
            }
            return View(vymenyLahvi);
        }

        // POST: VymenyLahvis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VymenyLahvi vymenyLahvi = db.VymenyLahvis.Find(id);
            db.VymenyLahvis.Remove(vymenyLahvi);
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
