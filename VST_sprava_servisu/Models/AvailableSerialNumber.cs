using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class AvailableSN
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("AvailableSN");
        private string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
        private string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
        private string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;

        public string ItemCode { get; set; }
        public int SysSerial { get; set; }
        public string SuppSerial { get; set; }
        public string IntrSerial { get; set; }
        public string WhsCode { get; set; }
        public decimal Quantity { get; set; }
        public int Status { get; set; }       
    }
    public partial class AvailableSNList
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("AvailableSNList");

        public int SZId { get; set; }
        public int SZPrvekId { get; set; }
        public string ArtiklKusovnikSAPKod { get; set; }

        public virtual List<AvailableSN> AvailableSN {
        get
            {
                List<AvailableSN> sn = new List<AvailableSN>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select t0.ItemCode, t0.SysSerial, t0.SuppSerial, t0.IntrSerial, t0.WhsCode, t0.Quantity, t0.Status");
                sql.Append($"  from OSRI t0 INNER JOIN SRI1 T1 ON T0.ItemCode = T1.ItemCode and T0.SysSerial = T1.SysSerial");
                
                sql.Append($"  where t0.Status = 0 and t1.ItemCode = '{ArtiklKusovnikSAPKod}' and t0.WhsCode = 'Servis'");
                sql.Append($"  group by t0.ItemCode, t0.SysSerial, t0.SuppSerial, t0.IntrSerial, t0.WhsCode, t0.Quantity, t0.Status");
 
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
                        AvailableSN availablesn = new AvailableSN();
                        try
                        {
                            availablesn.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            availablesn.SysSerial = dr.GetInt32(dr.GetOrdinal("SysSerial"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            availablesn.SuppSerial = dr.GetString(dr.GetOrdinal("SuppSerial")); ;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            availablesn.IntrSerial = dr.GetString(dr.GetOrdinal("IntrSerial")); ;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            availablesn.WhsCode = dr.GetString(dr.GetOrdinal("WhsCode")); ;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            availablesn.Quantity = dr.GetDecimal(dr.GetOrdinal("Quantity")); ;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            availablesn.Status = dr.GetInt16(dr.GetOrdinal("Status")); ;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        sn.Add(availablesn);
                    }
                }
                cnn.Close();
                return sn;

            }

        }

    }
}