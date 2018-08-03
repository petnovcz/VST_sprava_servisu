using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Configuration;
using System.Text;

namespace VST_sprava_servisu
{
    public class SAPImportController : Controller
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
        private readonly string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
        private readonly string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;
        private Model1Container db = new Model1Container();


        // POST: Zakaznici/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598

        /// <summary>
        /// Seznam všech ještě nenaimportovaných obchodních partnerů z IS SAP 
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult List(string Search)
        {
            SAPOPImportParametr SAPOPlist = new SAPOPImportParametr();

            if (Search == null)
            { Search = ""; }
            SAPOPlist.Search = Search;
            SAPOPlist.ListSAPOP = SAPOP.SAPOPList(Search);
            return View(SAPOPlist);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public PartialViewResult SAPOPList(string Search)
        {
            List<SAPOP> sapop = new List<SAPOP>();
            sapop = SAPOP.SAPOPList(Search);
            return PartialView(sapop);
        }

        
        

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult GenerateOPfromSAP(string kodOP)
        {
            SAPOP sapOP = new SAPOP();
            sapOP = SAPOP.GetSAPOPByCode(kodOP);
            bool jazyk = Jazyk.ValidateValue(sapOP.JazykId);
            bool region = Region.ValidateValue(sapOP.RegionId);
            bool success = Zakaznik.CreateFromSAPdata(sapOP);
            if (success == true)
            {
                ViewBag.Result = "Import proběhl OK";
            }
            else
            {
                StringBuilder result = new StringBuilder();
                result.Append("Import neproběhl:");
                if (jazyk == false)
                {
                    result.Append("Jazyk tiskové šablony nastavený na kartě obchodního partnera v SAP není nastaven v Servisním software. ");
                }
                if (region == false)
                {
                    result.Append("Region nastavený na kartě obchodního partnera v SAP není nastaven v Servisním software. ");
                }
                ViewBag.Result = result.ToString();
            }

            SAPOPImportParametr SAPOPlist = new SAPOPImportParametr();
            string Search = "";
            if (Search == null)
            { Search = ""; }
            SAPOPlist.Search = Search;
            SAPOPlist.ListSAPOP = SAPOP.SAPOPList(Search);
            return View("List", SAPOPlist);
        }


        
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult ImportSAPCP(string CardCode, int Zakaznik)

        {
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();
            SAPCP = SAPContactPerson.SAPContactPersonList(CardCode);

            foreach (var item in SAPCP)
            {
                bool result = KontakniOsoba.Generate(Zakaznik, item.Name, item.Position, item.Tel1, item.E_MaiL, item.CntctCode);
            }
            return RedirectToAction("Index", "KontaktniOsoby", new { Zakaznik });
        }
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult CPListByOP(string CardCode)
        {
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();
            SAPCP = SAPContactPerson.SAPContactPersonList(CardCode);

            return View(SAPCP);
        }

        
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult ImportSAPAddress(string CardCode, int Zakaznik)

        {
            List<SAPDeliveryAddress> SAPDAList = new List<SAPDeliveryAddress>();
            SAPDAList = SAPDeliveryAddress.LoadSAPDeliveryAddresses(CardCode, Zakaznik);

            

            foreach (var item in SAPDAList)
            {
                bool result = Provoz.Generate(item.Address, item.CardCode, item.Street, item.City, item.Country, item.Country, Zakaznik);
            }
            return RedirectToAction("Details", "Zakaznici", new { id = Zakaznik });
        }


        // SAP Artikly - import artiklů z SAP do Servisu
        /// <summary>
        /// Seznam artiklů nenaimportovaných v Servise z IS SAP , která mají správu sériových čísel
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult SAPItems()
        {
            List<SAPItem> SAPItemsList = new List<SAPItem>();
            SAPItemsList = SAPItem.SAPItemList();

            return View(SAPItemsList);
        }
        
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult GenerateItemfromSAP(string ItemCode)
        {
            SAPItem sapItem = new SAPItem();
            sapItem = SAPItem.GetSAPItemByCode(ItemCode);

            bool success = Artikl.CreateFromSAPdata(sapItem);
            if (success == true)
            {
                ViewBag.Result = "Import proběhl OK";
            }

            return RedirectToAction("SAPItems", "SAPImport");
        }


        // SAP Sériová čísla - import sériových čísel a jejich pohybů z SAP do Servisu
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult SAPSCList(string OPSAPkod, int Zakaznik)
        {
            List<SAPSerioveCislo> SAPSCList = new List<SAPSerioveCislo>();
            SAPSCList = SAPSerioveCislo.SAPSCList(OPSAPkod, Zakaznik);
            ViewBag.Zakaznik = OPSAPkod;
            ViewBag.ZakaznikId = Zakaznik;
            return View(SAPSCList);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult SCImport(SAPSerioveCislo SAPSC)
        {


            this.ViewData["Provoz"] = new SelectList(db.Provoz.Where(m => m.ZakaznikId == SAPSC.Zakaznik), "Id", "NazevProvozu");
            return View(SAPSC);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult SCImport([Bind(Include = "ArtiklId,SAPKod,SC,DatumVyroby,DatumDodani,ZakaznikSAP,ZakaznikId,Provoz")] SCImport scimport)
        {

            //return RedirectToAction("SAPSCList", new { OPSAPkod = scimport.ZakaznikSAPKod, Zakaznik = scimport.Zakaznik });
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult TestSC(int Zakaznik, int Provoz, int Umisteni)
        {

            ViewBag.Artikl = new SelectList(db.Artikl.OrderBy(a => a.Nazev), "Id", "Nazev");
            ViewBag.Zakaznik = Zakaznik;
            ViewBag.Provoz = Provoz;
            ViewBag.Umisteni = Umisteni;
            return View(new SCTest());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult TestSC2([Bind(Include = "Zakaznik, Provoz, Umisteni, SC, Artikl")] SCTest sctest)
        {
            // Vyhledat v SAP sériové číslo a zobrazit přehled sériových čísel
            // Vyhledat mezi sériovými čísly zadanými v Servise
            // Vyhleda mezi SC provozy zadanými v Servise

            ViewBag.Artikl = new SelectList(db.Artikl.OrderBy(a => a.Nazev), "Id", "Nazev", sctest.Artikl);
            ViewBag.Zakaznik = sctest.Zakaznik;
            ViewBag.Provoz = sctest.Provoz;
            ViewBag.Umisteni = sctest.Umisteni;
            ViewBag.NazevUmisteni = db.Umisteni.Where(u => u.Id == sctest.Umisteni);
            sctest.SAPSerioveCIslo = SAPSerioveCislo.LoadSCFromSAP(sctest.SC, sctest.Artikl);
            sctest.SerioveCisloList = db.SerioveCislo.Where(s => s.SerioveCislo1 == sctest.SC).Include(a => a.Artikl);
            sctest.SCProvozuList = db.SCProvozu.Where(s => s.SerioveCislo.SerioveCislo1 == sctest.SC && s.Status.Aktivni == true )
                .Include(a => a.Provoz).Include(a=>a.Umisteni1).Include(p=>p.Provoz.Zakaznik);
            return View(sctest);
        }
        

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult ImportSCtoServis([Bind(Include = "Zakaznik, Provozy, Umisteni, SerioveCislo, ArtiklId, DatumVyroby, DatumVymeny, DatumDodani, Submitted, DatumRevize, DatumBaterie, DatumPyro, DatumTlkZk, DatumPrirazeni, Lokace, Znaceni,Baterie,Proverit, BaterieArtikl,UpravenaPeriodaRevize,UpravenaPeriodaBaterie,UpravenaPeriodaPyro,UpravenaPeriodaTlkZk,DatumRevizeTlakoveNadoby,DatumVnitrniRevizeTlakoveNadoby,UpravenaPeriodaRevizeTlakoveNadoby,UpravenaPeriodaVnitrniRevizeTlakoveNadoby ")] SCImport scimport)
        {
            int id = 0;
            int idscprovozu = 0;
            //if (scimport.DatumVyroby == DateTime.MinValue) { scimport.DatumVyroby = DateTime.Now; }
            //if (scimport.DatumDodani == DateTime.MinValue) { scimport.DatumDodani = DateTime.Now; }
            if (scimport.DatumPosledniZmeny == DateTime.MinValue) { scimport.DatumPosledniZmeny = DateTime.Now; }
            

            if (scimport.Submitted == true)
            {
                if (scimport.DatumPrirazeni == null) { scimport.DatumPrirazeni = scimport.DatumDodani; }
                //if (scimport.DatumTlkZk == null) { scimport.DatumTlkZk = scimport.DatumVyroby; }
                id = SerioveCislo.AddSeriovecislo(scimport);
                idscprovozu = SCProvozu.AddSCProvozu(scimport, id);
                // uložení datumů do sesion
                if (scimport.DatumBaterie > DateTime.MinValue || scimport.DatumBaterie == null) { Session["DatumBaterie"] = scimport.DatumBaterie; }
                if (scimport.DatumDodani > DateTime.MinValue || scimport.DatumDodani == null) { Session["DatumDodani"] = scimport.DatumDodani; }
                if (scimport.DatumPrirazeni > DateTime.MinValue || scimport.DatumPrirazeni == null) { Session["DatumPrirazeni"] = scimport.DatumPrirazeni; }
                if (scimport.DatumPyro > DateTime.MinValue || scimport.DatumPyro == null) { Session["DatumPyro"] = scimport.DatumPyro; }
                if (scimport.DatumRevize > DateTime.MinValue || scimport.DatumRevize == null) { Session["DatumRevize"] = scimport.DatumRevize; }
                if (scimport.DatumTlkZk > DateTime.MinValue || scimport.DatumTlkZk == null) { Session["DatumTlkZk"] = scimport.DatumTlkZk; }
                if (scimport.DatumVymeny > DateTime.MinValue || scimport.DatumVymeny == null) { Session["DatumVymeny"] = scimport.DatumVymeny; }
                if (scimport.DatumVyroby > DateTime.MinValue || scimport.DatumVyroby == null) { Session["DatumVyroby"] = scimport.DatumVyroby; }
                if (scimport.UpravenaPeriodaRevize != null) { Session["UpravenaPeriodaRevize"] = scimport.UpravenaPeriodaRevize; }

                if ((id > 0) || (idscprovozu > 0))
                {
                    return RedirectToAction("Details", "Umistenis", new { id = scimport.Umisteni, Provoz = scimport.Provozy, scimport.Zakaznik });
                }
            }
            if ((scimport.Submitted == false) || ((id == 0) || (idscprovozu == 0)))
            {
                // načtení datumů ze session
                try
                {
                    var DatumBaterie = Convert.ToDateTime(Session["DatumBaterie"]);
                    if (DatumBaterie > DateTime.MinValue) { scimport.DatumBaterie = DatumBaterie; }
                    else { scimport.DatumBaterie = null; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumBaterie - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {                    
                    var DatumDodani = Convert.ToDateTime(Session["DatumDodani"]);
                    if (DatumDodani > DateTime.MinValue) { scimport.DatumDodani = DatumDodani; }
                    else { scimport.DatumDodani = DateTime.Now; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumDodani - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {                    
                    var DatumPrirazeni = Convert.ToDateTime(Session["DatumPrirazeni"]);
                    if (DatumPrirazeni > DateTime.MinValue) { scimport.DatumPrirazeni = DatumPrirazeni; }
                    else { scimport.DatumPrirazeni = DateTime.Now; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumPrirazeni - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {                    
                    var DatumPyro = Convert.ToDateTime(Session["DatumPyro"]);
                    if (DatumPyro > DateTime.MinValue) { scimport.DatumPyro = DatumPyro; }
                    else { scimport.DatumPyro = null; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumPyro - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {                   
                    var DatumRevize = Convert.ToDateTime(Session["DatumRevize"]);
                    if (DatumRevize > DateTime.MinValue) { scimport.DatumRevize = DatumRevize; }
                    else { scimport.DatumRevize = null; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumRevize - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {                   
                    var DatumTlkZk = Convert.ToDateTime(Session["DatumTlkZk"]);
                    if (DatumTlkZk > DateTime.MinValue) { scimport.DatumTlkZk = DatumTlkZk; }
                    else { scimport.DatumTlkZk = null; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumTlkZk - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {                   
                    var DatumVymeny = Convert.ToDateTime(Session["DatumVymeny"]);
                    if (DatumVymeny > DateTime.MinValue) { scimport.DatumVymeny = DatumVymeny; }
                    else { scimport.DatumVymeny = null; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumVymeny - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {                    
                    var DatumVyroby = Convert.ToDateTime(Session["DatumVyroby"]);
                    if (DatumVyroby > DateTime.MinValue) { scimport.DatumVyroby = DatumVyroby; }
                    else { scimport.DatumVyroby = DateTime.Now; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumVyroby - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                try
                {
                    int? UpravenaPeriodaRevize = Convert.ToInt32(Session["UpravenaPeriodaRevize"]);
                    if (UpravenaPeriodaRevize != null && UpravenaPeriodaRevize!=0) { scimport.UpravenaPeriodaRevize = UpravenaPeriodaRevize; }
                    else { scimport.UpravenaPeriodaRevize = null; }
                }
                catch (Exception ex)
                {
                    log.Debug("Details - Seassion Read - DatumVyroby - Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }

                ViewBag.Zakaznik = new SelectList(db.Zakaznik, "Id", "NazevZakaznika", scimport.Zakaznik);
                ViewBag.Provozy = new SelectList(db.Provoz.Where(p => p.ZakaznikId == scimport.Zakaznik), "Id", "NazevProvozu", scimport.Provozy);
                ViewBag.Umisteni = new SelectList(db.Umisteni.Where(u => u.ProvozId == scimport.Provozy), "Id", "NazevUmisteni", scimport.Umisteni);
                ViewBag.ArtiklId = new SelectList(db.Artikl.Where(r => r.SkupinaArtiklu1.Id != 129), "Id", "Nazev", scimport.ArtiklId);
                ViewBag.BaterieArtikl = new SelectList(db.Artikl.Where(r=>r.SkupinaArtiklu1.Id==2), "Id", "Nazev");
            }
            return View(scimport);
        }
    }
}