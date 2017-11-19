﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class KontakniOsoby : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: KontakniOsoby
        public ActionResult Index()
        {
            var kontakniOsoba = db.KontakniOsoba.Include(k => k.Zakaznik);
            return View(kontakniOsoba.ToList());
        }

        // GET: KontakniOsoby/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            return View(kontakniOsoba);
        }

        // GET: KontakniOsoby/Create
        public ActionResult Create()
        {
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika");
            return View();
        }

        // POST: KontakniOsoby/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ZakaznikId,JmenoPrijmeni,Pozice,Telefon,Email")] KontakniOsoba kontakniOsoba)
        {
            if (ModelState.IsValid)
            {
                db.KontakniOsoba.Add(kontakniOsoba);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            return View(kontakniOsoba);
        }

        // GET: KontakniOsoby/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            return View(kontakniOsoba);
        }

        // POST: KontakniOsoby/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ZakaznikId,JmenoPrijmeni,Pozice,Telefon,Email")] KontakniOsoba kontakniOsoba)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kontakniOsoba).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ZakaznikId = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", kontakniOsoba.ZakaznikId);
            return View(kontakniOsoba);
        }

        // GET: KontakniOsoby/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            if (kontakniOsoba == null)
            {
                return HttpNotFound();
            }
            return View(kontakniOsoba);
        }

        // POST: KontakniOsoby/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KontakniOsoba kontakniOsoba = db.KontakniOsoba.Find(id);
            db.KontakniOsoba.Remove(kontakniOsoba);
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
