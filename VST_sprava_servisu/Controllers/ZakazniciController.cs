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
    public class ZakazniciController : Controller
    {
        private Model1Container db = new Model1Container();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ZakazniciController");


        public ActionResult Redirect(int ZakaznikId, int ProvozId, int? UmisteniId)
        {
            if (UmisteniId == null)
            {
                return RedirectToAction("Details","Provozy", new { id = ProvozId });
            }
            else
            {
                return RedirectToAction("Details","Umistenis",new { id = UmisteniId, Provoz = ProvozId, Zakaznik = ZakaznikId });
            }
            
        }
        
        
        // GET: Zakaznici
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Index(int? Region, string Search)
        {
            int? session_region = null;
            try
            {
                session_region = Convert.ToInt32(Session["List_Skupina"].ToString());
            }
            catch { }
            
            
            if (Region != null) { Session["List_Skupina"] = Region; }
            else
            {
                if ((Region == null) && (session_region == null)) { Region = 0; }
                if ((Region == null) && (session_region != null)) { Region = session_region; }
            }

            var zakaznik = db.Zakaznik.Include(z => z.Region).Include(z => z.Jazyk);
            if ((Region != null)&&(Region !=0))
            {
                zakaznik = zakaznik.Where(r => r.Region.Skupina == Region);
            }
            if (Search != null)
            {
                zakaznik = zakaznik.Where(r => r.NazevZakaznika.Contains(Search));
            }
            zakaznik = zakaznik.OrderBy(r => r.NazevZakaznika);
            ViewBag.Region = Region;
            return View(zakaznik.ToList());
        }

        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Search(int? Skupina, string Search)
        {
            int? List_Skupina = null;

            try { List_Skupina = Convert.ToInt32(Session["List_Skupina"].ToString()); } catch { }

            List<SelectListItem> newList = new List<SelectListItem>();
            //Add select list item to list of selectlistitems
            newList.Add(new SelectListItem() { Value = "0", Text = "Vše" });
            newList.Add(new SelectListItem() { Value = "1", Text = "Česká Republika" });
            newList.Add(new SelectListItem() { Value = "2", Text = "Polsko" });
            newList.Add(new SelectListItem() { Value = "3", Text = "Slovensko a Maďarsko" });
            newList.Add(new SelectListItem() { Value = "4", Text = "Ostatní" });


            ZakaznikForm zf = new ZakaznikForm();
            var x = db.Zakaznik.ToList();



            if (Skupina != null)
            {
                Session["List_Skupina"] = Skupina;
                if (Skupina != 0) { 
                x = x.Where(r => r.Region.Skupina == Skupina).ToList();
                }
                zf.Skupina = Skupina;
                ViewBag.Skupina = new SelectList(newList, "Value", "Text", Skupina);
            }
            else 
            {
                if (List_Skupina != null)
                {
                    if(List_Skupina != 0) { 
                    x = x.Where(r => r.Region.Skupina == List_Skupina).ToList();
                    }
                    zf.Skupina = List_Skupina;
                    ViewBag.Skupina = new SelectList(newList, "Value", "Text", List_Skupina);
                }
                else { ViewBag.Skupina = new SelectList(newList, "Value", "Text", null); }

            }

            if (Search != "" && Search!= null)
            {
                x = x.Where(r => r.NazevZakaznika.ToLower().Contains(Search.ToLower())).ToList();
            }
            zf.ZakaznikList = x;
            zf.Search = Search;
            


            return View(zf);
        }

        // GET: Zakaznici/Details/5
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            return View(zakaznik);
        }
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Header(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            return View(zakaznik);
        }



        // GET: Zakaznici/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu");
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku");
            return View();
        }

        // POST: Zakaznici/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,NazevZakaznika,KodSAP,RegionId,IC,DIC,Telefon,Kontakt,Email,JazykId,Adresa")] Zakaznik zakaznik)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Zakaznik.Add(zakaznik);
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Index");
            }

            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // GET: Zakaznici/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // POST: Zakaznici/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,NazevZakaznika,KodSAP,RegionId,IC,DIC,Telefon,Kontakt,Email,JazykId,Adresa")] Zakaznik zakaznik)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(zakaznik).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Details", "Zakaznici", new { Id = zakaznik.Id });
            }
            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // GET: Zakaznici/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            if (zakaznik == null)
            {
                return HttpNotFound();
            }
            return View(zakaznik);
        }

        // POST: Zakaznici/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            try
            {
                db.Zakaznik.Remove(zakaznik);
                db.SaveChanges();
            }
            catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
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
