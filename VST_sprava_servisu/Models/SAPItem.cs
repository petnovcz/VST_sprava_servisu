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

            sql.Append(" select ItemCode, ItemName, t0.ItmsGrpCod, t1.ItmsGrpNam from oitm t0 left join OITB t1 on t0.ItmsGrpCod = t1.ItmsGrpCod  where /*ManSerNum = 'Y'*/");
            sql.Append($" /*and*/ ItemCode = '{ItemCode}' ");

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
                        sapItem.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                    }
                    try
                    {
                        sapItem.ItemName = dr.GetString(dr.GetOrdinal("ItemName"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapItem.ItmsGrpNam = dr.GetString(dr.GetOrdinal("ItmsGrpNam"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapItem.ItmsGrpCod = Int32.Parse(dr.GetString(dr.GetOrdinal("ItmsGrpCod")));
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

            List<SAPItem> SAPItemsList = new List<SAPItem>();
            StringBuilder sql = new StringBuilder();
            sql.Append("select ItemCode, ItemName, t1.ItmsGrpCod, t1.ItmsGrpNam from oitm t0 left join OITB t1 on t0.ItmsGrpCod = t1.ItmsGrpCod  where /*ManSerNum = 'Y'*/");
            sql.Append("/*and*/ ((select count(*) from [Servis].[dbo].[Artikl] where KodSAP COLLATE DATABASE_DEFAULT = ItemCode COLLATE DATABASE_DEFAULT) = 0)");

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
                        sapitem.ItmsGrpCod = dr.GetInt32(dr.GetOrdinal("ItmsGrpCod"));
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