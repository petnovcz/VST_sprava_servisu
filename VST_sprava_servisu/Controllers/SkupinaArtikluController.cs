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
    public class SkupinaArtikluController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: SkupinaArtiklu
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View(db.SkupinaArtiklu.Include(a=>a.Artikl).ToList());
        }

        // GET: SkupinaArtiklu/Details/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkupinaArtiklu skupinaArtiklu = db.SkupinaArtiklu.Find(id);
            if (skupinaArtiklu == null)
            {
                return HttpNotFound();
            }
            return View(skupinaArtiklu);
        }

        // GET: SkupinaArtiklu/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SkupinaArtiklu/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,Skupina,Revize,PeriodaRevize,TlakovaZk,PeriodaTlakovaZk,VymenaBaterie,PeriodaBaterie,ArtiklBaterieSAP,VymenaPyro,PeriodaPyro,ArtoklPyro,PoradiZobrazeni")] SkupinaArtiklu skupinaArtiklu)
        {
            if (ModelState.IsValid)
            {
                db.SkupinaArtiklu.Add(skupinaArtiklu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(skupinaArtiklu);
        }

        // GET: SkupinaArtiklu/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkupinaArtiklu skupinaArtiklu = db.SkupinaArtiklu.Find(id);
            if (skupinaArtiklu == null)
            {
                return HttpNotFound();
            }
            return View(skupinaArtiklu);
        }

        // POST: SkupinaArtiklu/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,Skupina,Revize,PeriodaRevize,TlakovaZk,PeriodaTlakovaZk,VymenaBaterie,PeriodaBaterie,ArtiklBaterieSAP,VymenaPyro,PeriodaPyro,ArtoklPyro,PoradiZobrazeni")] SkupinaArtiklu skupinaArtiklu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(skupinaArtiklu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skupinaArtiklu);
        }

        // GET: SkupinaArtiklu/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkupinaArtiklu skupinaArtiklu = db.SkupinaArtiklu.Find(id);
            if (skupinaArtiklu == null)
            {
                return HttpNotFound();
            }
            return View(skupinaArtiklu);
        }

        // POST: SkupinaArtiklu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            SkupinaArtiklu skupinaArtiklu = db.SkupinaArtiklu.Find(id);
            db.SkupinaArtiklu.Remove(skupinaArtiklu);
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
