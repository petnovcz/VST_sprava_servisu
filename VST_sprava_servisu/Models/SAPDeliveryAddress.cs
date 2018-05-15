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
    public partial class SAPDeliveryAddress
    {
        [Key]
        public string Address { get; set; }
        public string CardCode { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPDeliveryAddress");

        //SAP ADRESY - IMPORTOVÁNY JAKO PROVOZY
        [Authorize(Roles = "Administrator,Manager")]
        public static List<SAPDeliveryAddress> LoadSAPDeliveryAddresses(string CardCode, int Zakaznik)
        {
            List<SAPDeliveryAddress> SAPDAList = new List<SAPDeliveryAddress>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" Select Address, CardCode, Street, ZipCode, City, Country from CRD1");
            sql.Append($" where CardCode = '{CardCode}' and AdresType = 'S' and ");
            sql.Append($" (select count(*) from [{RS_dtb}].[dbo].[Provoz] X where X.ZakaznikId = '{Zakaznik}' and X.SAPAddress COLLATE DATABASE_DEFAULT = Address COLLATE DATABASE_DEFAULT) = 0 ");

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
                    SAPDeliveryAddress sapda = new SAPDeliveryAddress();
                    if (dr.GetString(dr.GetOrdinal("CardCode")) != null)
                    {
                        sapda.Address = dr.GetString(dr.GetOrdinal("Address"));
                    }
                    try
                    {
                        sapda.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sapda.Street = dr.GetString(dr.GetOrdinal("Street"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sapda.ZipCode = dr.GetString(dr.GetOrdinal("ZipCode"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapda.City = dr.GetString(dr.GetOrdinal("City"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        sapda.Country = dr.GetString(dr.GetOrdinal("Country"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }


                    SAPDAList.Add(sapda);
                }
            }
            cnn.Close();

            return SAPDAList;
        }

    }
}