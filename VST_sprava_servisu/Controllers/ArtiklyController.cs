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
    public class ArtiklyController : Controller
    {
        private Model1Container db = new Model1Container();
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Artikly
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View(db.Artikl.ToList());
        }

        // GET: Artikly/Details/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artikl artikl = db.Artikl.Find(id);
            if (artikl == null)
            {
                return HttpNotFound();
            }
            return View(artikl);
        }

        // GET: Artikly/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            ViewBag.SkupinaArtiklu = new SelectList(db.SkupinaArtiklu, "Id", "Skupina");
            return View();
        }

        // POST: Artikly/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,Nazev,Oznaceni,Typ,RozsahProvoznichTeplot,KodSAP,Revize,PeriodaRevize,TlakovaZk,PeriodaTlakovaZk,VymenaBaterie,PeriodaBaterie,ArtiklBaterieSAP,VymenaPyro,PeriodaPyro,ArtoklPyro,SkupinaArtiklu,TlakovaNadoba,PeriodaRevizeTlakoveNadoby,PeriodaVnitrniRevize")] Artikl artikl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Artikl.Add(artikl);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                return RedirectToAction("Details","SkupinaArtiklu",new { id = artikl.SkupinaArtiklu});
            }
            ViewBag.SkupinaArtiklu = new SelectList(db.SkupinaArtiklu, "Id", "Skupina",artikl.SkupinaArtiklu);
            return View(artikl);
        }

        // GET: Artikly/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artikl artikl = db.Artikl.Find(id);
            if (artikl == null)
            {
                return HttpNotFound();
            }
            ViewBag.SkupinaArtiklu = new SelectList(db.SkupinaArtiklu, "Id", "Skupina", artikl.SkupinaArtiklu);
            return View(artikl);
        }

        // POST: Artikly/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,Nazev,Oznaceni,Typ,RozsahProvoznichTeplot,KodSAP,Revize,PeriodaRevize,TlakovaZk,PeriodaTlakovaZk,VymenaBaterie,PeriodaBaterie,ArtiklBaterieSAP,VymenaPyro,PeriodaPyro,ArtoklPyro,SkupinaArtiklu,TlakovaNadoba,PeriodaRevizeTlakoveNadoby,PeriodaVnitrniRevize")] Artikl artikl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(artikl).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                return RedirectToAction("Details", "SkupinaArtiklu", new { id = artikl.SkupinaArtiklu });
            }
            ViewBag.SkupinaArtiklu = new SelectList(db.SkupinaArtiklu, "Id", "Skupina", artikl.SkupinaArtiklu);
            return View(artikl);
        }

        // GET: Artikly/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artikl artikl = db.Artikl.Find(id);
            if (artikl == null)
            {
                return HttpNotFound();
            }
            return View(artikl);
        }

        // POST: Artikly/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            Artikl artikl = db.Artikl.Find(id);
            int idsa = artikl.SkupinaArtiklu.Value;
            try
            {
                db.Artikl.Remove(artikl);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
            }
            return RedirectToAction("Details", "SkupinaArtiklu", new { id = idsa });
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
