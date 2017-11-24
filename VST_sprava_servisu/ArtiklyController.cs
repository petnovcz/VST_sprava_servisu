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
    public class ArtiklyController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Artikly
        public ActionResult Index()
        {
            return View(db.Artikl.ToList());
        }

        // GET: Artikly/Details/5
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Artikly/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nazev,Oznaceni,Typ,RozsahProvoznichTeplot,KodSAP,Revize ,PeriodaRevize,TlakovaZk ,PeriodaTlakovaZk,VymenaBaterie , PeriodaBaterie , ArtiklBaterieSAP , VymenaPyro , PeriodaPyro, ArtoklPyro")] Artikl artikl)
        {
            if (ModelState.IsValid)
            {
                db.Artikl.Add(artikl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(artikl);
        }

        // GET: Artikly/Edit/5
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
            return View(artikl);
        }

        // POST: Artikly/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nazev,Oznaceni,Typ,RozsahProvoznichTeplot,KodSAP,Revize ,PeriodaRevize,TlakovaZk ,PeriodaTlakovaZk,VymenaBaterie , PeriodaBaterie , ArtiklBaterieSAP , VymenaPyro , PeriodaPyro, ArtoklPyro")] Artikl artikl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artikl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(artikl);
        }

        // GET: Artikly/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            Artikl artikl = db.Artikl.Find(id);
            db.Artikl.Remove(artikl);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public bool CreateFromSAPdata(SAPItem sapItem)
        {

            if (ModelState.IsValid)
            {
                Artikl artikl = new Artikl();
                artikl.KodSAP = sapItem.ItemCode;
                artikl.Nazev = sapItem.ItemName;

                artikl.Typ = sapItem.ItmsGrpNam;
                artikl.Oznaceni = sapItem.ItemName;
                artikl.RozsahProvoznichTeplot = " ";

                db.Artikl.Add(artikl);
                db.SaveChanges();
            }




            return true;
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
