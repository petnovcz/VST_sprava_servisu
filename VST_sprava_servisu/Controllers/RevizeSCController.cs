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
    public class RevizeSCController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: RevizeSC
        public ActionResult Index(int id)
        {
            var revizeSC = db.RevizeSC.Include(r => r.Revize).Include(r => r.SCProvozu).Include(r => r.Umisteni).Where(r=>r.RevizeId ==id)
                .OrderBy(r=>r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.PoradiZobrazeni)
                .ThenBy(r=>r.SCProvozu.Znaceni);
            return View(revizeSC.ToList());
        }

        // GET: RevizeSC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            if (revizeSC == null)
            {
                return HttpNotFound();
            }
            return View(revizeSC);
        }

        // GET: RevizeSC/Create
        public ActionResult Create()
        {
            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav");
            ViewBag.SCProvozuId = new SelectList(db.SCProvozu, "Id", "Lokace");
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni");
            return View();
        }

        // POST: RevizeSC/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RevizeId,SCProvozuId,StavKoroze,StavZnecisteni,JineZavady,UmisteniId,Baterie,Pyro,TlakovaZkouska")] RevizeSC revizeSC)
        {
            if (ModelState.IsValid)
            {
                db.RevizeSC.Add(revizeSC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav", revizeSC.RevizeId);
            ViewBag.SCProvozuId = new SelectList(db.SCProvozu, "Id", "Lokace", revizeSC.SCProvozuId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", revizeSC.UmisteniId);
            return View(revizeSC);
        }

        // GET: RevizeSC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            if (revizeSC == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav", revizeSC.RevizeId);
            ViewBag.SCProvozuId = new SelectList(db.SCProvozu, "Id", "Lokace", revizeSC.SCProvozuId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", revizeSC.UmisteniId);
            return View(revizeSC);
        }

        // POST: RevizeSC/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RevizeId,SCProvozuId,StavKoroze,StavZnecisteni,JineZavady,UmisteniId,Baterie,Pyro,TlakovaZkouska")] RevizeSC revizeSC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revizeSC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav", revizeSC.RevizeId);
            ViewBag.SCProvozuId = new SelectList(db.SCProvozu, "Id", "Lokace", revizeSC.SCProvozuId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", revizeSC.UmisteniId);
            return View(revizeSC);
        }

        // GET: RevizeSC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            if (revizeSC == null)
            {
                return HttpNotFound();
            }
            return View(revizeSC);
        }

        // POST: RevizeSC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            db.RevizeSC.Remove(revizeSC);
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
