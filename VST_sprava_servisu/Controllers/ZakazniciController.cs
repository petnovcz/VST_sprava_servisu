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

        // GET: Zakaznici
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
        public ActionResult Create([Bind(Include = "Id,NazevZakaznika,KodSAP,RegionId,IC,DIC,Telefon,Kontakt,Email,JazykId,Adresa")] Zakaznik zakaznik)
        {
            if (ModelState.IsValid)
            {
                db.Zakaznik.Add(zakaznik);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // GET: Zakaznici/Edit/5
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
        public ActionResult Edit([Bind(Include = "Id,NazevZakaznika,KodSAP,RegionId,IC,DIC,Telefon,Kontakt,Email,JazykId,Adresa")] Zakaznik zakaznik)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zakaznik).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegionId = new SelectList(db.Region, "Id", "NazevRegionu", zakaznik.RegionId);
            ViewBag.JazykId = new SelectList(db.Jazyk, "Id", "NazevJazyku", zakaznik.JazykId);
            return View(zakaznik);
        }

        // GET: Zakaznici/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            Zakaznik zakaznik = db.Zakaznik.Find(id);
            db.Zakaznik.Remove(zakaznik);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool CreateFromSAPdata(SAPOP sapOP)
        {

            if (ModelState.IsValid)
            {
                Zakaznik zakaznik = new Zakaznik();
                zakaznik.KodSAP = sapOP.CardCode;
                zakaznik.NazevZakaznika = sapOP.CardName;
                zakaznik.Adresa = (sapOP.Address + ", " + sapOP.City + ", " + sapOP.ZipCode + ", " + sapOP.Country);
                zakaznik.DIC = sapOP.LicTradNum;
                zakaznik.IC = sapOP.VatIdUnCmp;
                zakaznik.JazykId = sapOP.JazykId;
                zakaznik.RegionId = sapOP.RegionId;
                zakaznik.Telefon = sapOP.Phone;
                zakaznik.Email = sapOP.Email;
                zakaznik.Kontakt = "d";
                db.Zakaznik.Add(zakaznik);
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
