using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions;
using CrystalDecisions.CrystalReports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace VST_sprava_servisu
{
    public class RevizeController : Controller
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Model1Container db = new Model1Container();

        private string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;

        // GET: Revize
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Index(int Zakaznik)
        {
            var revize = db.Revize.Include(r => r.Provoz).Include(r => r.StatusRevize)
                .Where(r=>r.Provoz.ZakaznikId == Zakaznik)
                .OrderByDescending(r=>r.Rok).ThenByDescending(r=>r.Pololeti) ;
            ViewBag.Zakaznik = Zakaznik;
            return View(revize.ToList());
        }



        // GET: Revize/Details/5
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Details(int? id, int? Region)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            try
            {
                ViewBag.ListRegion = Session["List_Skupina"].ToString();
            }
            catch (Exception ex) { log.Error("Details - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                ViewBag.ListDateFrom = Session["List_DateFrom"].ToString();
            }
            catch (Exception ex) { log.Error("Details - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                ViewBag.ListDateTo = Session["List_DateTo"].ToString();
            }
            catch (Exception ex) { log.Error("Details - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                ViewBag.ListStatusRevize = Session["List_StatusRevize"].ToString();
            }
            catch (Exception ex) { log.Error("Details - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            ViewBag.Region = Region;
            return View(revize);
        }

        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Header(int? id, int? Region)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }

            try
            {
                ViewBag.ListRegion = Session["List_Skupina"].ToString();
            }
            catch (Exception ex) { log.Error("Header - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            try
            {
                var ListDateFrom = Session["List_DateFrom"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateFrom);
                ViewBag.ListDateFrom = xx;
            }
            catch (Exception ex) { log.Error("Header - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                var ListDateTo = Session["List_DateTo"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateTo);
                ViewBag.ListDateTo = xx;
            }
            catch (Exception ex) { log.Error("Header - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                ViewBag.ListStatus = Session["List_Status"].ToString();
            }
            catch (Exception ex) { log.Error("Header - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            if (Region == null)
            {

                ViewBag.Region = 0;
                revize.Region  = 0;
            }
            else
            {
                revize.Region = Region.Value;
                ViewBag.Region = Region;
            }
            
            
            return View(revize);
        }

        // GET: Revize/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu");
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s=>s.Realizovana != true), "Id", "NazevStatusuRevize");
            return View();
        }

        // POST: Revize/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([
        Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V, Projekt, Nabidka")] Revize revize)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Revize.Add(revize);
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Create - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Index");
            }

            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // GET: Revize/Edit/5
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Edit(int? id, int? Region)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true).OrderBy(s => s.NazevStatusuRevize), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            ViewBag.Region = Region;
            return View(revize);
        }

        // POST: Revize/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Edit([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V, Projekt, Nabidka")] Revize revize, int Region)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(revize).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Edit - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Details","Revize",new { revize.Id, Region});
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true).OrderBy(s => s.NazevStatusuRevize), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // GET: Revize/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            return View(revize);
        }

        // POST: Revize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Revize revize = db.Revize.Find(id);
            RevizeSC.DeleteRevizeSCFromRevize(revize.Id);
            try
            {
                db.Revize.Remove(revize);
                db.SaveChanges();
            }
            catch (Exception ex) { log.Error("DeleteConfirmed - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            return RedirectToAction("Nahled", "Revize", null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult MonthViewHeader(int Rok, int Mesic, int? Region)
        {
            ViewBag.ThisYear = DateTime.Now.Year;
            ViewBag.NextYear = DateTime.Now.Year + 1;
            ViewBag.LastYear = DateTime.Now.Year - 1;
            ViewBag.Rok = Rok;
            ViewBag.Mesic = Mesic;
            ViewBag.Region = Region;
            return View();
        }
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Nahled (int? Rok, int? Mesic, int? Region)
        {
            int? session_rok = null;
            int? session_mesic = null;
            int? session_region = null;
            try
            {
                session_rok = Convert.ToInt32(Session["Rok"].ToString());
            }
            catch (Exception ex) { log.Error("Nahled - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                session_mesic = Convert.ToInt32(Session["Mesic"].ToString());
            }
            catch (Exception ex) { log.Error("Nahled - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                session_region = Convert.ToInt32(Session["List_Skupina"].ToString());
            }
            catch (Exception ex) { log.Error("Nahled - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            if (Rok != null) { Session["Rok"] = Rok; }
            if (Mesic != null) { Session["Mesic"] = Mesic; }
            if (Region != null) { Session["List_Skupina"] = Region; }


            if ((Rok == null) && (session_rok==null)) { Rok = System.DateTime.Now.Year; }
            if ((Rok == null) && (session_rok != null)) { Rok = session_rok; }
            if ((Mesic == null) && (session_mesic == null)) { Mesic = System.DateTime.Now.Month; }
            if ((Mesic == null) && (session_mesic != null)) { Mesic = session_mesic; }

            DateTime date1 = new DateTime(Rok.Value, Mesic.Value, 1);
            var x = (int)date1.DayOfWeek -1;
            if (x < 0) { x = x + 7; }
            var startOfMonth = new DateTime(date1.Year, date1.Month, 1);
            var DaysInMonth = DateTime.DaysInMonth(date1.Year, date1.Month);

            ViewBag.Rok = Rok;
            ViewBag.Mesic = Mesic;
            ViewBag.X = x;
            ViewBag.DaysInMonth = DaysInMonth;
            if ((Region == null) && (session_region == null)) { Region = 0; }
            if ((Region == null) && (session_region != null)) { Region = session_region; }
            ViewBag.Region = Region;
            return View();
        }

        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult ListLines(int? Rok, int? Mesic, int? Region)
        {
            string ListRegion = null;
            DateTime? ListDateFrom = null;
            DateTime? ListDateTo = null;
            string ListStatus = null;

            try
            {
                ListRegion = Session["List_Skupina"].ToString();
            }
            catch (Exception ex) { log.Error("ListLines - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            try
            {
                var xListDateFrom = Session["ListLines - List_DateFrom"].ToString();
                DateTime xx = Convert.ToDateTime(xListDateFrom);
                ListDateFrom = xx;
            }
            catch (Exception ex) { log.Error("ListLines - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                var ListDateTox = Session["List_DateTo"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateTox);
                ListDateTo = xx;
            }
            catch (Exception ex) { log.Error("ListLines - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            try
            {
                ListStatus = Session["List_Status"].ToString();
            }
            catch (Exception ex) { log.Error("ListLines - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            ViewBag.ListDateFrom = ListDateFrom;
            ViewBag.ListDateTo = ListDateTo;
            ViewBag.ListStatus = db.StatusRevize.Where(r => r.Id == Convert.ToInt32(ListStatus)).Select(r => r.NazevStatusuRevize).FirstOrDefault();
            if (ListRegion == "0") { ViewBag.Region = "Vše"; }
            if (ListRegion == "1") { ViewBag.Region = "Česká Republika"; }
            if (ListRegion == "2") { ViewBag.Region = "Polsko"; }
            if (ListRegion == "3") { ViewBag.Region = "Slovensko a Maďarsko"; }
            if (ListRegion == "4") { ViewBag.Region = "Ostatní"; }

            if (Region == null) { Region = 0; }

            List<Revize> list = new List<Revize>();
            list = Revize.GetByRegion(Region.Value);
            return View(list);
        }


        [HttpGet]
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult List(int? Skupina, DateTime? DateFrom, DateTime? DateTo, int? Zakaznik, int? Status)
        {
            string ListRegion = null;
            DateTime? ListDateFrom = null;
            DateTime? ListDateTo = null;
            string ListStatus = null;

            try
            {
                ListRegion = Session["List_Skupina"].ToString();
            }
            catch (Exception ex) { log.Error("List - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }


            try
            {
                var xListDateFrom = Session["List_DateFrom"].ToString();
                DateTime xx = Convert.ToDateTime(xListDateFrom);
                ListDateFrom = xx;
            }
            catch (Exception ex) { log.Error("List - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            try
            {
                var ListDateTox = Session["List_DateTo"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateTox);
                ListDateTo = xx;
            }
            catch (Exception ex) { log.Error("List - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            try
            {
                ListStatus = Session["List_Status"].ToString();
            }
            catch (Exception ex) { log.Error("List - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }







            RevizeListInput rl = new RevizeListInput();
            //Create a list of select list items - this will be returned as your select list
            List<SelectListItem> newList = new List<SelectListItem>();
            //Add select list item to list of selectlistitems
            newList.Add(new SelectListItem() { Value = "0", Text = "Vše" });
            newList.Add(new SelectListItem() { Value = "1", Text = "Česká Republika" });
            newList.Add(new SelectListItem() { Value = "2", Text = "Polsko" });
            newList.Add(new SelectListItem() { Value = "3", Text = "Slovensko a Maďarsko" });
            newList.Add(new SelectListItem() { Value = "4", Text = "Ostatní" });

            //Return the list of selectlistitems as a selectlist
            if (ListRegion != null)
            {
                ViewBag.Skupina = new SelectList(newList, "Value", "Text", ListRegion);
            }
            else
            {
                ViewBag.Skupina = new SelectList(newList, "Value", "Text", null);
            }
            if (ListStatus != null)
            {
                ViewBag.Status = new SelectList(db.StatusRevize.ToList(), "Id", "NazevStatusuRevize", ListStatus);
            }
            else
            {
                ViewBag.Status = new SelectList(db.StatusRevize.ToList(), "Id", "NazevStatusuRevize", null);
            }

            var x = db.Revize
                .Include(r => r.Provoz)
                .Include(r => r.RevizeSC)
                .Include(r => r.StatusRevize)
                .Include(r => r.Umisteni);
            //.Include(r => r.);
            if (DateFrom != null) { x = x.Where(r => r.DatumRevize >= DateFrom); }
            else {
                    if (ListDateFrom != null)
                    {
                        x = x.Where(r => r.DatumRevize >= ListDateFrom);
                    }

                }
            if (DateTo != null) { x = x.Where(r => r.DatumRevize <= DateTo); }
            else
            {
                if (ListDateTo != null)
                {
                    x = x.Where(r => r.DatumRevize >= ListDateTo);
                }

            }
            if (Zakaznik != null) { x = x.Where(r => r.Provoz.ZakaznikId == Zakaznik); }
            if (Status != null) { x = x.Where(r => r.StatusRevizeId == Status); }
            if (Skupina != null && Skupina != 0) { x = x.Where(r => r.Provoz.Zakaznik.Region.Skupina == Skupina); }

            var listrevizi = x.ToList();
            listrevizi = Revize.LoopRevizeAndUpdateBatery(listrevizi);

            rl.Revize = listrevizi;

            if (DateFrom != null)
            {
                ViewBag.ListDateFrom = DateFrom;
                rl.DateFrom = DateFrom;
            }
            else {
                if (ListDateFrom != null)
                {
                    ViewBag.ListDateFrom = ListDateFrom;
                    rl.DateFrom = ListDateFrom;
                }

            }
            if (DateTo != null)
            {
                ViewBag.ListDateTo = DateTo;
                rl.DateTo = DateTo;
            }
            else
            {
                if (ListDateTo != null)
                {
                    ViewBag.ListDateTo = ListDateTo;
                    rl.DateTo = ListDateTo;
                }

            }

            ViewBag.ListStatus = db.StatusRevize.Where(r => r.Id == Status).Select(r=>r.NazevStatusuRevize).FirstOrDefault();
            if (ListRegion == "0") { ViewBag.Region = "Vše"; }
            if (ListRegion == "1") { ViewBag.Region = "Česká Republika"; }
            if (ListRegion == "2") { ViewBag.Region = "Polsko"; }
            if (ListRegion == "3") { ViewBag.Region = "Slovensko a Maďarsko"; }
            if (ListRegion == "4") { ViewBag.Region = "Ostatní"; }

            return View(rl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult List(int? Skupina, DateTime? DateFrom, DateTime? DateTo, int? Zakaznik, int? Status, bool send)
        {
            Session["List_Skupina"] = Skupina;
            Session["List_DateFrom"] = DateFrom.ToString();
            Session["List_DateTo"] = DateTo.ToString();            
            Session["List_Status"] = Status;
          
            //string abc = Session["Test"].ToString();

            RevizeListInput rl = new RevizeListInput();
            //Create a list of select list items - this will be returned as your select list
            List<SelectListItem> newList = new List<SelectListItem>();
            //Add select list item to list of selectlistitems
            newList.Add(new SelectListItem() { Value = "0", Text = "Vše" });
            newList.Add(new SelectListItem() { Value = "1", Text = "Česká Republika" });
            newList.Add(new SelectListItem() { Value = "2", Text = "Polsko" });
            newList.Add(new SelectListItem() { Value = "3", Text = "Slovensko a Maďarsko" });
            newList.Add(new SelectListItem() { Value = "4", Text = "Ostatní" });
            rl.DateFrom = DateFrom;
            rl.DateTo = DateTo;
            //Return the list of selectlistitems as a selectlist
            ViewBag.Skupina = new SelectList(newList, "Value", "Text", Skupina);
            ViewBag.Zakaznik = new SelectList(db.Zakaznik.ToList(), "Id", "NazevZakaznika", Zakaznik);
            ViewBag.Status = new SelectList(db.StatusRevize.ToList(), "Id", "NazevStatusuRevize", Status);
            var x = db.Revize
                .Include(r => r.Provoz)
                .Include(r => r.RevizeSC)
                .Include(r => r.StatusRevize)
                .Include(r => r.Umisteni);
                
            if (DateFrom != null) { x = x.Where(r => r.DatumRevize >= DateFrom);   }
            if (DateTo != null) { x = x.Where(r => r.DatumRevize <= DateTo); }
            if (Zakaznik != null) { x = x.Where(r => r.Provoz.ZakaznikId == Zakaznik); }
            if (Status != null) { x = x.Where(r => r.StatusRevizeId == Status); }
            if (Skupina != null && Skupina != 0) { x = x.Where(r => r.Provoz.Zakaznik.Region.Skupina == Skupina); }

            //
            var listrevizi = x.ToList();
            listrevizi = Revize.LoopRevizeAndUpdateBatery(listrevizi);

            rl.Revize = listrevizi;
            ViewBag.ListDateFrom = DateFrom;
            ViewBag.ListDateTo = DateTo;
            ViewBag.ListStatus = db.StatusRevize.Where(r => r.Id == Status).Select(r => r.NazevStatusuRevize).FirstOrDefault();
            if (Skupina == 0) { ViewBag.ListRegion = "Vše"; }
            if (Skupina == 1) { ViewBag.ListRegion = "Česká Republika"; }
            if (Skupina == 2) { ViewBag.ListRegion = "Polsko"; }
            if (Skupina == 3) { ViewBag.ListRegion = "Slovensko a Maďarsko"; }
            if (Skupina == 4) { ViewBag.ListRegion = "Ostatní"; }
            return View(rl);
        }


        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult DateView(int? Rok, int? Mesic, int? Den, int? Region)
        {
            
            if (Rok == null) { Rok = System.DateTime.Now.Year; }
            if (Mesic == null) { Mesic = System.DateTime.Now.Month; }
            if (Den == null) { Mesic = System.DateTime.Now.Day; }

            List<Revize> list = new List<Revize>();
            list = Revize.GetByDate(Mesic.Value, Rok.Value, Den.Value, Region.Value);
            list = Revize.LoopRevizeAndUpdateBatery(list);
            ViewBag.Region = Region;
            return View(list);
        }

        // GET: Revize/Edit/5
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Replan(int? id, int? Region)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true).OrderBy(s => s.NazevStatusuRevize), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            ViewBag.Region = Region;
            return View(revize);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Replan([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V, Projekt, Nabidka")] Revize revize, int Region)
        {
            if (ModelState.IsValid)
            {
                var region = Region;
                try
                {
                    db.Entry(revize).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Replan - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Nahled", "Revize", new { Rok = revize.DatumRevize.Year, Mesic = revize.DatumRevize.Month, Region = region });
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult TiskZaznamuOKontrole(int Id)
        {
            
            ReportDocument rd = new ReportDocument();
            string path2 = @"C:\Logs\Crystal\Servis.rpt";
            rd.Load(path2);
            
            rd.SetParameterValue("Id@", Id);
            
            rd.SetDatabaseLogon("sa", "*2012Versino",
                               "SQL", "Servis", false);
            
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            str.Seek(0, SeekOrigin.Begin);
            string savedFilename = string.Format("Revize_{0}.pdf", Id);
            rd.Close();
            rd.Dispose();

            return File(str, "application/pdf", savedFilename);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public void OpenPDF(int Id, string lang)
        {
            ReportDocument Rel = new ReportDocument();
            string path = $"C:\\Logs\\Crystal\\Servis_{lang}.rpt";
            


            log.Error($"adresa {path}");

            try
            {

                Rel.Load(path);
                Rel.SetParameterValue("Id@", Id);
                Rel.SetDatabaseLogon("sa", "*2012Versino",
                                   "SQL", "Servis", false);

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
            catch { log.Error($"Nena4tena adresa {path}"); }

        }

        [Authorize(Roles = "Administrator,Manager")]
        public void OpenPDFTlkZk(int Id, string lang)
        {
            ReportDocument Rel = new ReportDocument();
            string path = $"C:\\Logs\\Crystal\\ServisTlkZk_{lang}.rpt";



            log.Error($"adresa {path}");

            try
            {

                Rel.Load(path);
                Rel.SetParameterValue("Id@", Id);
                Rel.SetDatabaseLogon("sa", "*2012Versino",
                                   "SQL", "Servis", false);

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
            catch { log.Error($"Nena4tena adresa {path}"); }

        }

        [Authorize(Roles = "Administrator,Manager")]
        public void OpenPDFPotvrzeni(int Id, string lang)
        {
            ReportDocument Rel = new ReportDocument();
            string path = $"C:\\Logs\\Crystal\\Servis2_{lang}.rpt";
            log.Error($"adresa {path}");
            
            try
            {

                Rel.Load(path);
                Rel.SetParameterValue("Id@", Id);
                Rel.SetDatabaseLogon("sa", "*2012Versino",
                                   "SQL", "Servis", false);

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
            catch { log.Error($"Nena4tena adresa {path}"); }
            
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Fill(int? id , int Region)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revize revize = db.Revize.Find(id);
            if (revize == null)
            {
                return HttpNotFound();
            }
            revize.Region = Region;
            ViewBag.Region = Region;
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // POST: Revize/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Fill([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V, Projekt, Nabidka")] Revize revize, int Region)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(revize).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Fill - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                return RedirectToAction("Details", "Revize", new { revize.Id, Region });
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            revize.Region = Region;
            ViewBag.Region = Region;
            return View(revize);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Close(int Id)
        {
            Revize revize = new Revize();
            Revize.CloseRevize(Id);
            

            return RedirectToAction("Nahled", "Revize");
        }
    }
}
