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
    public partial class SAPItem
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItmsGrpNam { get; set; }
        public int ItmsGrpCod { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPItem");

        [Authorize(Roles = "Administrator,Manager")]
        public static SAPItem GetSAPItemByCode(string ItemCode)
        {
            SAPItem sapItem = new SAPItem();
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" select ItemCode, ItemName, t0.ItmsGrpCod as 'ItmsGrpCod', t1.ItmsGrpNam as 'ItmGrpNam' from oitm t0 left join OITB t1 on t0.ItmsGrpCod = t1.ItmsGrpCod  where ");
            sql.Append($" ItemCode = '{ItemCode}' ");

            log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
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

                    if (dr.GetString(dr.GetOrdinal("ItemCode")) != null)
                    {
                        sapItem.ItemCode = dr.GetString(0);
                    }
                    try
                    {
                        sapItem.ItemName = dr.GetString(1);
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapItem.ItmsGrpNam = dr.GetString(3);
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        
                        int codeint = dr.GetInt16(2);
                        sapItem.ItmsGrpCod = codeint;
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                }
            }
            cnn.Close();
            return sapItem;
        }

        public static List<SAPItem> SAPItemList()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;

            List<SAPItem> SAPItemsList = new List<SAPItem>();
            StringBuilder sql = new StringBuilder();
            sql.Append("select ItemCode, ItemName, t1.ItmsGrpCod, t1.ItmsGrpNam from oitm t0 left join OITB t1 on t0.ItmsGrpCod = t1.ItmsGrpCod  where /*ManSerNum = 'Y'*/");
            sql.Append($"/*and*/ ((select count(*) from [{RS_dtb}].[dbo].[Artikl] where KodSAP COLLATE DATABASE_DEFAULT = ItemCode COLLATE DATABASE_DEFAULT) = 0)");

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
                    SAPItem sapitem = new SAPItem();
                    try
                    {
                        sapitem.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapitem.ItemName = dr.GetString(dr.GetOrdinal("ItemName"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapitem.ItmsGrpNam = dr.GetString(dr.GetOrdinal("ItmsGrpNam"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapitem.ItmsGrpCod = dr.GetInt16(dr.GetOrdinal("ItmsGrpCod"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    SAPItemsList.Add(sapitem);
                }
            }
            cnn.Close();
            return SAPItemsList;
        }
    }
}