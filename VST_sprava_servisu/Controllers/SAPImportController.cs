using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace VST_sprava_servisu
{
    public class SAPImportController : Controller
    {
        private string connectionString = @"Data Source=sql;Initial Catalog=SBO_TEST;User ID=sa;Password=*2012Versino";
        private Model1Container db = new Model1Container();


        // POST: Zakaznici/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598

        /// <summary>
        /// Seznam všech ještě nenaimportovaných obchodních partnerů z IS SAP 
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public ActionResult List(string Search)
        {
            SAPOPImportParametr SAPOPlist = new SAPOPImportParametr();

            if (Search == null)
            { Search = ""; }
            

            SAPOPlist.Search = Search;
                     
            SAPOPlist.ListSAPOP = SAPOP(Search);
            return View(SAPOPlist);
        }

        /*public PartialViewResult SAPOPList(List<SAPOP> SAPOP)
        {

            return PartialView(SAPOP);
        }*/

        /// <summary>
        /// Vytvoření seznamu obchodních partnerů z IS SAP, které ještě nebyly importovány v SAP
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public List<SAPOP> SAPOP(string Search)
        {
            List<SAPOP> listocrd = new List<SAPOP>();
            string sql = @" Select CardCode,CardName,Address,City, ZipCode,Country,LicTradNum,VatIdUnCmp,";
            sql = sql + @" ((select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' and X.U_Status not in ('7', '8')) ";
            sql = sql + @" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' and X.U_Status not in ('7', '8'))";
            sql = sql + @" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' and X.U_Status not in ('7', '8'))) as 'Open',";
            sql = sql + @" ( (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' )";
            sql = sql + @" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' )";
            sql = sql + @"  + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' )) as 'Total'";

            if (Search == "")
            { sql = sql + @" from OCRD where "; }
            else
            {
                sql = sql + @" from OCRD where (CHARINDEX(N'" + Search + "', CardName) > 0 ";
                sql = sql + @" or CHARINDEX(N'" + Search + "', CardCode) > 0 ";
                sql = sql + @" or CHARINDEX(N'" + Search + "', Address) > 0 ";
                sql = sql + @") and ";
            }
            sql = sql + @" CardType = 'C' and";
            sql = sql + @" ((Select count(*) from OINV Z where Z.CardCode = CardCode ) > 0) and";
            sql = sql + @" ((select COUNT(*) from[Servis].[dbo].[Zakaznik] Z where Z.KodSAP COLLATE DATABASE_DEFAULT = CardCode COLLATE DATABASE_DEFAULT) = 0)";
            sql = sql + @"and ((select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R') + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' ) + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' )) > 0";
            sql = sql + @" order by";
            sql = sql + @" ((select COUNT(*) from [@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' and X.U_Status not in ('7','8')) ";
            sql = sql + @"  + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' and X.U_Status not in ('7', '8')) ";
            sql = sql + @" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' and X.U_Status not in ('7', '8')) ) desc,";
            sql = sql + @" ( (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' )";
            sql = sql + @" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' )";
            sql = sql + @"  + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' )";
            sql = sql + @") desc ";

            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;

            cnn.Open();

            cmd.ExecuteNonQuery();

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {
                    SAPOP ocrd = new SAPOP();
                    if (dr.GetString(dr.GetOrdinal("CardCode")) != null)
                    {
                        ocrd.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    try
                    {
                        ocrd.CardName = dr.GetString(dr.GetOrdinal("CardName"));
                    }
                    catch { }
                    try
                    {
                        ocrd.Address = dr.GetString(dr.GetOrdinal("Address"));
                    }
                    catch { }
                    try
                    {
                        ocrd.City = dr.GetString(dr.GetOrdinal("City"));
                    }
                    catch { }
                    try
                    {
                        ocrd.ZipCode = dr.GetString(dr.GetOrdinal("ZipCode"));
                    }
                    catch { }
                    try
                    {
                        ocrd.Country = dr.GetString(dr.GetOrdinal("Country"));
                    }
                    catch { }
                    try
                    {
                        ocrd.LicTradNum = dr.GetString(dr.GetOrdinal("LicTradNum"));
                    }
                    catch { }
                    try
                    {
                        ocrd.VatIdUnCmp = dr.GetString(dr.GetOrdinal("VatIdUnCmp"));
                    }
                    catch { }
                    ocrd.Open = dr.GetInt32(dr.GetOrdinal("Open"));
                    ocrd.Total = dr.GetInt32(dr.GetOrdinal("Total"));
                    listocrd.Add(ocrd);
                }
            }
            cnn.Close();
            return listocrd;
        }
        public SAPOP GetSAPOPByCode(string KodOP)
        {
            SAPOP sapOP = new SAPOP();
            string sql = @" Select CardCode,CardName,Address,City, ZipCode,Country,LicTradNum,VatIdUnCmp, Phone1, E_Mail, LangCode, Territory";
            sql = sql + @" from OCRD where CardCode = '" + KodOP + "' ";

            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;
            cnn.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {

                    if (dr.GetString(dr.GetOrdinal("CardCode")) != null)
                    {
                        sapOP.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    try
                    {
                        sapOP.CardName = dr.GetString(dr.GetOrdinal("CardName"));
                    }
                    catch { }
                    try
                    {
                        sapOP.Address = dr.GetString(dr.GetOrdinal("Address"));
                    }
                    catch { }
                    try
                    {
                        sapOP.City = dr.GetString(dr.GetOrdinal("City"));
                    }
                    catch { }
                    try
                    {
                        sapOP.ZipCode = dr.GetString(dr.GetOrdinal("ZipCode"));
                    }
                    catch { }
                    try
                    {
                        sapOP.Country = dr.GetString(dr.GetOrdinal("Country"));
                    }
                    catch { }
                    try
                    {
                        sapOP.LicTradNum = dr.GetString(dr.GetOrdinal("LicTradNum"));
                    }
                    catch { }
                    try
                    {
                        sapOP.VatIdUnCmp = dr.GetString(dr.GetOrdinal("VatIdUnCmp"));
                    }
                    catch { }

                    try
                    {
                        sapOP.Phone = dr.GetString(dr.GetOrdinal("Phone1"));
                    }
                    catch { }
                    try
                    {
                        sapOP.Email = dr.GetString(dr.GetOrdinal("E_Mail"));
                    }
                    catch { }
                    try
                    {
                        sapOP.JazykId = dr.GetInt32(dr.GetOrdinal("LangCode"));
                    }
                    catch { }
                    try
                    {
                        sapOP.RegionId = dr.GetInt32(dr.GetOrdinal("Territory"));
                    }
                    catch { }

                }
            }
            cnn.Close();
            return sapOP;
        }
        public ActionResult GenerateOPfromSAP(string kodOP)
        {
            SAPOP sapOP = new SAPOP();
            sapOP = GetSAPOPByCode(kodOP);

            var Zcontroller = DependencyResolver.Current.GetService<ZakazniciController>();
            Zcontroller.ControllerContext = new ControllerContext(this.Request.RequestContext, Zcontroller);

            bool success = Zcontroller.CreateFromSAPdata(sapOP);
            if (success == true)
            {
                ViewBag.Result = "Import proběhl OK";
            }
            SAPOPImportParametr SAPOPlist = new SAPOPImportParametr();
            string Search = "";
            if (Search == null)
            { Search = ""; }


            SAPOPlist.Search = Search;

            SAPOPlist.ListSAPOP = SAPOP(Search);
            return View("List", SAPOPlist);
        }


        // KONTAKTNÍ OSOBY
        public List<SAPContactPerson> SAPContactPerson(string SAPOP)

        {
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();
            string sql = @" Select CntctCode, CardCode, Name, Position, Tel1, Cellolar, E_MailL from OCPR";
            sql = sql + @" Where CardCode = '" + SAPOP + "' and";
            sql = sql + @" (select count(*) from [Servis].[dbo].[KontakniOsoba] where SapId = CntctCode ) = 0";


            //(select COUNT(*) from[Servis].[dbo].[Zakaznik] Z where Z.KodSAP COLLATE DATABASE_DEFAULT = CardCode COLLATE DATABASE_DEFAULT) = 0)";
            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;
            cnn.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {
                    SAPContactPerson sapcp = new SAPContactPerson();
                    if (dr.GetString(dr.GetOrdinal("CardCode")) != null)
                    {
                        sapcp.CntctCode = dr.GetInt32(dr.GetOrdinal("CntctCode"));
                    }
                    try
                    {
                        sapcp.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    catch { }
                    try
                    {
                        sapcp.Name = dr.GetString(dr.GetOrdinal("Name"));
                    }
                    catch { }
                    try
                    {
                        sapcp.Position = dr.GetString(dr.GetOrdinal("Position"));
                    }
                    catch { }
                    try
                    {
                        sapcp.Tel1 = dr.GetString(dr.GetOrdinal("Tel1"));
                    }
                    catch { }
                    try
                    {
                        sapcp.Cellolar = dr.GetString(dr.GetOrdinal("Cellolar"));
                    }
                    catch { }
                    try
                    {
                        sapcp.E_MaiL = dr.GetString(dr.GetOrdinal("E_MailL"));
                    }
                    catch { }

                    SAPCP.Add(sapcp);
                }
            }
            cnn.Close();


            return SAPCP;
        }
        public ActionResult ImportSAPCP(string CardCode, int Zakaznik)

        {
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();
            SAPCP = SAPContactPerson(CardCode);

            var Zcontroller = DependencyResolver.Current.GetService<KontaktniOsobyController>();
            Zcontroller.ControllerContext = new ControllerContext(this.Request.RequestContext, Zcontroller);

            foreach (var item in SAPCP)
            {
                bool result = Zcontroller.Generate(Zakaznik,item.Name,item.Position, item.Tel1, item.E_MaiL, item.CntctCode);
            }
            return RedirectToAction("Index", "KontaktniOsoby", new { Zakaznik = Zakaznik });
        }
        public ActionResult CPListByOP(string CardCode)
        {
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();
            SAPCP = SAPContactPerson(CardCode);

            return View(SAPCP);
        }

        //SAP ADRESY - IMPORTOVÁNY JAKO PROVOZY
        private List<SAPDeliveryAddress> LoadSAPDeliveryAddresses(string CardCode, int Zakaznik)
        {
            List<SAPDeliveryAddress> SAPDAList = new List<SAPDeliveryAddress>();

            string sql = @" Select Address, CardCode, Street, ZipCode, City, Country from CRD1";
            sql = sql + @" where CardCode = '" + CardCode + "' and AdresType = 'S' and";
            sql = sql + @" (select count(*) from [Servis].[dbo].[Provoz] X where X.ZakaznikId = '" + Zakaznik + "' and X.SAPAddress COLLATE DATABASE_DEFAULT = Address COLLATE DATABASE_DEFAULT) = 0 ";

            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;

            cnn.Open();

            cmd.ExecuteNonQuery();

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {
                    SAPDeliveryAddress sapda = new SAPDeliveryAddress();
                    if (dr.GetString(dr.GetOrdinal("CardCode")) != null)
                    {
                        sapda.Address = dr.GetString(dr.GetOrdinal("Address"));
                    }
                    try
                    {
                        sapda.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    catch { }
                    try
                    {
                        sapda.Street = dr.GetString(dr.GetOrdinal("Street"));
                    }
                    catch { }
                    try
                    {
                        sapda.ZipCode = dr.GetString(dr.GetOrdinal("ZipCode"));
                    }
                    catch { }
                    try
                    {
                        sapda.City = dr.GetString(dr.GetOrdinal("City"));
                    }
                    catch { }
                    try
                    {
                        sapda.Country = dr.GetString(dr.GetOrdinal("Country"));
                    }
                    catch { }


                    SAPDAList.Add(sapda);
                }
            }
            cnn.Close();

            return SAPDAList;
        }         
        public ActionResult ImportSAPAddress(string CardCode, int Zakaznik)

        {
            List<SAPDeliveryAddress> SAPDAList = new List<SAPDeliveryAddress>();
            SAPDAList = LoadSAPDeliveryAddresses(CardCode, Zakaznik);

            var Zcontroller = DependencyResolver.Current.GetService<ProvozyController>();
            Zcontroller.ControllerContext = new ControllerContext(this.Request.RequestContext, Zcontroller);

            foreach (var item in SAPDAList)
            {
                bool result = Zcontroller.Generate(item.Address, item.CardCode, item.Street, item.City, item.Country, item.Country, Zakaznik);
            }
            return RedirectToAction("Details", "Zakaznici", new { id = Zakaznik });
        }


        // SAP Artikly - import artiklů z SAP do Servisu
        /// <summary>
        /// Seznam artiklů nenaimportovaných v Servise z IS SAP , která mají správu sériových čísel
        /// </summary>
        /// <returns></returns>
        public ActionResult SAPItems ()
        {
            List<SAPItem> SAPItemsList = new List<SAPItem>();
            string sql = @"select ItemCode, ItemName, t1.ItmsGrpNam from oitm t0 left join OITB t1 on t0.ItmsGrpCod = t1.ItmsGrpCod  where ManSerNum = 'Y'";
            sql = sql + @"and ((select count(*) from [Servis].[dbo].[Artikl] where KodSAP COLLATE DATABASE_DEFAULT = ItemCode COLLATE DATABASE_DEFAULT) = 0)";

            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;
            cnn.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {
                    SAPItem sapitem = new SAPItem();
                    try
                    {
                        sapitem.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                    }
                    catch { }
                    try
                    {
                        sapitem.ItemName = dr.GetString(dr.GetOrdinal("ItemName"));
                    }
                    catch { }
                    try
                    {
                        sapitem.ItmsGrpNam = dr.GetString(dr.GetOrdinal("ItmsGrpNam"));
                    }
                    catch { }


                    SAPItemsList.Add(sapitem);
                }
            }
            cnn.Close();

            return View(SAPItemsList);
        }
        public SAPItem GetSAPItemByCode(string ItemCode)
        {
            SAPItem sapItem = new SAPItem();
            string sql = @" select ItemCode, ItemName, t1.ItmsGrpNam from oitm t0 left join OITB t1 on t0.ItmsGrpCod = t1.ItmsGrpCod  where ManSerNum = 'Y'";
            sql = sql + @" and ItemCode = '" + ItemCode + "' ";

            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;
            cnn.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {

                    if (dr.GetString(dr.GetOrdinal("ItemCode")) != null)
                    {
                        sapItem.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                    }
                    try
                    {
                        sapItem.ItemName = dr.GetString(dr.GetOrdinal("ItemName"));
                    }
                    catch { }
                    try
                    {
                        sapItem.ItmsGrpNam = dr.GetString(dr.GetOrdinal("ItmsGrpNam"));
                    }
                    catch { }
                    

                }
            }
            cnn.Close();
            return sapItem;
        }
        public ActionResult GenerateItemfromSAP(string ItemCode)
        {
            SAPItem sapItem = new SAPItem();
            sapItem = GetSAPItemByCode(ItemCode);

            var Zcontroller = DependencyResolver.Current.GetService<ArtiklyController>();
            Zcontroller.ControllerContext = new ControllerContext(this.Request.RequestContext, Zcontroller);

            bool success = Zcontroller.CreateFromSAPdata(sapItem);
            if (success == true)
            {
                ViewBag.Result = "Import proběhl OK";
            }
            
            return RedirectToAction("SAPItems", "SAPImport");
        }


        // SAP Sériová čísla - import sériových čísel a jejich pohybů z SAP do Servisu
        public ActionResult SAPSCList(string OPSAPkod, int Zakaznik)
        {
            List<SAPSerioveCislo> SAPSCList = new List<SAPSerioveCislo>();
            string sql = @" SELECT t2.id as 'ArticlId', t2.KodSAP, t2.Nazev, T0.[IntrSerial] as 'SerioveCislo' ,MIN(t0.InDate) as 'Vyrobeno', MAX(t1.docdate) as 'Dodano' FROM OSRI  T0 INNER JOIN SRI1 T1 ON T0.ItemCode = T1.ItemCode and T0.SysSerial = T1.SysSerial";
            sql = sql + @" left join[Servis].[dbo].[Artikl] t2 on t1.ItemCode COLLATE DATABASE_DEFAULT = t2.KodSAP COLLATE DATABASE_DEFAULT";
            sql = sql + @" where t1.Direction = 1 and";
            sql = sql + @" ((select count (*) from [Servis].[dbo].[Artikl] where KodSAP COLLATE DATABASE_DEFAULT = t1.ItemCode) > 0)";
            sql = sql + @" and((select count (*) from [Servis].[dbo].[Zakaznik] where KodSAP COLLATE DATABASE_DEFAULT = t1.CardCode) > 0)";
            sql = sql + @" and T1.CardCode = '" + OPSAPkod + "'";
            sql = sql + @" group by T0.[IntrSerial], t2.id, t2.kodSAp, t2.nazev";
            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;
            cnn.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {
                    SAPSerioveCislo sapitem = new SAPSerioveCislo();
                    try
                    {
                        sapitem.SerioveCislo = dr.GetString(dr.GetOrdinal("SerioveCislo"));
                    }
                    catch { }
                    try
                    {
                        sapitem.ArticlId = dr.GetInt32(dr.GetOrdinal("ArticlId"));
                    }
                    catch { }
                    try
                    {
                        sapitem.NazevArtiklu = dr.GetString(dr.GetOrdinal("Nazev"));
                    }
                    catch { }
                    try
                    {
                        sapitem.DatumDodani = dr.GetDateTime(dr.GetOrdinal("Dodano"));
                    }
                    catch { }
                    try
                    {
                        sapitem.DatumVyroby = dr.GetDateTime(dr.GetOrdinal("Vyrobeno"));
                    }
                    catch { }
                    try
                    {
                        sapitem.KodSAP = dr.GetString(dr.GetOrdinal("KodSAP"));
                    }
                    catch { }
                    sapitem.Zakaznik = Zakaznik;
                    sapitem.ZakaznikSAPKod = OPSAPkod;
                    sapitem.Provoz = new SelectList(db.Provoz.Where(m => m.ZakaznikId == Zakaznik), "Id", "NazevProvozu");
                    SAPSCList.Add(sapitem);
                }
            }
            cnn.Close();
            ViewBag.Zakaznik = OPSAPkod;
            ViewBag.ZakaznikId = Zakaznik;
            return View(SAPSCList);
        }

        [HttpGet]
        public ActionResult SCImport(SAPSerioveCislo SAPSC)
        {


            this.ViewData["Provoz"] = new SelectList(db.Provoz.Where(m => m.ZakaznikId == SAPSC.Zakaznik), "Id", "NazevProvozu");
            return View(SAPSC);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SCImport([Bind(Include = "ArtiklId,SAPKod,SC,DatumVyroby,DatumDodani,ZakaznikSAP,ZakaznikId,Provoz")] SCImport scimport)
        {

            //return RedirectToAction("SAPSCList", new { OPSAPkod = scimport.ZakaznikSAPKod, Zakaznik = scimport.Zakaznik });
            return View();
        }
    }
}