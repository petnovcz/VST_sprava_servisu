using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu.Controllers
{
    public class ServisniZasahPrvekController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: ServisniZasahPrvek
        public ActionResult Index(int Id)
        {
            var servisniZasahPrvek = ServisniZasahPrvek.GetPrvkyById(Id);
            return View(servisniZasahPrvek);
        }
        public ActionResult Index2(int Id)
        {
            var servisniZasahPrvek = ServisniZasahPrvek.GetPrvkyById(Id);
            return View(servisniZasahPrvek);
        }

        public ActionResult Reklamace(int Id)
        {
            var servisniZasahPrvek = ServisniZasahPrvek.GetPrvekById(Id);
            if (servisniZasahPrvek.Reklamace == true) { servisniZasahPrvek.Reklamace = false; } else { servisniZasahPrvek.Reklamace = true; }
            db.Entry(servisniZasahPrvek).State = EntityState.Modified;
            db.SaveChanges();
            ServisniZasah.UpdateHeader(servisniZasahPrvek.ServisniZasahId);
            return RedirectToAction("Details", "ServisniZasah", new { Id = servisniZasahPrvek.ServisniZasahId });
        }

        public ActionResult Poruseni(int Id)
        {
            var servisniZasahPrvek = ServisniZasahPrvek.GetPrvekById(Id);
            if (servisniZasahPrvek.PoruseniZarucnichPodminek == true) { servisniZasahPrvek.PoruseniZarucnichPodminek = false; } else { servisniZasahPrvek.PoruseniZarucnichPodminek = true; }
            db.Entry(servisniZasahPrvek).State = EntityState.Modified;
            db.SaveChanges();
            ServisniZasah.UpdateHeader(servisniZasahPrvek.ServisniZasahId);
            return RedirectToAction("Details", "ServisniZasah", new { Id = servisniZasahPrvek.ServisniZasahId });
        }

        // GET: ServisniZasahPrvek/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            if (servisniZasahPrvek == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvek);
        }

        public ActionResult Add(int Id)
        {
            ServisniZasahPrvek szp = new ServisniZasahPrvek();
            szp.ServisniZasahId = Id;
            ServisniZasah sz = new ServisniZasah();
            sz = db.ServisniZasah.Where(t => t.Id == Id).FirstOrDefault();
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu.Where(t=>t.ProvozId == sz.ProvozId && t.Umisteni == sz.UmisteniId), "Id", "Znaceni");          
            return View(szp);
        }


        // GET: ServisniZasahPrvek/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int ServisniZasahId, int? SCProvozuID)
        {
            ServisniZasahPrvek szp = new ServisniZasahPrvek();
            szp.ServisniZasahId = ServisniZasahId;
            szp.SCProvozuID = SCProvozuID;
            SCProvozu scprovozu = new SCProvozu();
            try
            {
                 scprovozu = SCProvozu.GetSCProvozuById(szp.SCProvozuID.Value);
            }
            catch (Exception ex) { }
            int? skupina = 0;
            try
            {
                skupina = scprovozu.Artikl.SkupinaArtiklu;
            }
            catch (Exception ex) { }

            ViewBag.ArtiklID = new SelectList(db.Artikl.Where(t=>t.SkupinaArtiklu == 129 && t.KodSAP !="SP02" && t.KodSAP != "SP01"), "Id", "Nazev");
            if (skupina != null && skupina !=0)
            {
                ViewBag.PoruchaID = new SelectList(Porucha.GetPoruchyProSkupinu(scprovozu.Artikl.SkupinaArtiklu.Value), "Id", "NazevPoruchy");
            }
            else
            {
                ViewBag.PoruchaID = new SelectList(Porucha.GetPoruchyProSkupinu(0), "Id", "NazevPoruchy");
            }
            
            return View("Create",szp);
        }

        // POST: ServisniZasahPrvek/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ServisniZasahId,SCProvozuID,PoruchaID,ArtiklID,Pocet,CenaZaKus,CenaCelkem")] ServisniZasahPrvek servisniZasahPrvek)
        {
            if (ModelState.IsValid)
            {
                
                decimal cena;
                if (servisniZasahPrvek.ArtiklID != null)
                {
                    cena = ServisniZasah.GetCenaForprvek(servisniZasahPrvek);
                    servisniZasahPrvek.CenaZaKus = cena;
                    servisniZasahPrvek.CenaCelkem = cena * servisniZasahPrvek.Pocet;
                }
                ServisniZasah sz = new ServisniZasah();
                sz = ServisniZasah.GetZasah(servisniZasahPrvek.ServisniZasahId);
                
                //datum ukonceni platnosti zaruky umisteni
                var umisteni = Umisteni.GetDatumZaruky(sz.UmisteniId);
                //datum ukonceni platnosti zaruky SCProvozu
                DateTime? scprovozu = null;
                try
                {
                    scprovozu = SCProvozu.GetDatumZaruky(servisniZasahPrvek.SCProvozuID.Value);
                }
                catch(Exception ex)
                {
                    scprovozu = null;
                }
                
                DateTime? datum = DateTime.Now;
                //v zaruce?
                if (scprovozu != null)
                {
                    datum = scprovozu;
                }
                else
                {
                    if (umisteni != null)
                    {
                        if (scprovozu != null) { 
                        if (scprovozu >= umisteni) { datum = scprovozu; }
                        if (scprovozu < umisteni) { datum = umisteni; }
                        }
                        else { datum = umisteni; }
                    }
                }
                if (datum >= sz.DatumVyzvy)
                {
                    //var porucha = Porucha.ReklamaceById(servisniZasahPrvek.PoruchaID);
                    var porucha = false;
                    if (porucha == true)
                    {
                        servisniZasahPrvek.Reklamace = true;
                    }
                }

                db.ServisniZasahPrvek.Add(servisniZasahPrvek);
                
                




                db.SaveChanges();
                ServisniZasah.UpdateHeader(servisniZasahPrvek.ServisniZasahId);





                return RedirectToAction("Details","ServisniZasah", new { Id = servisniZasahPrvek.ServisniZasahId});
            }

            ViewBag.ArtiklID = new SelectList(db.Artikl, "Id", "Nazev", servisniZasahPrvek.ArtiklID);
            ViewBag.PoruchaID = new SelectList(db.Porucha, "Id", "NazevPoruchy", servisniZasahPrvek.PoruchaID);
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu, "Id", "Lokace", servisniZasahPrvek.SCProvozuID);
            ViewBag.ServisniZasahId = new SelectList(db.ServisniZasah, "Id", "Odkud", servisniZasahPrvek.ServisniZasahId);
            return View(servisniZasahPrvek);
        }

        // GET: ServisniZasahPrvek/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            if (servisniZasahPrvek == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtiklID = new SelectList(db.Artikl, "Id", "Nazev", servisniZasahPrvek.ArtiklID);
            ViewBag.PoruchaID = new SelectList(db.Porucha, "Id", "NazevPoruchy", servisniZasahPrvek.PoruchaID);
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu, "Id", "Lokace", servisniZasahPrvek.SCProvozuID);
            ViewBag.ServisniZasahId = new SelectList(db.ServisniZasah, "Id", "Odkud", servisniZasahPrvek.ServisniZasahId);
            return View(servisniZasahPrvek);
        }

        // POST: ServisniZasahPrvek/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ServisniZasahId,SCProvozuID,PoruchaID,ArtiklID,Pocet,CenaZaKus,CenaCelkem")] ServisniZasahPrvek servisniZasahPrvek)
        {
            if (ModelState.IsValid)
            {
                decimal cena;
                cena = ServisniZasah.GetCenaForprvek(servisniZasahPrvek);
                servisniZasahPrvek.CenaZaKus = cena;
                servisniZasahPrvek.CenaCelkem = cena * servisniZasahPrvek.Pocet;

                ServisniZasah sz = new ServisniZasah();
                sz = ServisniZasah.GetZasah(servisniZasahPrvek.ServisniZasahId);

                //datum ukonceni platnosti zaruky umisteni
                var umisteni = Umisteni.GetDatumZaruky(sz.UmisteniId);
                //datum ukonceni platnosti zaruky SCProvozu
                DateTime? scprovozu = DateTime.MinValue;
                if (servisniZasahPrvek.SCProvozuID != null)
                {
                    scprovozu = SCProvozu.GetDatumZaruky(servisniZasahPrvek.SCProvozuID.Value);
                }
                DateTime? datum = DateTime.Now;
                //v zaruce?
                if (scprovozu != null)
                {
                    datum = scprovozu;
                }
                else
                {
                    if (umisteni != null)
                    {
                        if (scprovozu != null)
                        {
                            if (scprovozu >= umisteni) { datum = scprovozu; }
                            if (scprovozu < umisteni) { datum = umisteni; }
                        }
                        else { datum = umisteni; }
                    }
                }
                if (datum >= sz.DatumVyzvy)
                {
                    var porucha = Porucha.ReklamaceById(servisniZasahPrvek.PoruchaID);
                    if (porucha == true)
                    {
                        servisniZasahPrvek.Reklamace = true;
                    }
                }
                db.Entry(servisniZasahPrvek).State = EntityState.Modified;
                db.SaveChanges();
                ServisniZasah.UpdateHeader(servisniZasahPrvek.ServisniZasahId);
                return RedirectToAction("Details","ServisniZasah",new { Id = servisniZasahPrvek.ServisniZasahId});
            }
            ViewBag.ArtiklID = new SelectList(db.Artikl, "Id", "Nazev", servisniZasahPrvek.ArtiklID);
            ViewBag.PoruchaID = new SelectList(db.Porucha, "Id", "NazevPoruchy", servisniZasahPrvek.PoruchaID);
            ViewBag.SCProvozuID = new SelectList(db.SCProvozu, "Id", "Lokace", servisniZasahPrvek.SCProvozuID);
            ViewBag.ServisniZasahId = new SelectList(db.ServisniZasah, "Id", "Odkud", servisniZasahPrvek.ServisniZasahId);
            return View(servisniZasahPrvek);
        }

        // GET: ServisniZasahPrvek/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            if (servisniZasahPrvek == null)
            {
                return HttpNotFound();
            }
            return View(servisniZasahPrvek);
        }

        // POST: ServisniZasahPrvek/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServisniZasahPrvek servisniZasahPrvek = db.ServisniZasahPrvek.Find(id);
            int servisnizasahid = servisniZasahPrvek.ServisniZasahId;
            db.ServisniZasahPrvek.Remove(servisniZasahPrvek);
            db.SaveChanges();
            return RedirectToAction("Details", "ServisniZasah", new { Id = servisnizasahid });
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
