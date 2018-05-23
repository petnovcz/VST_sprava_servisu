using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace VST_sprava_servisu
{
    
    public class Statistiky
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Statistiky");






    }
    public partial class CelkovyManazerskyPrehled
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("CelkovyManazerskyPrehled");
        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum od")]
        public DateTime DatumOd { get; set; }
        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum do")]
        public DateTime DatumDo { get; set; }
        public FinancniVysledkyDleZeme finVysledkyDleZeme {
            get
            {
                FinancniVysledkyDleZeme fvdlz = new FinancniVysledkyDleZeme();
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append("SELECT SUM(X1.Vynosy) as 'Vynosy', SUM(X1.Naklady) as 'Naklady', SUM(X1.Vynosy) - SUM(X1.Naklady) as 'Vysledek'  " +
                    " FROM ( " +
                    " SELECT  T0.Account 'Ucet', " +
                    " CASE WHEN(T0.Account like '6%') and(T0.Account <> '663100') then isnull((T0.Credit * (isnull(T4.[PrcAmount], 100) / 100)) - (T0.Debit * (isnull(T4.[PrcAmount], 100) / 100)), 0) ELSE 0 END 'Vynosy'," +
                    " CASE WHEN(T0.Account like '5%') and(T0.Account not like('56%')) then isnull((T0.Debit * (isnull(T4.[PrcAmount], 100) / 100)) - (T0.Credit * (isnull(T4.[PrcAmount], 100) / 100)), 0) ELSE 0 END 'Naklady'" +
                    " FROM JDT1 T0" +
                    " INNER JOIN OACT T2 ON T0.Account = T2.AcctCode" +
                    " LEFT OUTER JOIN OOCR T3 ON T0.ProfitCode = T3.OcrCode" +
                    " LEFT OUTER JOIN OCR1 T4 ON T3.OcrCode = T4.OcrCode AND(T4.ValidFrom =" +
                    " (select TOP 1 A.ValidFrom from OCR1 A where A.OcrCode = T4.OcrCode AND A.ValidFrom <= T0.Refdate order by A.ValidFrom desc))" +
                    " LEFT OUTER JOIN OPRC T1 ON T4.PrcCode = T1.PrcCode" +
                    " WHERE(T0.Account like '6%' OR T0.Account like '5%')");
                sql.Append($" AND T0.[RefDate] >= '{DatumOd.ToString("yyyyMMdd")}'");
                sql.Append($" AND T0.[RefDate] <= '{DatumDo.ToString("yyyyMMdd")}'");
                sql.Append(" AND T0.TransType NOT IN ('-2', '-3') ) X1 INNER JOIN OACT X2 ON X1.Ucet = X2.AcctCode for browse" );

                //log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
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
                        
                        try
                        {
                            fvdlz.Vynosy = dr.GetDecimal(dr.GetOrdinal("Vynosy"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            fvdlz.Naklady = dr.GetDecimal(dr.GetOrdinal("Naklady"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fvdlz.Zisk = dr.GetDecimal(dr.GetOrdinal("Vysledek"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }

                    }
                }
                cnn.Close();
           


                return fvdlz;
            }
    
        }
        public StatistikaProjektu statistikaProjektu {
            get
            {
                StatistikaProjektu sp = new StatistikaProjektu();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select ( SELECT count(T0.[Code]) FROM[dbo].[@VCZ_CT_PRJ] T0 where T0.Code not like 'RP%' and T0.U_Type not in ('R', 'P', 'S') and (select top 1 U_Status from[dbo].[@AVCZ_CT_PRJ]  T1 where t1.Code = T0.Code order by LogInst desc) in ('3a', '3b', '3c', '3d', '3e', '3f', '3g', '3h', '4', '5', '5b') ) as 'OtevreneProjekty', ");
                sql.Append($" (SELECT count(T0.[Code]) FROM [dbo].[@VCZ_CT_PRJ] T0 where T0.Code not like 'RP%' and T0.U_Type not in ('R', 'P', 'S') and(select top 1 U_Status from[dbo].[@AVCZ_CT_PRJ]  T1 where t1.Code = T0.Code and  T1.UpdateDate <= '{DatumDo.ToString("yyyyMMdd")}' order by LogInst desc) in ('3a', '3b', '3c', '3d', '3e', '3f', '3g', '3h', '4', '5', '5b') and coalesce(T0.U_EndDate, getdate()) < GETDATE() and((select COUNT(tz.docentry) from ORDR tz where tz.Project = t0.[code] and tz.DocStatus = 'O') > 0 or (select COUNT(tz.docentry) from ODLN tz where tz.project = t0.[Code] and tz.docdate > coalesce(T0.U_EndDate, getdate())) > 0)) as 'OtevreneProjektyVProdleni',");
                sql.Append($" ( SELECT count(T0.[Code]) FROM [dbo].[@VCZ_CT_PRJ] T0 where T0.Code not like 'RP%' and T0.U_Type not in ('R', 'P', 'S') and (select top 1 U_Status from[dbo].[@AVCZ_CT_PRJ]  T1 where t1.Code = T0.Code and T1.U_ActEndDt >= '{DatumOd.ToString("yyyyMMdd")}' and T1.U_ActEndDt <= '{DatumDo.ToString("yyyyMMdd")}' order by LogInst desc)   = '7') as 'UzavreneProjekty',");
                sql.Append($" (SELECT count(T0.[Code]) FROM [dbo].[@VCZ_CT_PRJ] T0 where T0.Code not like 'RP%' and T0.U_Type not in ('R', 'P', 'S') and (select top 1 U_Status from[dbo].[@AVCZ_CT_PRJ]  T1 where t1.Code = T0.Code and T1.U_EndDate >= '{DatumOd.ToString("yyyyMMdd")}' and T1.U_EndDate <= '{DatumDo.ToString("yyyyMMdd")}' order by LogInst desc) = '7' and(datediff(dd, coalesce(T0.U_EndDate, '20120101'), coalesce((select max(docdate) from ODLN tx where tx.project = t0.code ), getdate())) > 0)) as 'UzavreneProjektyVProdleni'");

                //log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
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

                        try
                        {
                            sp.OtevreneProjekty = dr.GetInt32(dr.GetOrdinal("OtevreneProjekty"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            sp.OtevreneProjektyVProdleni = dr.GetInt32(dr.GetOrdinal("OtevreneProjektyVProdleni"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            sp.UzavreneProjekty = dr.GetInt32(dr.GetOrdinal("UzavreneProjekty"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            sp.UzavreneProjektyVProdleni = dr.GetInt32(dr.GetOrdinal("UzavreneProjektyVProdleni"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }

                    }
                }
                cnn.Close();

                return sp;
            }


        }
        public FakturaceProjektu fakturaceProjektu
        {
            get
            {
                FakturaceProjektu fp = new FakturaceProjektu();
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select x.dtb, x.FakturyVydane, x.FakturyPrijate, x.VydejPokladna, coalesce(x.VydejMzdy,0) as 'VydejMzdy', (x.FakturyVydane - x.FakturyPrijate - x.VydejPokladna - coalesce(x.VydejMzdy,0)) as 'HrubyZisk', x.FakturyPrijate_RP, x.FakturyPrijate_Obchod, x.VydejPokladna_RP, x.VydejPokladna_Obchod from");
                sql.Append($" ( select 'CZ' 'dtb',");
                sql.Append($" ( select sum(X.X) from ( select 'CZ' as 'dtb', 'INV' as 'doklad', T0.DocDate, T0.CardCode, T0.CardName, T0.DocNum, (T0.DocTotal - coalesce(T0.VatSum, 0) - coalesce(T0.RoundDif, 0) + coalesce((select sum(DrawnSum) from INV9 where DocEntry = T0.docentry), 0)) 'X'");
                sql.Append($" from OINV T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}'");
                sql.Append($" union all select 'CZ' as 'dtb','0RIN' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, -(T0.DocTotal - T0.VatSum - T0.RoundDif) 'X'");
                sql.Append($" from ORIN T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}'");
                sql.Append($" union all select 'CZ' as 'dtb', 'CSI' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (T0.DocTotal - T0.VatSum - T0.RoundDif) 'X'");
                sql.Append($" from OCSI T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}'");
                sql.Append($" union all select 'CZ' as 'dtb','CSV' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (T0.DocTotal - T0.VatSum - T0.RoundDif) 'X'");
                sql.Append($" from OCSV T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' ) X ) 'FakturyVydane'");
                sql.Append($" , ( select sum(X.X) from ( select 'CZ' as 'dtb', 'PCH' as 'doklad', T0.DocDate, T0.CardCode, T0.CardName, T0.DocNum, (T0.DocTotal - coalesce(T0.VatSum, 0) - coalesce(T0.RoundDif, 0) + coalesce((select sum(DrawnSum) from pch9 where DocEntry = T0.docentry), 0)) 'X'");
                sql.Append($" from OPCH T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' union all select 'CZ' as 'dtb','RPC' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, -(T0.DocTotal - T0.VatSum - T0.RoundDif) 'X'");
                sql.Append($" from ORPC T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' ");
                sql.Append($" union all select 'CZ' as 'dtb', 'CPI' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (T0.DocTotal - T0.VatSum - T0.RoundDif) 'X' from OCPI T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' union all");
                sql.Append($" select 'CZ' as 'dtb','CPV' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (T0.DocTotal - T0.VatSum - T0.RoundDif) 'X' from OCPV T0 where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' ) X ) 'FakturyPrijate'");


                sql.Append($" ,(select SUM(credit) from JDT1 T1 inner join OJDT T0 on t0.TransId = t1.TransId where Account like '211%' and credit > 0");
                sql.Append($" and T0.RefDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.RefDate <= '{DatumDo.ToString("yyyyMMdd")}') 'VydejPokladna'");

                sql.Append($" ,(select SUM(coalesce(T0.U_Castka, 0)) from [@VST_MZDY] t0 where T0.U_Datum >= '{DatumOd.ToString("yyyyMMdd")}' and T0.U_Datum <= '{DatumDo.ToString("yyyyMMdd")}') 'VydejMzdy'");
                
                
                sql.Append($" , ( select sum(X.X) from ( select 'CZ' as 'dtb', 'PCH' as 'doklad', T0.DocDate, T0.CardCode, T0.CardName, T0.DocNum, (T1.LineTotal) 'X', coalesce(left(t0.Project, 2), (select left(max(Project), 2) from PCH1 tx where tx.docentry = t0.docentry)) as 'Project'");
                sql.Append($" from OPCH T0 inner join PCH1 T1 on t0.DocEntry = T1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' and(left(t1.Project, 2) = 'RP' and t1.project not in ('RP00006', 'RP00078'))");
                sql.Append($" union all select 'CZ' as 'dtb','RPC' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, -(T1.Linetotal) 'X', coalesce(left(t0.Project, 2), (select left(max(Project), 2) from rpc1 tx where tx.docentry = t0.docentry)) from ORPC T0 inner join RPC1 T1 on t0.DocEntry = t1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' and(left(t1.Project, 2) = 'RP' and t1.project not in ('RP00006', 'RP00078'))");
                sql.Append($" union all select 'CZ' as 'dtb', 'CPI' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (T1.LineTotal) 'X' ,coalesce(left(t0.Project, 2), (select left(max(Project), 2) from cpi1 tx where tx.docentry = t0.docentry))");
                sql.Append($" from OCPI T0 inner join CPI1 t1 on t0.DocEntry = t1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' and(left(t1.Project, 2) = 'RP' and t1.project not in ('RP00006', 'RP00078'))");
                sql.Append($" union all select 'CZ' as 'dtb','CPV' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (t1.linetotal) 'X' ,coalesce(left(t0.Project, 2), (select left(max(Project), 2) from cpv1 tx where tx.docentry = t0.docentry)) from OCPV T0 inner join CPV1 t1 on t0.DocEntry = t1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' and(left(t1.Project, 2) = 'RP' and t1.project not in ('RP00006', 'RP00078')) ) X ) 'FakturyPrijate_RP'");


                sql.Append($" , ( select sum(X.X) from ( select 'CZ' as 'dtb', 'PCH' as 'doklad', T0.DocDate, T0.CardCode, T0.CardName, T0.DocNum, (t1.LineTotal) 'X' , coalesce(left(t0.Project, 2), (select left(max(Project), 2) from PCH1 tx where tx.docentry = t0.docentry)) as 'Project' from OPCH T0 inner join PCH1 t1 on t0.DocEntry = t1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}'  and(left(t1.Project, 2) <> 'RP' or t1.project in ('RP00006', 'RP00078')) ");
                sql.Append($" union all select 'CZ' as 'dtb','RPC' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, -(t1.LineTotal) 'X', coalesce(left(t0.Project, 2), (select left(max(Project), 2) from rpc1 tx where tx.docentry = t0.docentry)) from ORPC T0 inner join RPC1 t1 on t0.DocEntry = t1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' and(left(t1.Project, 2) <> 'RP' or t1.project in ('RP00006', 'RP00078')) ");
                sql.Append($" union all select 'CZ' as 'dtb', 'CPI' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (t1.linetotal) 'X' ,coalesce(left(t0.Project, 2), (select left(max(Project), 2) from cpi1 tx where tx.docentry = t0.docentry)) from OCPI T0 inner join CPI1 t1 on t0.DocEntry = t1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' and(left(t1.Project, 2) <> 'RP' or t1.project in ('RP00006', 'RP00078')) ");
                sql.Append($" union all select 'CZ' as 'dtb','CPV' as 'doklad', T0.DOcdate, T0.CardCode, T0.CardName, T0.DocNum, (T0.DocTotal - T0.VatSum - T0.RoundDif) 'X' ,coalesce(left(t0.Project, 2), (select left(max(Project), 2) from cpv1 tx where tx.docentry = t0.docentry)) from OCPV T0 inner join CPV1 t1 on t0.DocEntry = t1.DocEntry where T0.VatDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.VatDate <= '{DatumDo.ToString("yyyyMMdd")}' and(left(t1.Project, 2) <> 'RP' or t1.project in ('RP00006', 'RP00078')) ) X ) 'FakturyPrijate_Obchod'");
                sql.Append($" , (select SUM(credit) from JDT1 T1 inner join OJDT T0 on t0.TransId = t1.TransId where Account like '211%' and credit > 0 and T0.RefDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.RefDate <= '{DatumDo.ToString("yyyyMMdd")}'  and LEFT(T1.Project,2) = 'RP') as 'VydejPokladna_RP' ,");
                sql.Append($" (select SUM(credit) from JDT1 T1 inner join OJDT T0 on t0.TransId = t1.TransId where Account like '211%' and credit > 0  and T0.RefDate >= '{DatumOd.ToString("yyyyMMdd")}' and T0.RefDate <= '{DatumDo.ToString("yyyyMMdd")}' and LEFT(T1.Project,2) <> 'RP') as 'VydejPokladna_Obchod' ) X");


                //log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
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

                        try
                        {
                            fp.FakturyVydane = dr.GetDecimal(dr.GetOrdinal("FakturyVydane"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            fp.FakturyPrijate = dr.GetDecimal(dr.GetOrdinal("FakturyPrijate"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fp.FakturyPrijate_Obchod = dr.GetDecimal(dr.GetOrdinal("FakturyPrijate_Obchod"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fp.FakturyPrijate_RP = dr.GetDecimal(dr.GetOrdinal("FakturyPrijate_RP"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fp.VydejMzdy = dr.GetDecimal(dr.GetOrdinal("VydejMzdy"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fp.VydejPokladna = dr.GetDecimal(dr.GetOrdinal("VydejPokladna"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fp.VydejPokladna_Obchod = dr.GetDecimal(dr.GetOrdinal("VydejPokladna_Obchod"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fp.VydejPokladna_RP = dr.GetDecimal(dr.GetOrdinal("VydejPokladna_RP"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            fp.HrubyZisk = dr.GetDecimal(dr.GetOrdinal("HrubyZisk"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }

                    }
                }
                cnn.Close();


                return fp;
            }


        }



        public class FinancniVysledkyDleZeme
        {
            public decimal Vynosy { get; set; }
            public decimal Naklady { get; set; }
            public decimal Zisk { get; set; }
        }
        public class StatistikaProjektu
        {
            public int OtevreneProjekty { get; set; }
            public int OtevreneProjektyVProdleni { get; set; }
            public int UzavreneProjekty { get; set; }
            public int UzavreneProjektyVProdleni { get; set; }


        }
        public class FakturaceProjektu
        {
            public decimal FakturyVydane { get; set; }
            public decimal FakturyPrijate_Obchod { get; set; }
            public decimal FakturyPrijate_RP { get; set; }
            public decimal FakturyPrijate { get; set; }
            public decimal VydejMzdy { get; set; }
            public decimal HrubyZisk { get; set; }
            public decimal VydejPokladna_Obchod { get; set; }
            public decimal VydejPokladna_RP { get; set; }
            public decimal VydejPokladna { get; set; }


        }
    }
    

}