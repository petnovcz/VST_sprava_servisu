﻿using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
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
        private Model1Container db = new Model1Container();
        private string connectionString = @"Data Source=sql;Initial Catalog=SBO;User ID=sa;Password=*2012Versino";

        // GET: Revize
        public ActionResult Index()
        {
            var revize = db.Revize.Include(r => r.Provoz).Include(r => r.StatusRevize);
            return View(revize.ToList());
        }

        

        // GET: Revize/Details/5
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
            catch { }
            try
            {
                ViewBag.ListDateFrom = Session["List_DateFrom"].ToString();
            }
            catch { }
            try
            {
                ViewBag.ListDateTo = Session["List_DateTo"].ToString();
            }
            catch { }
            try
            {
                ViewBag.ListStatusRevize = Session["List_StatusRevize"].ToString();
            }
            catch { }

            ViewBag.Region = Region;
            return View(revize);
        }

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
            catch { }

            try
            {
                var ListDateFrom = Session["List_DateFrom"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateFrom);
                ViewBag.ListDateFrom = xx;
            }
            catch { }
            try
            {
                var ListDateTo = Session["List_DateTo"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateTo);
                ViewBag.ListDateTo = xx;
            }
            catch { }
            try
            {
                ViewBag.ListStatus = Session["List_Status"].ToString();
            }
            catch { }
            revize.Region = Region.Value;
            ViewBag.Region = Region;
            return View(revize);
        }

        // GET: Revize/Create
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
        public ActionResult Create([
        Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V")] Revize revize)
        {
            if (ModelState.IsValid)
            {
                db.Revize.Add(revize);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // GET: Revize/Edit/5
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
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            ViewBag.Region = Region;
            return View(revize);
        }

        // POST: Revize/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V")] Revize revize, int Region)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details","Revize",new { Id = revize.Id, Region = Region});
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        // GET: Revize/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            Revize revize = db.Revize.Find(id);
            db.Revize.Remove(revize);
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

        public ActionResult MonthViewHeader(int Rok, int Mesic, int? Region)
        {
            ViewBag.ThisYear = DateTime.Now.Year;
            ViewBag.NextYear = DateTime.Now.Year + 1;
            ViewBag.Rok = Rok;
            ViewBag.Mesic = Mesic;
            ViewBag.Region = Region;
            return View();
        }
        public ActionResult Nahled (int? Rok, int? Mesic, int? Region)
        {
            int? session_rok = null;
            int? session_mesic = null;
            int? session_region = null;
            try
            {
                session_rok = Convert.ToInt32(Session["Rok"].ToString());
            }
            catch {}
            try
            {
                session_mesic = Convert.ToInt32(Session["Mesic"].ToString());
            }
            catch { }
            try
            {
                session_region = Convert.ToInt32(Session["List_Skupina"].ToString());
            }
            catch { }
            if (Rok != null) { Session["Rok"] = Rok; }
            if (Mesic != null) { Session["Mesic"] = Mesic; }
            if (Region != null) { Session["List_Skupina"] = Region; }


            if ((Rok == null) && (session_rok==null)) { Rok = System.DateTime.Now.Year; }
            if ((Rok == null) && (session_rok != null)) { Rok = session_rok; }
            if ((Mesic == null) && (session_mesic == null)) { Mesic = System.DateTime.Now.Month; }
            if ((Mesic == null) && (session_mesic != null)) { Mesic = session_mesic; }

            DateTime date1 = new DateTime(Rok.Value, Mesic.Value, 1);
            var x = (int)date1.DayOfWeek - 1;

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
            catch { }

            try
            {
                var xListDateFrom = Session["List_DateFrom"].ToString();
                DateTime xx = Convert.ToDateTime(xListDateFrom);
                ListDateFrom = xx;
            }
            catch { }
            try
            {
                var ListDateTox = Session["List_DateTo"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateTox);
                ListDateTo = xx;
            }
            catch { }
            try
            {
                ListStatus = Session["List_Status"].ToString();
            }
            catch { }

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
            catch { }

            try
            {
                var xListDateFrom = Session["List_DateFrom"].ToString();
                DateTime xx = Convert.ToDateTime(xListDateFrom);
                ListDateFrom = xx;
            }
            catch { }
            try
            {
                var ListDateTox = Session["List_DateTo"].ToString();
                DateTime xx = Convert.ToDateTime(ListDateTox);
                ListDateTo = xx;
            }
            catch { }
            try
            {
                ListStatus = Session["List_Status"].ToString();
            }
            catch { }






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
            rl.Revize = x.ToList();
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
                //.Include(r => r.);
            if (DateFrom != null) { x = x.Where(r => r.DatumRevize >= DateFrom);   }
            if (DateTo != null) { x = x.Where(r => r.DatumRevize <= DateTo); }
            if (Zakaznik != null) { x = x.Where(r => r.Provoz.ZakaznikId == Zakaznik); }
            if (Status != null) { x = x.Where(r => r.StatusRevizeId == Status); }
            if (Skupina != null && Skupina != 0) { x = x.Where(r => r.Provoz.Zakaznik.Region.Skupina == Skupina); }
            rl.Revize = x.ToList();
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



        public ActionResult DateView(int? Rok, int? Mesic, int? Den, int? Region)
        {
            
            if (Rok == null) { Rok = System.DateTime.Now.Year; }
            if (Mesic == null) { Mesic = System.DateTime.Now.Month; }
            if (Den == null) { Mesic = System.DateTime.Now.Day; }

            List<Revize> list = new List<Revize>();
            list = Revize.GetByDate(Mesic.Value, Rok.Value, Den.Value, Region.Value);

            ViewBag.Region = Region;
            return View(list);
        }

        // GET: Revize/Edit/5
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
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            ViewBag.Region = Region;
            return View(revize);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Replan([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V")] Revize revize, int Region)
        {
            if (ModelState.IsValid)
            {
                var region = Region;
                db.Entry(revize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Nahled", "Revize", new { Rok = revize.DatumRevize.Year, Mesic = revize.DatumRevize.Month, Region = region });
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize.Where(s => s.Realizovana != true), "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            return View(revize);
        }

        public ActionResult TiskZaznamuOKontrole(int Id)
        {
            List<VypocetPlanuRevizi> list = VypocetPlanuRevizi.Run(connectionString);
            ReportDocument rd = new ReportDocument();
            // Your .rpt file path will be below
            rd.Load(Path.Combine(Server.MapPath("~/Servis.rpt")));
            //set dataset to the report viewer.
            rd.SetParameterValue("Id@", Id);
            //rd.ParameterFields.
            rd.SetDatabaseLogon("sa", "*2012Versino",
                               "SQL", "SBO", false);
            ;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            str.Seek(0, SeekOrigin.Begin);
            string savedFilename = string.Format("Revize_{0}.pdf", Id);

            return File(str, "application/pdf", savedFilename);
        }

        public void OpenPDF(int Id)
        {
            ReportDocument Rel = new ReportDocument();
            Rel.Load(Path.Combine(Server.MapPath("~/Servis.rpt")));
            Rel.SetParameterValue("Id@", Id);
            //rd.ParameterFields.
            Rel.SetDatabaseLogon("sa", "*2012Versino",
                               "SQL", "Servis", false);
            
            BinaryReader stream = new BinaryReader(Rel.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
            Response.Flush();
            Response.Close();
        }

        public void OpenPDFPotvrzeni(int Id)
        {
            ReportDocument Rel = new ReportDocument();
            Rel.Load(Path.Combine(Server.MapPath("~/Servis2.rpt")));
            Rel.SetParameterValue("Id@", Id);
            //rd.ParameterFields.
            Rel.SetDatabaseLogon("sa", "*2012Versino",
                               "SQL", "Servis", false);

            BinaryReader stream = new BinaryReader(Rel.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
            Response.Flush();
            Response.Close();
        }

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

        public ActionResult Fill([Bind(Include = "Id,ProvozId,DatumRevize,StatusRevizeId,DatumVystaveni,ZjistenyStav,ProvedeneZasahy,OpatreniKOdstraneni,KontrolaProvedenaDne,PristiKontrola,Rok,Pololeti,UmisteniId, Baterie, Pyro, TlkZk, AP, S, RJ, M, V")] Revize revize, int Region)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Revize", new { Id = revize.Id, Region = Region });
            }
            ViewBag.ProvozId = new SelectList(db.Provoz, "Id", "NazevProvozu", revize.ProvozId);
            ViewBag.StatusRevizeId = new SelectList(db.StatusRevize, "Id", "NazevStatusuRevize", revize.StatusRevizeId);
            revize.Region = Region;
            ViewBag.Region = Region;
            return View(revize);
        }

        public ActionResult Close(int Id)
        {
            Revize revize = new Revize();
            Revize.CloseRevize(Id);
            //dohledat revizi
            //změnit status revize 
            //vyhledat všechny RevizeSC
            //pro každou ReviziSC dohledat SC provozu a update datumu revize 

            return RedirectToAction("Details", "Revize", new { Id = Id });
        }
    }
}
