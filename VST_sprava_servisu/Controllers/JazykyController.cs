using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class JazykyController : Controller
    {
        private Model1Container db = new Model1Container();
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Jazyky
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View(db.Jazyk.ToList());
        }

        // GET: Jazyky/Details/5
        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Jazyky/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,NazevJazyku")] Jazyk jazyk)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Jazyk.Add(jazyk);
                    db.SaveChanges();
                }
                catch (SqlException e)
                {
                    log.Error("Error number: " + e.Number + " - " + e.Message);
                }
                return RedirectToAction("Index");
            }

            return View(jazyk);
        }

        // GET: Jazyky/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,NazevJazyku")] Jazyk jazyk)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(jazyk).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (SqlException e)
                {
                    log.Error("Error number: " + e.Number + " - " + e.Message);
                }
                return RedirectToAction("Index");
            }
            return View(jazyk);
        }

        // GET: Jazyky/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Jazyk jazyk = db.Jazyk.Find(id);
            try
            {
                db.Jazyk.Remove(jazyk);
                db.SaveChanges();
            }
            catch (SqlException e)
            {
                log.Error("Error number: " + e.Number + " - " + e.Message);
            }
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
