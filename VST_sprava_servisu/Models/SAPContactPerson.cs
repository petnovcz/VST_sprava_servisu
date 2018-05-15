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
    public partial class SAPContactPerson
    {
        [Key]
        public int CntctCode { get; set; }
        public string CardCode { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Tel1 { get; set; }
        public string Cellolar { get; set; }
        public string E_MaiL { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPContactPerson");
        


        // KONTAKTNÍ OSOBY
        [Authorize(Roles = "Administrator,Manager")]
        public static List<SAPContactPerson> SAPContactPersonList(string SAPOP)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;
            List<SAPContactPerson> SAPCP = new List<SAPContactPerson>();

            StringBuilder sql = new StringBuilder();

            sql.Append(" Select CntctCode, CardCode, Name, Position, Tel1, Cellolar, E_MailL from OCPR");
            sql.Append($" Where CardCode = '{SAPOP}' and");
            sql.Append($" (select count(*) from [{RS_dtb}].[dbo].[KontakniOsoba] where SapId = CntctCode ) = 0");


            //(select COUNT(*) from[Servis].[dbo].[Zakaznik] Z where Z.KodSAP COLLATE DATABASE_DEFAULT = CardCode COLLATE DATABASE_DEFAULT) = 0)";
            SqlConnection cnn = new SqlConnection(connectionString);
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
                    SAPContactPerson sapcp = new SAPContactPerson();
                    if (dr.GetString(dr.GetOrdinal("CardCode")) != null)
                    {
                        sapcp.CntctCode = dr.GetInt32(dr.GetOrdinal("CntctCode"));
                    }
                    try
                    {
                        sapcp.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapcp.Name = dr.GetString(dr.GetOrdinal("Name"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapcp.Position = dr.GetString(dr.GetOrdinal("Position"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sapcp.Tel1 = dr.GetString(dr.GetOrdinal("Tel1"));
                    }
                    catch (Exception ex) {// log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapcp.Cellolar = dr.GetString(dr.GetOrdinal("Cellolar"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapcp.E_MaiL = dr.GetString(dr.GetOrdinal("E_MailL"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }

                    SAPCP.Add(sapcp);
                }
            }
            cnn.Close();


            return SAPCP;
        }


    }
}