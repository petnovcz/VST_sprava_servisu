using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace VST_sprava_servisu
{
    public class SPZ
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SPZ");

        public List<SPZ_Line> SPZ_NotImported { get; set; }
        public int ImportedRecords { get; set; }

        public class SPZ_Line
        {
            [DisplayFormat(DataFormatString = "{0:#.##}")]
            public DateTime U_datumOd { get; set; }
            [DisplayFormat(DataFormatString = "{0:#.##}")]
            public DateTime U_datumDo { get; set; }
            public decimal U_UjetoSlu { get; set; }
            public string U_MistoOdk { get; set; }
            public string U_MistoKam { get; set; }
            public string U_SPZ { get; set; }
            public string U_projekt { get; set; }
            public string U_ridic { get; set; }


        }


        public static int ImportSPZData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;
            int numberOfRecords = 0;

            StringBuilder sql = new StringBuilder();
            sql.Append($"  INSERT INTO {SAP_dtb}.[dbo].[@VCZ_SPZ_DATA] ([Code],[Name],[U_DatumOd],[U_DatumDo],[U_UjetoSlu],[U_MistoOdk],[U_MistoKam],[U_SPZ],[U_Stredisk],[U_Projekt],[U_Ridic],[U_ZemeOdk],[U_ZemeKam],[U_Category],[U_TypPHM],[U_Period],[U_Status])");
            sql.Append($" SELECT cast('000' as varchar) + cast((SELECT MAX( [Code]) FROM {SAP_dtb}.[dbo].[@VCZ_SPZ_DATA]) + Row_Number() Over(Order By t0.DatumOd ) as varchar)  as 'code',");
            sql.Append($" cast('000' as varchar) + cast((SELECT MAX( [Code]) FROM {SAP_dtb}.[dbo].[@VCZ_SPZ_DATA]) + Row_Number() Over(Order By t0.DatumOd ) as varchar) as 'name',");
            sql.Append(" t0.DatumOd as 'U_datumOd', t0.DatumDo as 'U_datumDo', t0.UjetoSluzebne as 'U_UjetoSlu', t0.MistoOdkud as 'U_MistoOdk', t0.MistoKam as 'U_MistoKam', t2.SPZ as 'U_SPZ', 'OB' as 'U_Stredisk', substring(t0.Poznamka,0,8) as 'U_projekt', t1.Jmeno as 'U_ridic', t3.Nazev as 'U_zemeodk', t4.Nazev as 'U_zemekam', 'kat4' as 'U_category',");
            sql.Append(" '2' as 'U_typphm', CAST(t0.Rok AS varchar(5)) + '-' + RIGHT(Replicate('0', 2) + CAST(t0.Mesic as varchar(5)), 2) as 'U_period', 'N' as 'U_status'");
            sql.Append(" FROM [spzsql].[dbo].[BeznaKniha] t0 left join [spzsql].[dbo].[Osoby] t1 on t0.LinkOsoba = t1.LinkOsoba left join [spzsql].[dbo].[auta] t2 on t0.LinkAuto = t2.LinkAuto left join [spzsql].[dbo].[zeme] t3 on t0.LinkZeme = t3.LinkZeme left join [spzsql].[dbo].[zeme] t4 on t0.LinkZeme2 = t4.LinkZeme ");
            sql.Append(" where t0.Rok >= '2018' and t1.OsCislo not in ('20','67','18') and UjetoSluzebne<> 0");
            sql.Append($"  and ( select COUNT(*) from {SAP_dtb}.[dbo].[@VCZ_SPZ_DATA] tx where tx.U_DatumOd = t0.DatumOd and tx.U_datumdo = t0.Datumdo and tx.U_ujetoslu = t0.UjetoSluzebne) = 0");
            sql.Append($" and ( select COUNT(*) from {SAP_dtb}.[dbo].[OPRJ] tx where tx.PrjCode COLLATE DATABASE_DEFAULT = substring(t0.Poznamka, 0, 8) COLLATE DATABASE_DEFAULT) > 0");
            sql.Append(" and substring(t0.Poznamka,0,8) <> ''");

            //log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandText = sql.ToString()
            };
            cnn.Open();
            try
            {
                numberOfRecords = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("SPZ import Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);

            }
                cnn.Close();
            return numberOfRecords;
        }
    

        public static List<SPZ_Line> GetNotImportableData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;

            List<SPZ_Line> item = new List<SPZ_Line>();
            
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT t0.DatumOd as 'U_datumOd', t0.DatumDo as 'U_datumDo', t0.UjetoSluzebne as 'U_UjetoSlu', t0.MistoOdkud as 'U_MistoOdk', t0.MistoKam as 'U_MistoKam', t2.SPZ as 'U_SPZ', 'OB' as 'U_Stredisk', substring(t0.Poznamka, 0, 8) as 'U_projekt', t1.Jmeno as 'U_ridic'");
            sql.Append(" FROM [spzsql].[dbo].[BeznaKniha] t0 left join [spzsql].[dbo].[Osoby] t1 on t0.LinkOsoba = t1.LinkOsoba left join [spzsql].[dbo].[auta] t2 on t0.LinkAuto = t2.LinkAuto left join[spzsql].[dbo].[zeme] t3 on t0.LinkZeme = t3.LinkZeme left join[spzsql].[dbo].[zeme] t4 on t0.LinkZeme2 = t4.LinkZeme");
            sql.Append(" where t0.Rok >= '2018' and t1.OsCislo not in ('20','67','18') and UjetoSluzebne<> 0 ");
            sql.Append($" and ( select COUNT(*) from {SAP_dtb}.[dbo].[@VCZ_SPZ_DATA] tx where tx.U_DatumOd = t0.DatumOd and tx.U_datumdo = t0.Datumdo and tx.U_ujetoslu = t0.UjetoSluzebne) = 0");
            sql.Append($" and((select COUNT(*) from {SAP_dtb}.[dbo].[@VCZ_CT_PRJ] tx where tx.Code COLLATE DATABASE_DEFAULT = substring(t0.Poznamka, 0, 8) COLLATE DATABASE_DEFAULT) = 0");
            sql.Append(" or substring(t0.Poznamka,0,8) = '')");

            // log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

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
                    SPZ_Line line = new SPZ_Line();
                    // sil.ServisniZasahId = ServisniZasahId;
                    try
                    {
                        line.U_datumOd = dr.GetDateTime(dr.GetOrdinal("U_datumOd"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        line.U_datumDo = dr.GetDateTime(dr.GetOrdinal("U_datumDo"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }

                    try
                    {
                        line.U_UjetoSlu = dr.GetDecimal(dr.GetOrdinal("U_UjetoSlu"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        line.U_MistoOdk = dr.GetString(dr.GetOrdinal("U_MistoOdk"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        line.U_MistoKam = dr.GetString(dr.GetOrdinal("U_MistoKam"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        line.U_SPZ = dr.GetString(dr.GetOrdinal("U_SPZ"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        line.U_projekt = dr.GetString(dr.GetOrdinal("U_projekt"));
                    }
                    catch (Exception ex)
                    { 
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        line.U_ridic = dr.GetString(dr.GetOrdinal("U_ridic"));
                    }
                    catch (Exception ex)
                    { 
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    item.Add(line);

                }
            }
            cnn.Close();

            return item;
        }

        public void SendEmailWithSPZNotImportableData(List<SPZ_Line> item)
        {

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.vst.cz");

                mail.From = new MailAddress("novakp@vst.cz");
                mail.To.Add("novakp@vst.cz");
                mail.Subject = "SPZ";
                StringBuilder mailbody = new StringBuilder();
                mailbody.Append("Ahoj všem,");
                mailbody.AppendLine();
                mailbody.Append("prosím o doplnění správného projhektu k cesťákům v SPZ pro jednotlivé řádky níže:");
                mailbody.AppendLine();
                mailbody.Append("Datum od \t\t Datum do \t\t Odkud \t\t Kam \t\t SPZ \t\t Projekt \t\t Řidič");
                mailbody.AppendLine();
                foreach (var line in item)
                {
                    mailbody.Append($"{line.U_datumOd.ToShortDateString()} \t\t {line.U_datumDo.ToShortDateString()} \t\t {line.U_MistoOdk} \t\t {line.U_MistoKam} \t\t {line.U_SPZ} \t\t {line.U_projekt} \t\t {line.U_ridic}");
                    mailbody.AppendLine();
                }
                mail.Body = mailbody.ToString();
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("novakp", "nAp*322#");
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);
                
            }
            catch (Exception ex)
            {
                log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
            }

        }

}
    


    
}