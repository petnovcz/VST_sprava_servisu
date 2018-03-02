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
    public class KategoriePoruchyController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: KategoriePoruchy
        public ActionResult Index()
        {
            return View(db.KategoriePoruchy.ToList());
        }

        // GET: KategoriePoruchy/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KategoriePoruchy kategoriePoruchy = db.KategoriePoruchy.Find(id);
            if (kategoriePoruchy == null)
            {
                return HttpNotFound();
            }
            return View(kategoriePoruchy);
        }

        // GET: KategoriePoruchy/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KategoriePoruchy/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazevKategorie,OpravnenaReklamace,NeopravnenaReklamace,ReklamaceServisniZasah")] KategoriePoruchy kategoriePoruchy)
        {
            if (ModelState.IsValid)
            {
                db.KategoriePoruchy.Add(kategoriePoruchy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kategoriePoruchy);
        }

        // GET: KategoriePoruchy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KategoriePoruchy kategoriePoruchy = db.KategoriePoruchy.Find(id);
            if (kategoriePoruchy == null)
            {
                return HttpNotFound();
            }
            return View(kategoriePoruchy);
        }

        // POST: KategoriePoruchy/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazevKategorie,OpravnenaReklamace,NeopravnenaReklamace,ReklamaceServisniZasah")] KategoriePoruchy kategoriePoruchy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kategoriePoruchy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kategoriePoruchy);
        }

        // GET: KategoriePoruchy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KategoriePoruchy kategoriePoruchy = db.KategoriePoruchy.Find(id);
            if (kategoriePoruchy == null)
            {
                return HttpNotFound();
            }
            return View(kategoriePoruchy);
        }

        // POST: KategoriePoruchy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KategoriePoruchy kategoriePoruchy = db.KategoriePoruchy.Find(id);
            db.KategoriePoruchy.Remove(kategoriePoruchy);
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
