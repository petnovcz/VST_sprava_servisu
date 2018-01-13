﻿using System;
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
    public class ProvozyController : Controller
    {
        private Model1Container db = new Model1Container();
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Provozy
        [Authorize(Roles = "Administrator,Manager")]
        public PartialViewResult Index(int Zakaznik)
        {
            var provoz = db.Provoz.Include(p => p.Zakaznik).Where(p=>p.ZakaznikId == Zakaznik);
            ViewBag.Zakaznik = Zakaznik;
            return PartialView(provoz.ToList());
        }

        // GET: Provozy/Details/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provoz provoz = db.Provoz.Find(id);
            if (provoz == null)
            {
                return HttpNotFound();
            }
            return View(provoz);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Header(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provoz provoz = db.Provoz.Find(id);
            if (provoz == null)
            {
                return HttpNotFound();
            }
            return View(provoz);
        }

        // GET: Provozy/Create
        [HttpGet]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create(int Zakaznik)
        {
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika");
            ViewBag.Zakaznik = Zakaznik;
            return View();
        }

        // POST: Provozy/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,ZakaznikId,NazevProvozu,OddeleniVybuchu,PotlaceniVybuchu,OdlehceniVybuchu,AdresaProvozu,SAPAddress")] Provoz provoz)
        {
            int Zakaznik = provoz.ZakaznikId;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Provoz.Add(provoz);
                    db.SaveChanges();
                }
                catch (SqlException e)
                {
                    log.Error("Error number: " + e.Number + " - " + e.Message);
                }
                var provozy = db.Provoz.Include(p => p.Zakaznik).Where(p => p.ZakaznikId == Zakaznik);
                ViewBag.Zakaznik = Zakaznik;
                return RedirectToAction("Details","Zakaznici",new {Id = Zakaznik });
            }
            ViewBag.Zakaznik = Zakaznik;
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", provoz.ZakaznikId);
            return View(provoz);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public bool Generate(string Address, string CardCode, string Street, string ZipCode, string City, string Country, int Zakaznik)
        {
            Provoz provoz  = new Provoz();
            provoz.ZakaznikId = Zakaznik;
            provoz.NazevProvozu = Address;
            provoz.SAPAddress = Address;
            provoz.AdresaProvozu = Street + ", " + ZipCode + ", " + City + ", " + Country;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Provoz.Add(provoz);
                    db.SaveChanges();
                }
                catch (SqlException e)
                {
                    log.Error("Error number: " + e.Number + " - " + e.Message);
                }

            }

            return true;
        }


        // GET: Provozy/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provoz provoz = db.Provoz.Find(id);
            if (provoz == null)
            {
                return HttpNotFound();
            }
            ViewBag.Zakaznik = provoz.ZakaznikId;
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", provoz.ZakaznikId);
            return View(provoz);
        }

        // POST: Provozy/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,ZakaznikId,NazevProvozu,OddeleniVybuchu,PotlaceniVybuchu,OdlehceniVybuchu,AdresaProvozu,SAPAddress")] Provoz provoz)
        {
            int zakaznik = provoz.ZakaznikId;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(provoz).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (SqlException e)
                {
                    log.Error("Error number: " + e.Number + " - " + e.Message);
                }
                return RedirectToAction("Details", "Zakaznici", new { Id = zakaznik });
            }
            ViewBag.Zakaznik = zakaznik;
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", provoz.ZakaznikId);
            return View(provoz);
        }

        // GET: Provozy/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provoz provoz = db.Provoz.Find(id);
            if (provoz == null)
            {
                return HttpNotFound();
            }
            ViewBag.Zakaznik = provoz.ZakaznikId;
            return View(provoz);
        }

        // POST: Provozy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Provoz provoz = db.Provoz.Find(id);
            int zakaznik = provoz.ZakaznikId;
            try
            {
                db.Provoz.Remove(provoz);
                db.SaveChanges();
            }
            catch (SqlException e)
            {
                log.Error("Error number: " + e.Number + " - " + e.Message);
            }
            return RedirectToAction("Details", "Zakaznici", new { Id = zakaznik });
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
