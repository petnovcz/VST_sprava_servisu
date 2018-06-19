using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ServisniZasahController");

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
        private readonly string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
        private readonly string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;


        // GET: ServisniZasah
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            List<ServisniZasah> servisniZasah = db.ServisniZasah.Include(s => s.Provoz).Include(s => s.Umisteni).Include(s => s.Vozidlo).Include(s => s.Zakaznik).ToList();
            return View(servisniZasah);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Poruchy(int Id)
        {
            ServisniZasah item = new ServisniZasah();
            item = db.ServisniZasah.Find(Id);
            item.Poruchy = ServisniZasah.Recalculcateporuchy(Id);
            return View(item.Poruchy);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult PoruchyChange(int Id, int Porucha, int? SCProvozuID)
        {
                       
                ServisniZasahPrvek item2 = new ServisniZasahPrvek();
                item2.PoruchaID = Porucha;
                item2.ServisniZasahId = Id;
                item2.SCProvozuID = SCProvozuID;
                db.ServisniZasahPrvek.Add(item2);
                db.SaveChanges();
                ServisniZasah sz = db.ServisniZasah.Find(Id);
                //sz.Poruchy = ServisniZasah.Recalculcateporuchy(Id);
            

            return RedirectToAction("Details", "ServisniZasah", new { Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Uploadx(int Id)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var x = Path.GetDirectoryName(file.FileName);
                }
            }
            return RedirectToAction("Details", "ServisniZasah", new { Id });
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult GenerateDL(int Id)
        {
            string retval = SAPDIAPI.GenerateDL(Id);
            ServisniZasah.UpdateDelivery(retval, Id);
            return RedirectToAction("Details", "ServisniZasah", new { Id });
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult GenerateQuotation(int Id)
        {
            string retval = "";
            try
            {
                retval = SAPDIAPI.GenerateQuotation(Id);
            }
            catch(Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
            }
            try
            {
                ServisniZasah.UpdateQuotation(retval, Id);
            }
            catch (Exception ex)
            {
                log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
            }

                return RedirectToAction("Details", "ServisniZasah", new { Id });
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult GenerateOrder(int Id)
        {
            string retval = SAPDIAPI.GenerateOrder(Id);
            ServisniZasah.UpdateOrder(retval, Id);

            return RedirectToAction("Details", "ServisniZasah", new { Id });
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult SelectProject(int Id)
        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
            List<Projekt> list = Projekt.ProjectList(sz.Zakaznik.KodSAP, Id);
            return View(list);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult SelectProject([Bind(Include = "Code,Name,ServisniZasahId,Status")] Projekt projekt)
        {
            if (ModelState.IsValid)
            {
                ServisniZasah sz = new ServisniZasah();
                sz = ServisniZasah.GetZasah(projekt.ServisniZasahId);
                sz.Projekt = projekt.Code;
                sz.ProjektStatus = projekt.Status;
                db.Entry(sz).State = EntityState.Modified;
                db.SaveChanges();

            }

                return RedirectToAction("Details", "ServisniZasah", new { Id = projekt.ServisniZasahId });
        }


        // GET: ServisniZasah/Details/5
        [Authorize(Roles = "Administrator,Manager")]
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

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Predelivery(int? id)
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

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Close(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServisniZasah.UpdateHeader(Id.Value);
            ServisniZasah servisniZasah = db.ServisniZasah.Find(Id);
            servisniZasah.Closed = true;
            db.Entry(servisniZasah).State = EntityState.Modified;
            db.SaveChanges();

            if (servisniZasah == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Details", "ServisniZasah", new { Id });
        }

        [Authorize(Roles = "Administrator,Manager,SIL,Vedení")]
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
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create(int Zakaznik, int Provoz, int Umisteni,string Odkud, string Kam, string Zpet)
        {
            ServisniZasah sz = new ServisniZasah
            {
                ZakaznikID = Zakaznik,
                ProvozId = Provoz,
                UmisteniId = Umisteni
            };
            
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
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,ZakaznikID,ProvozId,UmisteniId,DatumVyzvy,DatumVznikuPoruchy,DatumZasahu,DatumOdstraneni,Odkud,Kam,Zpět,Km,VozidloId,CestaCelkem,PraceHod,PraceSazba,Pracelidi,PraceCelkem,Celkem,Reklamace,PoruseniZarucnichPodminek,Mena,Closed,Porjekt,Nabidka,Zakazka,DodaciList")] ServisniZasah servisniZasah, string action)
        {
            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(action))
            {

                switch (action)
                {
                    case "Přepočti cestu":
                        servisniZasah.Km = ServisniZasah.GetDistance(servisniZasah.Odkud, servisniZasah.Kam, servisniZasah.Zpět);
                        ModelState.Clear();
                        break;
                    case "Vytvoř servisní zásah":
                        var km = CenaArtikluZakaznik.GetCena("SP02", servisniZasah.ZakaznikID);
                        decimal kmcena;
                        if (km.ZCCena != 0) { kmcena = km.ZCCena; } else { kmcena = km.CenikCena; }
                        servisniZasah.CestaCelkem = servisniZasah.Km * kmcena;
                        var prace = CenaArtikluZakaznik.GetCena("SP01", servisniZasah.ZakaznikID);
                        decimal pracecena;
                        if (prace.ZCCena != 0) { pracecena = prace.ZCCena; } else { pracecena = prace.CenikCena; }
                        servisniZasah.PraceSazba = pracecena;
                        servisniZasah.PraceCelkem = servisniZasah.Pracelidi * servisniZasah.PraceSazba * servisniZasah.PraceHod;



                        db.ServisniZasah.Add(servisniZasah);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        return RedirectToAction("Details", "ServisniZasah", new { servisniZasah.Id });
                        
                    
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
        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
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
        [Authorize(Roles = "Administrator,Manager")]
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


        [Authorize(Roles = "Administrator,Manager")]
        public void PrintQuotation(int Id)
        {
            ReportDocument Rel = new ReportDocument();
            string path = $"C:\\Logs\\Crystal\\Quotation.rpt";
            ServisniZasah sz = ServisniZasah.GetZasah(Id);


            //log.Error($"adresa {path}");

            try
            {

                Rel.Load(path);
                Rel.SetParameterValue("DocKey@", sz.Nabidka);
                Rel.SetParameterValue("ObjectId@", "23");
                Rel.SetDatabaseLogon("sa", "*2012Versino",
                                   "SQL", SAP_dtb, false);

                BinaryReader stream = new BinaryReader(Rel.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
                Rel.Close();
                Rel.Dispose();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
                Response.Flush();
                Response.Close();
            }
            catch(Exception ex) { log.Error($"Nenačtena adresa {path} {ex.Data} {ex.HResult} {ex.InnerException} {ex.Message}"); 
            }

        }

        [Authorize(Roles = "Administrator,Manager")]
        public void PrintOrder(int Id)
        {
            ReportDocument Rel = new ReportDocument();
            string path = $"C:\\Logs\\Crystal\\Order.rpt";
            ServisniZasah sz = ServisniZasah.GetZasah(Id);


            //log.Error($"adresa {path}");

            try
            {

                Rel.Load(path);
                Rel.SetParameterValue("DocKey@", sz.Zakazka);
                Rel.SetParameterValue("ObjectId@", "17");
                Rel.SetDatabaseLogon("sa", "*2012Versino",
                                   "SQL", SAP_dtb, false);

                BinaryReader stream = new BinaryReader(Rel.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
                Rel.Close();
                Rel.Dispose();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
                Response.Flush();
                Response.Close();
            }
            catch (Exception ex)
            { log.Error($"Nena4tena adresa {path}" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
            }

        }
        [Authorize(Roles = "Administrator,Manager")]
        public void PrintDelivery(int Id)
        {
            ReportDocument Rel = new ReportDocument();
            string path = $"C:\\Logs\\Crystal\\Delivery.rpt";
            ServisniZasah sz = ServisniZasah.GetZasah(Id);


            //log.Error($"adresa {path}");

            try
            {

                Rel.Load(path);
                Rel.SetParameterValue("DocKey@", sz.DodaciList);
                Rel.SetParameterValue("ObjectId@", "15");
                Rel.SetDatabaseLogon("sa", "*2012Versino",
                                   "SQL", SAP_dtb, false);

                BinaryReader stream = new BinaryReader(Rel.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
                Rel.Close();
                Rel.Dispose();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
                Response.Flush();
                Response.Close();
            }
            catch (Exception ex)
            {
                log.Error($"Nena4tena adresa {path} " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
            }

        }
    }
}
