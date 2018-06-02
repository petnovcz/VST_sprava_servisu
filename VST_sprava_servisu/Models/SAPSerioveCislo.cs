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
    public partial class SAPSerioveCislo
    {
        [Key]
        public string SerioveCislo { get; set; }
        public int ArticlId { get; set; }
        public string NazevArtiklu { get; set; }
        public string KodSAP { get; set; }
        public DateTime DatumVyroby { get; set; }
        public DateTime DatumDodani { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int BaseType { get; set; }
        public int BaseNum { get; set; }
        public string PrjCode { get; set; }
        public string PrjName { get; set; }

        public string ZakaznikSAPKod { get; set; }
        public int Zakaznik { get; set; }
        public int ProvozId { get; set; }
        public IEnumerable<SelectListItem> Provoz { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPSerioveCislo");

        [Authorize(Roles = "Administrator,Manager")]
        public static IEnumerable<SAPSerioveCislo> LoadSCFromSAP(string SC, int Artikl)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;

            List<SAPSerioveCislo> SAPSCList = new List<SAPSerioveCislo>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT t2.id as 'ArticlId', t2.KodSAP, t2.Nazev, ");
            sql.Append(" T0.[IntrSerial] as 'SerioveCislo' , t0.InDate as 'Vyrobeno', ");
            sql.Append(" t1.docdate as 'Dodano' ");
            sql.Append("  ,t3.CardCode, t3.CardName, t1.BaseType, t1.BaseNum, t6.PrjCode, t6.PrjName ");
            sql.Append(" FROM OSRI  T0 ");
            sql.Append(" INNER JOIN SRI1 T1 ON T0.ItemCode = T1.ItemCode and T0.SysSerial = T1.SysSerial");
            sql.Append($" left join [{RS_dtb}].[dbo].[Artikl] t2 on t1.ItemCode COLLATE DATABASE_DEFAULT = t2.KodSAP COLLATE DATABASE_DEFAULT");
            sql.Append(" inner join ocrd t3 on t3.CardCode = t1.CardCode");
            sql.Append(" left join ODLN t4 on t4.ObjType = t1.BaseType and t4.DocEntry = t1.BaseEntry ");
            sql.Append(" left join OINV t5 on t5.ObjType = t1.BaseType and t5.DocEntry = t1.BaseEntry ");
            sql.Append(" left join OPRJ t6 on (t4.Project = t6.PrjCode) or (t5.Project = t6.PrjCode)");
            sql.Append(" where t1.Direction = 1 and t1.CardCode is not null and ");
            sql.Append($" ((select count (*) from [{RS_dtb}].[dbo].[Artikl] where KodSAP COLLATE DATABASE_DEFAULT = t1.ItemCode) > 0)");
            sql.Append($" and T0.[IntrSerial] = '{SC}'");
            //sql = sql + @" group by T0.[IntrSerial], t2.id, t2.kodSAp, t2.nazev";
            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandText = sql.ToString()
            };
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
                    catch (Exception ex)
                    { 
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.ArticlId = dr.GetInt32(dr.GetOrdinal("ArticlId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.NazevArtiklu = dr.GetString(dr.GetOrdinal("Nazev"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.DatumDodani = dr.GetDateTime(dr.GetOrdinal("Dodano"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sapitem.DatumVyroby = dr.GetDateTime(dr.GetOrdinal("Vyrobeno"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sapitem.KodSAP = dr.GetString(dr.GetOrdinal("KodSAP"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.CardName = dr.GetString(dr.GetOrdinal("CardName"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.BaseType = dr.GetInt32(dr.GetOrdinal("BaseType"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.BaseNum = dr.GetInt32(dr.GetOrdinal("BaseNum"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sapitem.PrjCode = dr.GetString(dr.GetOrdinal("PrjCode"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                     }
                    try
                    {
                        sapitem.PrjName = dr.GetString(dr.GetOrdinal("PrjName"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }

                    using (var dbCtx = new Model1Container())
                    {
                        sapitem.ArticlId = dbCtx.Artikl.Where(a => a.KodSAP == sapitem.KodSAP).Select(a => a.Id).FirstOrDefault();
                    }
                    SAPSCList.Add(sapitem);
                }
            }
            cnn.Close();

            return SAPSCList;
        }

        [Authorize(Roles = "Administrator,Manager")]
        public static List<SAPSerioveCislo> SAPSCList(string OPSAPkod, int Zakaznik)
        {
            List<SAPSerioveCislo> SAPSCList = new List<SAPSerioveCislo>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" SELECT t2.id as 'ArticlId', t2.KodSAP, t2.Nazev, T0.[IntrSerial] as 'SerioveCislo' ,MIN(t0.InDate) as 'Vyrobeno', MAX(t1.docdate) as 'Dodano' FROM OSRI  T0 INNER JOIN SRI1 T1 ON T0.ItemCode = T1.ItemCode and T0.SysSerial = T1.SysSerial");
            sql.Append($" left join[{RS_dtb}].[dbo].[Artikl] t2 on t1.ItemCode COLLATE DATABASE_DEFAULT = t2.KodSAP COLLATE DATABASE_DEFAULT");
            sql.Append(" where t1.Direction = 1 and");
            sql.Append($" ((select count (*) from [{RS_dtb}].[dbo].[Artikl] where KodSAP COLLATE DATABASE_DEFAULT = t1.ItemCode) > 0)");
            sql.Append($" and((select count (*) from [{RS_dtb}].[dbo].[Zakaznik] where KodSAP COLLATE DATABASE_DEFAULT = t1.CardCode) > 0)");
            sql.Append($" and T1.CardCode = '{OPSAPkod}'");
            sql.Append(" group by T0.[IntrSerial], t2.id, t2.kodSAp, t2.nazev");

            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandText = sql.ToString()
            };
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
                    catch (Exception ex) {  log.Debug ("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.ArticlId = dr.GetInt32(dr.GetOrdinal("ArticlId"));
                    }
                    catch (Exception ex) {  log.Debug ("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.NazevArtiklu = dr.GetString(dr.GetOrdinal("Nazev"));
                    }
                    catch (Exception ex) {  log.Debug ("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.DatumDodani = dr.GetDateTime(dr.GetOrdinal("Dodano"));
                    }
                    catch (Exception ex) {  log.Debug ("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sapitem.DatumVyroby = dr.GetDateTime(dr.GetOrdinal("Vyrobeno"));
                    }
                    catch (Exception ex) {  log.Debug ("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapitem.KodSAP = dr.GetString(dr.GetOrdinal("KodSAP"));
                    }
                    catch (Exception ex) {  log.Debug ("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }

                    sapitem.Zakaznik = Zakaznik;
                    sapitem.ZakaznikSAPKod = OPSAPkod;
                    using (var dbCtx = new Model1Container())
                    {
                        sapitem.Provoz = new SelectList(dbCtx.Provoz.Where(m => m.ZakaznikId == Zakaznik), "Id", "NazevProvozu");
                    }
                    SAPSCList.Add(sapitem);
                }
            }
            cnn.Close();
            return SAPSCList;

        }
    }
}