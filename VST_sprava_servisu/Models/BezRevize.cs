using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class ZakaznickySeznam
    {
        public int ZakaznikId { get; set; }
        public string Zakaznik { get; set; }
        public int ProvozId { get; set; }
        public string Provoz { get; set; }
        public int UmisteniId { get; set; }
        public string NazevUmisteni { get; set; }

    }
    public partial class BezRevize
    {
        public int Rok { get; set; }
        public int Skupina { get; set; }
        public string Search { get; set; }
        public List<ZakaznickySeznam> ZakaznickySeznam { get; set; }

        /// <summary>
        /// Vyhledá zákazníky, provozy a umisteni kde pro zadany rok neni vygenerovaná revize (omezení na skupinu zákazníků (skupinu regionů))
        /// </summary>
        /// <param name="Rok"></param>
        /// <param name="Skupina"></param>
        /// <returns></returns>
        internal protected static List<ZakaznickySeznam>  GetCustomerListWithoutRevision(int Rok, int Skupina, string Search)
        {
            List<ZakaznickySeznam> list = new List<ZakaznickySeznam>  ();
            
            //načtení defaultního connection stringu SQL SERVIS
            string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            // definování SQL querry
            string sql = @" select t0.ID as 'ZakaznikId', T0.NazevZakaznika as 'Zakaznik',";
            sql = sql + @" T1.ID as 'ProvozId', T1.NazevProvozu as 'Provoz',";
            sql = sql + @" T2.Id as 'UmisteniId', T2.NazevUmisteni as 'NazevUmisteni'";
            sql = sql + @" from Zakaznik t0 inner join Provoz t1 on t0.id = t1.zakaznikid left join Umisteni t2 on t1.id = t2.provozid and t2.samostatnarevize = 1 left join Region t3 on t0.RegionId = t3.Id";
            sql = sql + @" where (t3.Skupina = '" + Skupina + "' or 0 = '"+ Skupina + "') and (t0.NazevZakaznika like '%" + Search +"%' or '"+Search+"' = '')";
            sql = sql + @" and (select COUNT(*) from Revize where provozid = t1.id and (UmisteniId = t2.id or UmisteniID is null) and rok = '"+Rok+"') = 0";
            
            SqlConnection cnn = new SqlConnection(con);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql;
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
                    catch { }
                    try
                    {
                        item.Zakaznik = dr.GetString(dr.GetOrdinal("Zakaznik"));
                    }
                    catch { }
                    try
                    {
                        item.ProvozId = dr.GetInt32(dr.GetOrdinal("ProvozId"));
                    }
                    catch { }
                    try
                    {
                        item.Provoz = dr.GetString(dr.GetOrdinal("Provoz"));
                    }
                    catch { }
                    try
                    {
                        item.UmisteniId = dr.GetInt32(dr.GetOrdinal("UmisteniId"));
                    }
                    catch { }
                    try
                    {
                        item.NazevUmisteni = dr.GetString(dr.GetOrdinal("NazevUmisteni"));
                    }
                    catch { }


                    list.Add(item);
                }
            }
            cnn.Close();


            return list;
        }
    }
}