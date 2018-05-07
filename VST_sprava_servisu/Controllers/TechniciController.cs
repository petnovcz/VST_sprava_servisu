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
    public class TechniciController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Technicis
        public ActionResult Index()
        {
            return View(db.Technici.ToList());
        }

        // GET: Technicis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Technici technici = db.Technici.Find(id);
            if (technici == null)
            {
                return HttpNotFound();
            }
            return View(technici);
        }

        // GET: Technicis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Technicis/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PrijmeniJmeno,File,ImageSize,FileName")] Technici technici)
        {
            if (ModelState.IsValid)
            {
                db.Technici.Add(technici);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(technici);
        }

        // GET: Technicis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Technici technici = db.Technici.Find(id);
            if (technici == null)
            {
                return HttpNotFound();
            }
            return View(technici);
        }

        // POST: Technicis/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PrijmeniJmeno,File,ImageSize,FileName")] Technici technici)
        {
            if (ModelState.IsValid)
            {
                db.Entry(technici).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(technici);
        }

        // GET: Technicis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Technici technici = db.Technici.Find(id);
            if (technici == null)
            {
                return HttpNotFound();
            }
            return View(technici);
        }

        // POST: Technicis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Technici technici = db.Technici.Find(id);
            db.Technici.Remove(technici);
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
