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

        // GET: ServisniZasah/Details/5
        public ActionResult Details(int? id)
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

        // GET: ServisniZasah/Create
        public ActionResult Create(int Zakaznik, int Provoz, int Umisteni)
        {
            ServisniZasah sz = new ServisniZasah();
            sz.ZakaznikID = Zakaznik;
            sz.ProvozId = Provoz;
            sz.UmisteniId = Umisteni;
            sz.Odkud = "Semtín 79, Pardubice, Česká Republika";
            sz.Kam = db.Provoz.Where(t => t.Id == Provoz).Select(t => t.AdresaProvozu).FirstOrDefault();
            sz.Zpět = "Semtín 79, Pardubice, Česká Republika";

            var origin = sz.Odkud;
            var destination = sz.Kam;
            var destination2 = sz.Zpět;

            float km1 = 0;
            float km2 = 0;
            string url = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + destination + "&sensor=false";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sreader = new StreamReader(dataStream);
                string responsereader = sreader.ReadToEnd();
                response.Close();

                DataSet ds = new DataSet();
                ds.ReadXml(new XmlTextReader(new StringReader(responsereader)));
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables["element"].Rows[0]["status"].ToString() == "OK")
                    {
                    string x = ds.Tables["distance"].Rows[0]["text"].ToString();
                    var cx = x.Substring(0,x.Length-3).Replace(".",",");
                    
                    km1 = float.Parse(cx);

                }
                }

            string url2 = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + destination + "&destinations=" + destination2 + "&sensor=false";

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url2);
            WebResponse response2 = request2.GetResponse();
            Stream dataStream2 = response2.GetResponseStream();
            StreamReader sreader2 = new StreamReader(dataStream2);
            string responsereader2 = sreader2.ReadToEnd();
            response2.Close();

            DataSet ds2 = new DataSet();
            ds2.ReadXml(new XmlTextReader(new StringReader(responsereader)));
            if (ds2.Tables.Count > 0)
            {
                if (ds2.Tables["element"].Rows[0]["status"].ToString() == "OK")
                {

                    string x = ds2.Tables["distance"].Rows[0]["text"].ToString();
                    var cx = x.Substring(0, x.Length - 3).Replace(".", ",");
                    km2 = float.Parse(cx);
                }
            }
            var celkem = km1 + km2;
            var doubl = Math.Round(celkem);
            sz.Km = Int32.Parse(doubl.ToString());
            sz.DatumOdstraneni = DateTime.Now;
            sz.DatumVyzvy = DateTime.Now;
            sz.DatumVznikuPoruchy = DateTime.Now;
            sz.DatumZasahu = DateTime.Now;
            
            
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
        public ActionResult Create([Bind(Include = "Id,ZakaznikID,ProvozId,UmisteniId,DatumVyzvy,DatumVznikuPoruchy,DatumZasahu,DatumOdstraneni,Odkud,Kam,Zpět,Km,VozidloId,CestaCelkem,PraceHod,PraceSazba,Pracelidi,PraceCelkem,Celkem,Reklamace,PoruseniZarucnichPodminek,Mena")] ServisniZasah servisniZasah)
        {
            if (ModelState.IsValid)
            {
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
                return RedirectToAction("Details","ServisniZasah",new { Id = servisniZasah.Id});
            }

            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", servisniZasah.ProvozId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", servisniZasah.UmisteniId);
            ViewBag.VozidloId = new SelectList(db.Vozidlo, "Id", "NazevVozidla", servisniZasah.VozidloId);
            ViewBag.ZakaznikID = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", servisniZasah.ZakaznikID);
            return View(servisniZasah);
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
        public ActionResult Edit([Bind(Include = "Id,ZakaznikID,ProvozId,UmisteniId,DatumVyzvy,DatumVznikuPoruchy,DatumZasahu,DatumOdstraneni,Odkud,Kam,Zpět,Km,VozidloId,CestaCelkem,PraceHod,PraceSazba,Pracelidi,PraceCelkem,Celkem,Reklamace,PoruseniZarucnichPodminek,Mena")] ServisniZasah servisniZasah)
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
