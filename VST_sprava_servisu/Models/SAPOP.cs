using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public partial class SAPOP
    {

        [Key]
        [Required(ErrorMessage = "Zadejte prosím kód OP")] 
        public string CardCode { get; set; }
        [Required(ErrorMessage = "Zadejte prosím Název OP")]
        public string CardName { get; set; }
        [Required(ErrorMessage = "Zadejte prosím adresu OP")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Zadejte prosím město OP")]
        public string City { get; set; }
        [Required(ErrorMessage = "Zadejte prosím PSČ OP")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Zadejte prosím stát OP")]
        public string Country { get; set; }

        public string LicTradNum { get; set; }
        public string VatIdUnCmp { get; set; }
        [Required(ErrorMessage = "Zadejte prosím region")]
        public int RegionId { get; set; }
        [Required(ErrorMessage = "Zadejte prosím jazyk")]
        public int JazykId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }


        public int Open { get; set; }
        public int Total { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPOP");

        /// <summary>
        /// Vytvoření seznamu obchodních partnerů z IS SAP, které ještě nebyly importovány v SAP
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Manager")]
        public static List<SAPOP> SAPOPList(string Search)
        {
            List<SAPOP> listocrd = new List<SAPOP>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" Select CardCode,CardName,Address,City, ZipCode,Country,LicTradNum,VatIdUnCmp,");
            sql.Append(" ((select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' and X.U_Status not in ('7', '8')) ");
            sql.Append(" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' and X.U_Status not in ('7', '8'))");
            sql.Append(" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' and X.U_Status not in ('7', '8'))) as 'Open',");
            sql.Append(" ( (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' )");
            sql.Append(" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' )");
            sql.Append("  + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' )) as 'Total'");

            if (Search == "*")
            { sql.Append(" from OCRD where "); }
            else
            {
                sql.Append($" from OCRD where (CHARINDEX(N'{Search}', CardName) > 0 ");
                sql.Append($" or CHARINDEX(N'{Search}', CardCode) > 0 ");
                sql.Append($" or CHARINDEX(N'{Search}', Address) > 0 ");
                sql.Append(") and ");
            }
            sql.Append(" CardType = 'C' and");
            sql.Append(" ((Select count(*) from OINV Z where Z.CardCode = CardCode ) > 0) and");
            sql.Append($" ((select COUNT(*) from [{RS_dtb}].[dbo].[Zakaznik] Z where Z.KodSAP COLLATE DATABASE_DEFAULT = CardCode COLLATE DATABASE_DEFAULT) = 0)");

            sql.Append(" order by");
            sql.Append(" ((select COUNT(*) from [@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' and X.U_Status not in ('7','8')) ");
            sql.Append("  + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' and X.U_Status not in ('7', '8')) ");
            sql.Append(" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' and X.U_Status not in ('7', '8')) ) desc,");
            sql.Append(" ( (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'R' )");
            sql.Append(" + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'S' )");
            sql.Append("  + (select COUNT(*) from[@VCZ_CT_PRJ] X where x.U_CardCode = CardCode and x.U_Type = 'P' )");
            sql.Append(") desc ");

            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql.ToString();

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
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        ocrd.Address = dr.GetString(dr.GetOrdinal("Address"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        ocrd.City = dr.GetString(dr.GetOrdinal("City"));
                    }
                    catch(Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        ocrd.ZipCode = dr.GetString(dr.GetOrdinal("ZipCode"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        ocrd.Country = dr.GetString(dr.GetOrdinal("Country"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        ocrd.LicTradNum = dr.GetString(dr.GetOrdinal("LicTradNum"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        ocrd.VatIdUnCmp = dr.GetString(dr.GetOrdinal("VatIdUnCmp"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    ocrd.Open = dr.GetInt32(dr.GetOrdinal("Open"));
                    ocrd.Total = dr.GetInt32(dr.GetOrdinal("Total"));
                    listocrd.Add(ocrd);
                }
            }
            cnn.Close();
            return listocrd;
        }


        [Authorize(Roles = "Administrator,Manager")]
        public static SAPOP GetSAPOPByCode(string KodOP)
        {
            SAPOP sapOP = new SAPOP();
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" Select CardCode,CardName,Address,City, ZipCode,Country,LicTradNum,VatIdUnCmp, Phone1, E_Mail, LangCode, Territory");
            sql.Append($" from OCRD where CardCode = '{KodOP}' ");

            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql.ToString();
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
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.Address = dr.GetString(dr.GetOrdinal("Address"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.City = dr.GetString(dr.GetOrdinal("City"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.ZipCode = dr.GetString(dr.GetOrdinal("ZipCode"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.Country = dr.GetString(dr.GetOrdinal("Country"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.LicTradNum = dr.GetString(dr.GetOrdinal("LicTradNum"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.VatIdUnCmp = dr.GetString(dr.GetOrdinal("VatIdUnCmp"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.Phone = dr.GetString(dr.GetOrdinal("Phone1"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.Email = dr.GetString(dr.GetOrdinal("E_Mail"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.JazykId = dr.GetInt32(dr.GetOrdinal("LangCode"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapOP.RegionId = dr.GetInt32(dr.GetOrdinal("Territory"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                }
            }
            cnn.Close();
            return sapOP;
        }


    }
}