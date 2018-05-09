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
    public class TypPBZController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: TypPBZs
        public ActionResult Index()
        {
            return View(db.TypPBZ.ToList());
        }

        // GET: TypPBZs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypPBZ typPBZ = db.TypPBZ.Find(id);
            if (typPBZ == null)
            {
                return HttpNotFound();
            }
            return View(typPBZ);
        }

        // GET: TypPBZs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypPBZs/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazevPBZ")] TypPBZ typPBZ)
        {
            if (ModelState.IsValid)
            {
                db.TypPBZ.Add(typPBZ);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typPBZ);
        }

        // GET: TypPBZs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypPBZ typPBZ = db.TypPBZ.Find(id);
            if (typPBZ == null)
            {
                return HttpNotFound();
            }
            return View(typPBZ);
        }

        // POST: TypPBZs/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazevPBZ")] TypPBZ typPBZ)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typPBZ).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(typPBZ);
        }

        // GET: TypPBZs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypPBZ typPBZ = db.TypPBZ.Find(id);
            if (typPBZ == null)
            {
                return HttpNotFound();
            }
            return View(typPBZ);
        }

        // POST: TypPBZs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TypPBZ typPBZ = db.TypPBZ.Find(id);
            db.TypPBZ.Remove(typPBZ);
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
