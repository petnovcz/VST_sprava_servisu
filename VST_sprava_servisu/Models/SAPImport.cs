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
    public partial class Projekt
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int ServisniZasahId { get; set; }


        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Projekt");

        [Authorize(Roles = "Administrator,Manager")]
        public static List<Projekt> ProjectList(string SAPKod, int ServisniZasahId)
        {
            
            
            List<Projekt> list = new List<Projekt>();

            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" select t0.Code, t0.U_Descript, t1.Name   from [@VCZ_CT_PRJ]  t0 left join[@VCZ_CT_STATUS] t1 on t0.U_Status = t1.Code");
            sql.Append($" where U_CardCode = '{SAPKod}'");
            sql.Append(" and coalesce(U_ActStart, U_StartDat) <= GETDATE() and coalesce(U_ActEndDt, U_EndDate) >= GETDATE()");
            sql.Append(" and U_Status not in ('7', '8', '2')");
            
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
                    Projekt sapItem = new Projekt();
                    sapItem.ServisniZasahId = ServisniZasahId;
                    try
                    {
                        sapItem.Code = dr.GetString(dr.GetOrdinal("Code"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        sapItem.Name = dr.GetString(dr.GetOrdinal("U_Descript"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    try
                    {
                        sapItem.Status = dr.GetString(dr.GetOrdinal("Status"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    list.Add(sapItem);
                }
            }
            cnn.Close();
            return list;
        }
    }




    public partial class SAPImport
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPImport");

    }

}