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

    public partial class BezRevize
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("BezRevize");

        [Display(Name="Rok")]
        public int Rok { get; set; }
        [Display(Name = "Skupina")]
        public int Skupina { get; set; }
        [Display(Name = "Hledání")]
        public string Search { get; set; }
        public List<ZakaznickySeznam> ZakaznickySeznam { get; set; }

        /// <summary>
        /// Vyhledá zákazníky, provozy a umisteni kde pro zadany rok neni vygenerovaná revize (omezení na skupinu zákazníků (skupinu regionů))
        /// </summary>
        /// <param name="Rok"></param>
        /// <param name="Skupina"></param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Administrator,Manager")]
        internal protected static List<ZakaznickySeznam> GetCustomerListWithoutRevision(int Rok, int Skupina, string Search)
        {
            List<ZakaznickySeznam> list = new List<ZakaznickySeznam>();

            //načtení defaultního connection stringu SQL SERVIS
            string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            // definování SQL querry
            StringBuilder sql = new StringBuilder();

            sql.Append(" select t0.ID as 'ZakaznikId', T0.NazevZakaznika as 'Zakaznik',");
            sql.Append(" T1.ID as 'ProvozId', T1.NazevProvozu as 'Provoz',");
            sql.Append(" T2.Id as 'UmisteniId', T2.NazevUmisteni as 'NazevUmisteni'");
            sql.Append(" from Zakaznik t0 inner join Provoz t1 on t0.id = t1.zakaznikid left join Umisteni t2 on t1.id = t2.provozid and t2.samostatnarevize = 1 left join Region t3 on t0.RegionId = t3.Id");
            sql.Append($" where (t3.Skupina = '{Skupina}' or 0 = '{Skupina}') and (t0.NazevZakaznika like '%{Search}%' or '{Search}' = '')");
            sql.Append($" and (select COUNT(*) from Revize where provozid = t1.id and (UmisteniId = t2.id or UmisteniID is null) and rok = '{Rok}') = 0");

            log.Debug($"GetCustomerListWithoutRevision pro Rok: {Rok}, Skupina: {Skupina}, Search: {Search}");
            log.Debug(sql.ToString());

            SqlConnection cnn = new SqlConnection(con);
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
                    ZakaznickySeznam item = new ZakaznickySeznam();
                    try
                    {
                        item.ZakaznikId = dr.GetInt32(dr.GetOrdinal("ZakaznikId"));
                    }
                    catch(Exception ex)
                    {
                        log.Debug("GetCustomerListWithoutRevision - načtení ZakaznikId: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.Zakaznik = dr.GetString(dr.GetOrdinal("Zakaznik"));
                    }
                    catch(Exception ex)
                    {
                        log.Debug("GetCustomerListWithoutRevision - načtení Zakaznik: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.ProvozId = dr.GetInt32(dr.GetOrdinal("ProvozId"));
                    }
                    catch(Exception ex)
                    {
                        log.Debug("GetCustomerListWithoutRevision - načtení ProvozId: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.Provoz = dr.GetString(dr.GetOrdinal("Provoz"));
                    }
                    catch(Exception ex)
                    {
                        log.Debug("GetCustomerListWithoutRevision - načtení Provoz: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.UmisteniId = dr.GetInt32(dr.GetOrdinal("UmisteniId"));
                    }
                    catch(Exception ex)
                    {
                        log.Debug("GetCustomerListWithoutRevision - načtení UmisteniId: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.NazevUmisteni = dr.GetString(dr.GetOrdinal("NazevUmisteni"));
                    }
                    catch(Exception ex)
                    {
                        log.Debug("GetCustomerListWithoutRevision - načtení NazevUmisteni: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    list.Add(item);
                }
            }
            cnn.Close();
            return list;
        }
    }
}