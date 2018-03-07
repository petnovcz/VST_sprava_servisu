using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using VST_sprava_servisu;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu.Controllers
{
    public class ServisniZasahController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: ServisniZasah
        public ActionResult Index()
        {
            var servisniZasah = db.ServisniZasah.Include(s => s.Provoz).Include(s => s.Umisteni).Include(s => s.Vozidlo).Include(s => s.Zakaznik);
            return View(servisniZasah.ToList());
        }

        public ActionResult GenerateDL(int Id)
        {
            bool retval = SAPDIAPI.GenerateDL(Id);
            return View();
        }
        public ActionResult GenerateQuotation(int Id)
        {
            string retval = SAPDIAPI.GenerateQuotation(Id);

            return View();
        }
        public ActionResult GenerateOrder(int Id)
        {
            bool retval = SAPDIAPI.GenerateDL(Id);
            return View();
        }

        public ActionResult SelectProject(int Id)
        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
            List<Projekt> list = Projekt.ProjectList(sz.Zakaznik.KodSAP, Id);
            return View(list);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SelectProject([Bind(Include = "Code,Name,ServisniZasahId")] Projekt projekt)
        {
            if (ModelState.IsValid)
            {
                ServisniZasah sz = new ServisniZasah();
                sz = ServisniZasah.GetZasah(projekt.ServisniZasahId);
                sz.Projekt = projekt.Code;
                db.Entry(sz).State = EntityState.Modified;
                db.SaveChanges();

            }

                return RedirectToAction("Details", "ServisniZasah", new { Id = projekt.ServisniZasahId });
        }


        // GET: ServisniZasah/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasah.UpdateHeader(id.Value);
            ServisniZasah servisniZasah = db.ServisniZasah.Find(id);
            if (servisniZasah == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasah);
        }

        public ActionResult Header(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ServisniZasah.UpdateHeader(id.Value);
            ServisniZasah servisniZasah = db.ServisniZasah.Find(id);
            if (servisniZasah == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasah);
        }

        // GET: ServisniZasah/Create
        public ActionResult Create(int Zakaznik, int Provoz, int Umisteni,string Odkud, string Kam, string Zpet)
        {
            ServisniZasah sz = new ServisniZasah();
            sz.ZakaznikID = Zakaznik;
            sz.ProvozId = Provoz;
            sz.UmisteniId = Umisteni;

            if (!String.IsNullOrWhiteSpace(Odkud)) { sz.Odkud = Odkud; } else {  sz.Odkud = "Semtín 79, Pardubice, Česká Republika"; }
            if (!String.IsNullOrWhiteSpace(Kam)) { sz.Kam = Kam; }
             else { sz.Kam = db.Provoz.Where(t => t.Id == Provoz).Select(t => t.AdresaProvozu).FirstOrDefault(); }
            if (!String.IsNullOrWhiteSpace(Zpet)) { sz.Zpět = Zpet; }
            else 
            { sz.Zpět = "Semtín 79, Pardubice, Česká Republika"; }



            sz.Km = ServisniZasah.GetDistance(sz.Odkud, sz.Kam, sz.Zpět);
            sz.DatumOdstraneni = DateTime.Now;
            sz.DatumVyzvy = DateTime.Now;
            sz.DatumVznikuPoruchy = DateTime.Now;
            sz.DatumZasahu = DateTime.Now;
            sz.Mena = ServisniZasah.GetCurrency(sz.ZakaznikID);
            
            ViewBag.Provoz = db.Provoz.Where(t => t.Id == Provoz).Select(t => t.NazevProvozu).FirstOrDefault();
            ViewBag.Umisteni = db.Umisteni.Where(t => t.Id == Umisteni).Select(t => t.NazevUmisteni).FirstOrDefault();
            ViewBag.VozidloId = new SelectList(db.Vozidlo, "Id", "NazevVozidla",1);
            ViewBag.Zakaznik = db.Zakaznik.Where(t => t.Id == Zakaznik).Select(t => t.NazevZakaznika).FirstOrDefault();
            return View(sz);
        }

        // POST: ServisniZasah/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ZakaznikID,ProvozId,UmisteniId,DatumVyzvy,DatumVznikuPoruchy,DatumZasahu,DatumOdstraneni,Odkud,Kam,Zpět,Km,VozidloId,CestaCelkem,PraceHod,PraceSazba,Pracelidi,PraceCelkem,Celkem,Reklamace,PoruseniZarucnichPodminek,Mena,Closed,Porjekt,Nabidka,Zakazka,DodaciList")] ServisniZasah servisniZasah, string action)
        {
            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(action))
            {

                switch (action)
                {
                    case "Refresh":
                        servisniZasah.Km = ServisniZasah.GetDistance(servisniZasah.Odkud, servisniZasah.Kam, servisniZasah.Zpět);
                        ModelState.Clear();
                        break;
                    case "Create":
                        var km = CenaArtikluZakaznik.GetCena("SP05", servisniZasah.ZakaznikID);
                        decimal kmcena;
                        if (km.ZCCena != 0) { kmcena = km.ZCCena; } else { kmcena = km.CenikCena; }
                        servisniZasah.CestaCelkem = servisniZasah.Km * kmcena;
                        var prace = CenaArtikluZakaznik.GetCena("SP01", servisniZasah.ZakaznikID);
                        decimal pracecena;
                        if (prace.ZCCena != 0) { pracecena = prace.ZCCena; } else { pracecena = prace.CenikCena; }
                        servisniZasah.PraceSazba = pracecena;
                        servisniZasah.PraceCelkem = servisniZasah.Pracelidi * servisniZasah.PraceSazba * servisniZasah.PraceHod;



                        db.ServisniZasah.Add(servisniZasah);
                        db.SaveChanges();
                        return RedirectToAction("Details", "ServisniZasah", new { Id = servisniZasah.Id });
                        
                    
                    default: 
                        break; }
                
                
            }

            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", servisniZasah.ProvozId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", servisniZasah.UmisteniId);
            ViewBag.VozidloId = new SelectList(db.Vozidlo, "Id", "NazevVozidla", servisniZasah.VozidloId);
            ViewBag.ZakaznikID = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", servisniZasah.ZakaznikID);
            //return RedirectToAction("Create", "ServisniZasah", new { Zakaznik = servisniZasah.ZakaznikID, Provoz = servisniZasah.ProvozId, Umisteni = servisniZasah.UmisteniId, Odkud = servisniZasah.Odkud, Kam = servisniZasah.Kam, Zpet = servisniZasah.Zpět }); 
            return View("Create", servisniZasah);
        }

        // GET: ServisniZasah/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasah servisniZasah = db.ServisniZasah.Find(id);
            if (servisniZasah == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", servisniZasah.ProvozId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", servisniZasah.UmisteniId);
            ViewBag.VozidloId = new SelectList(db.Vozidlo, "Id", "NazevVozidla", servisniZasah.VozidloId);
            ViewBag.ZakaznikID = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", servisniZasah.ZakaznikID);
            return View(servisniZasah);
        }

        // POST: ServisniZasah/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ZakaznikID,ProvozId,UmisteniId,DatumVyzvy,DatumVznikuPoruchy,DatumZasahu,DatumOdstraneni,Odkud,Kam,Zpět,Km,VozidloId,CestaCelkem,PraceHod,PraceSazba,Pracelidi,PraceCelkem,Celkem,Reklamace,PoruseniZarucnichPodminek,Mena,Closed,Porjekt,Nabidka,Zakazka,DodaciList")] ServisniZasah servisniZasah)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servisniZasah).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", servisniZasah.ProvozId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", servisniZasah.UmisteniId);
            ViewBag.VozidloId = new SelectList(db.Vozidlo, "Id", "NazevVozidla", servisniZasah.VozidloId);
            ViewBag.ZakaznikID = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", servisniZasah.ZakaznikID);
            return View(servisniZasah);
        }

        // GET: ServisniZasah/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasah servisniZasah = db.ServisniZasah.Find(id);
            if (servisniZasah == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasah);
        }

        // POST: ServisniZasah/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServisniZasah servisniZasah = db.ServisniZasah.Find(id);
            db.ServisniZasah.Remove(servisniZasah);
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
