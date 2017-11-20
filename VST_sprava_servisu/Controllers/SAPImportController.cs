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

        

        // POST: Zakaznici/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598
        
        public ActionResult List(string Search)
        {
            SAPOPImportParametr SAPOPlist = new SAPOPImportParametr();

            if (Search == null)
            { Search = ""; }
            

            SAPOPlist.Search = Search;
                     
            SAPOPlist.ListSAPOP = SAPOP(Search);
            return View(SAPOPlist);
        }
        public PartialViewResult SAPOPList(List<SAPOP> SAPOP)
        {

            return PartialView(SAPOP);
        }

        public List<SAPContactPerson> SAPContactPerson(string SAPOP)

        {
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();
            string sql = @" Select CntctCode, CardCode, Name, Position, Tel1, Cellolar, E_MailL from OCPR";
            sql = sql + @" Where CardCode = '" + SAPOP + "'";

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

        public ActionResult CPListByOP(string CardCode)
        {
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();
            SAPCP = SAPContactPerson(CardCode);

            return View(SAPCP);
        }

        public List<SAPOP> SAPOP (string Search)
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
                sql = sql + @") and "; }
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

    }
}