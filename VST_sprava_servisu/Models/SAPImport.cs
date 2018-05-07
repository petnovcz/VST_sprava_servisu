using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Status_descript { get; set; }
        public int EmpNo { get; set; }
        public string EmpName { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public string Series { get; set; }
        public int Territory { get; set; }
        public string TeritoryName { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocCurr { get; set; }
        public ORDRforProject ordrforproject { get; set; }
        public OINVforProject oinvforproject
        {
            get; set;
        }
        public OPCHforProject opchforproject
        {
            get; set;
        }


        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_PlaRev { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_PlaRevFC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_PlaRevSC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_ActRev { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_ActRevFC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_ActRevSC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_PlaExp { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_PlaExpFC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_PlaExpSC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_ActExp { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_ActExpFC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal U_ActExpSC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal Zisk { get { var x = U_ActRev - U_ActExp; return x; } }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal ZiskPl { get { var x = U_PlaRev - U_PlaExp; return x; } }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal ZiskFC { get { var x = U_ActRevFC - U_ActExpFC; return x; } }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal ZiskPlFC { get { var x = U_PlaRevFC - U_PlaExpFC; return x; } }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal ZiskSC { get { var x = U_ActRevSC - U_ActExpSC; return x; } }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal ZiskPlSC { get { var x = U_PlaRevSC - U_PlaExpSC; return x; } }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal ZiskPrc {
            get
            {
                decimal x = 0;
                if (U_ActRev != 0) { x = Zisk / U_ActRev * 100; } else { x = 0; }
               

                return x;
            }
        }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal ZiskPlPrc
        {
            get
            {
                decimal x = 0;
                if (U_PlaRev != 0) { x = ZiskPl / U_PlaRev * 100; } else { x = 0; }
                return x;
            }
        }

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
            sql.Append(" and U_Status not in ('7', '8', '2') or t0.Code = 'RP00078'");
            
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
                        sapItem.Status = dr.GetString(dr.GetOrdinal("Name"));
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