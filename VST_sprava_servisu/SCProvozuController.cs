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
    public class SCProvozuController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: SCProvozu
        public ActionResult Index()
        {
            var sCProvozu = db.SCProvozu.Include(s => s.Provoz).Include(s => s.SerioveCislo).Include(s => s.Status).Include(s => s.Umisteni1);
            return View(sCProvozu.ToList());
        }

        // GET: SCProvozu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            if (sCProvozu == null)
            {
                return HttpNotFound();
            }
            return View(sCProvozu);
        }

        // GET: SCProvozu/Create
        public ActionResult Create()
        {
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu");
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1");
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu");
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni");
            return View();
        }

        // POST: SCProvozu/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProvozId,SerioveCisloId,StatusId,DatumPrirazeni,DatumPosledniZmeny,DatumVymeny,Umisteni")] SCProvozu sCProvozu)
        {
            if (ModelState.IsValid)
            {
                db.SCProvozu.Add(sCProvozu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", sCProvozu.ProvozId);
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1", sCProvozu.SerioveCisloId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu", sCProvozu.StatusId);
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni", sCProvozu.Umisteni);
            return View(sCProvozu);
        }

        // GET: SCProvozu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            if (sCProvozu == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", sCProvozu.ProvozId);
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1", sCProvozu.SerioveCisloId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu", sCProvozu.StatusId);
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni", sCProvozu.Umisteni);
            return View(sCProvozu);
        }

        // POST: SCProvozu/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProvozId,SerioveCisloId,StatusId,DatumPrirazeni,DatumPosledniZmeny,DatumVymeny,Umisteni")] SCProvozu sCProvozu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sCProvozu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", sCProvozu.ProvozId);
            ViewBag.SerioveCisloId = new SelectList(db.SerioveCislo, "Id", "SerioveCislo1", sCProvozu.SerioveCisloId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "NazevStatusu", sCProvozu.StatusId);
            ViewBag.Umisteni = new SelectList(db.Umisteni, "Id", "NazevUmisteni", sCProvozu.Umisteni);
            return View(sCProvozu);
        }

        // GET: SCProvozu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            if (sCProvozu == null)
            {
                return HttpNotFound();
            }
            return View(sCProvozu);
        }

        // POST: SCProvozu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCProvozu sCProvozu = db.SCProvozu.Find(id);
            db.SCProvozu.Remove(sCProvozu);
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
