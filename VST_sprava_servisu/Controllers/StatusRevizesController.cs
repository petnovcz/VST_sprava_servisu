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
    public class StatusRevizesController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: StatusRevizes
        public ActionResult Index()
        {
            return View(db.StatusRevize.ToList());
        }

        // GET: StatusRevizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusRevize statusRevize = db.StatusRevize.Find(id);
            if (statusRevize == null)
            {
                return HttpNotFound();
            }
            return View(statusRevize);
        }

        // GET: StatusRevizes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StatusRevizes/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazevStatusuRevize,Planovana,Potvrzena,Realizovana")] StatusRevize statusRevize)
        {
            if (ModelState.IsValid)
            {
                db.StatusRevize.Add(statusRevize);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statusRevize);
        }

        // GET: StatusRevizes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusRevize statusRevize = db.StatusRevize.Find(id);
            if (statusRevize == null)
            {
                return HttpNotFound();
            }
            return View(statusRevize);
        }

        // POST: StatusRevizes/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazevStatusuRevize,Planovana,Potvrzena,Realizovana")] StatusRevize statusRevize)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statusRevize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statusRevize);
        }

        // GET: StatusRevizes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusRevize statusRevize = db.StatusRevize.Find(id);
            if (statusRevize == null)
            {
                return HttpNotFound();
            }
            return View(statusRevize);
        }

        // POST: StatusRevizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StatusRevize statusRevize = db.StatusRevize.Find(id);
            db.StatusRevize.Remove(statusRevize);
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
