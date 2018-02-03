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
    public class VymenyLahviController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: VymenyLahvi
        public ActionResult Index(int RevizeId)
        {
            var vymenyLahvi = db.VymenyLahvi.Include(v => v.SCProvozu).Include(v => v.SCProvozu1).Include(v => v.Revize1)
                .Where(v =>v.Revize == RevizeId);
            return View(vymenyLahvi.ToList());
        }

        // GET: VymenyLahvi/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VymenyLahvi vymenyLahvi = db.VymenyLahvi.Find(id);
            if (vymenyLahvi == null)
            {
                return HttpNotFound();
            }
            return View(vymenyLahvi);
        }

        // GET: VymenyLahvi/Create
        public ActionResult Create()
        {
            ViewBag.SCProvozuNova = new SelectList(db.SCProvozu, "Id", "Lokace");
            ViewBag.SCProvozuPuvodni = new SelectList(db.SCProvozu, "Id", "Lokace");
            ViewBag.Revize = new SelectList(db.Revize, "Id", "ZjistenyStav");
            return View();
        }

        // POST: VymenyLahvi/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SCProvozuPuvodni,SCProvozuNova,SCLahve,DatumVymeny,Revize")] VymenyLahvi vymenyLahvi)
        {
            if (ModelState.IsValid)
            {
                db.VymenyLahvi.Add(vymenyLahvi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SCProvozuNova = new SelectList(db.SCProvozu, "Id", "Lokace", vymenyLahvi.SCProvozuNova);
            ViewBag.SCProvozuPuvodni = new SelectList(db.SCProvozu, "Id", "Lokace", vymenyLahvi.SCProvozuPuvodni);
            ViewBag.Revize = new SelectList(db.Revize, "Id", "ZjistenyStav", vymenyLahvi.Revize);
            return View(vymenyLahvi);
        }

        // GET: VymenyLahvi/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VymenyLahvi vymenyLahvi = db.VymenyLahvi.Find(id);
            if (vymenyLahvi == null)
            {
                return HttpNotFound();
            }
            ViewBag.SCProvozuNova = new SelectList(db.SCProvozu, "Id", "Lokace", vymenyLahvi.SCProvozuNova);
            ViewBag.SCProvozuPuvodni = new SelectList(db.SCProvozu, "Id", "Lokace", vymenyLahvi.SCProvozuPuvodni);
            ViewBag.Revize = new SelectList(db.Revize, "Id", "ZjistenyStav", vymenyLahvi.Revize);
            return View(vymenyLahvi);
        }

        // POST: VymenyLahvi/Edit/5
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
            ViewBag.SCProvozuNova = new SelectList(db.SCProvozu, "Id", "Lokace", vymenyLahvi.SCProvozuNova);
            ViewBag.SCProvozuPuvodni = new SelectList(db.SCProvozu, "Id", "Lokace", vymenyLahvi.SCProvozuPuvodni);
            ViewBag.Revize = new SelectList(db.Revize, "Id", "ZjistenyStav", vymenyLahvi.Revize);
            return View(vymenyLahvi);
        }

        // GET: VymenyLahvi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VymenyLahvi vymenyLahvi = db.VymenyLahvi.Find(id);
            if (vymenyLahvi == null)
            {
                return HttpNotFound();
            }
            return View(vymenyLahvi);
        }

        // POST: VymenyLahvi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VymenyLahvi vymenyLahvi = db.VymenyLahvi.Find(id);
            db.VymenyLahvi.Remove(vymenyLahvi);
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
