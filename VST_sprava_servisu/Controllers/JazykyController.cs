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
    public class JazykyController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Jazyky
        public ActionResult Index()
        {
            return View(db.Jazyk.ToList());
        }

        // GET: Jazyky/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jazyk jazyk = db.Jazyk.Find(id);
            if (jazyk == null)
            {
                return HttpNotFound();
            }
            return View(jazyk);
        }

        // GET: Jazyky/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Jazyky/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazevJazyku")] Jazyk jazyk)
        {
            if (ModelState.IsValid)
            {
                db.Jazyk.Add(jazyk);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(jazyk);
        }

        // GET: Jazyky/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jazyk jazyk = db.Jazyk.Find(id);
            if (jazyk == null)
            {
                return HttpNotFound();
            }
            return View(jazyk);
        }

        // POST: Jazyky/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazevJazyku")] Jazyk jazyk)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jazyk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jazyk);
        }

        // GET: Jazyky/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jazyk jazyk = db.Jazyk.Find(id);
            if (jazyk == null)
            {
                return HttpNotFound();
            }
            return View(jazyk);
        }

        // POST: Jazyky/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Jazyk jazyk = db.Jazyk.Find(id);
            db.Jazyk.Remove(jazyk);
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
