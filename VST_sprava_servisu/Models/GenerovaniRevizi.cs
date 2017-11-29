using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace VST_sprava_servisu
{

    public partial class VypocetPlanuRevizi
    {
        [Key]
        public int ZakaznikId { get; set; }
        public string Zakaznik { get; set; }
        public int ProvozId { get; set; }
        public string Provoz { get; set; }
        public Nullable<DateTime> R1 { get; set; }
        public Nullable<DateTime> R2 { get; set; }
        public int Rok_R1 { get; set; }
        public int Rok_R2 { get; set; }
        public int R1POL { get; set; }
        public int R2POL { get; set; }
        public Revize Revize1 { get; set; }
        public Revize Revize2 { get; set; }
        //public DateTime DatumRevize1 { get; set; }
        //public DateTime DatumRevize2 { get; set; }


        internal protected static List<VypocetPlanuRevizi> Run(string conn)
        {
            //bool done = true;
            List<VypocetPlanuRevizi> listplanrev = new List<VypocetPlanuRevizi>();
            listplanrev = VypocetPlanuRevizi.Calculate(conn);
            listplanrev = VypocetPlanuRevizi.LoopAndCreate(conn, listplanrev);

            return listplanrev;
        }

        /// <summary>
        /// Výpočet příští revize dle sériových čísel v provozu pro umisteni která nejsou resena samostatně
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static List<VypocetPlanuRevizi> Calculate(string conn)
        {
            List<VypocetPlanuRevizi> listplanrev = new List<VypocetPlanuRevizi>();
            string sql = @"";
            sql = sql + @" select x.ZakaznikId , x.Zakaznik,x.ProvozId ,x.Provoz, ";
            sql = sql + @" min(x.NextRevize) 'R1', Year(min(x.NextRevize)) as 'Rok_R1', case when Month(min(x.NextRevize)) <= 6 then 1 else 2 end as 'R1POL',";
            sql = sql + @" min(x.Next2Revize) 'R2', Year(min(x.Next2Revize)) as 'Rok_R2', case when Month(min(x.Next2Revize)) <= 6 then 1 else 2 end as 'R2POL'";
            sql = sql + @" from";
            sql = sql + @" (";
            sql = sql + @" select t1.Id as 'ZakaznikId', t1.NazevZakaznika as 'Zakaznik',";
            sql = sql + @" t2.Id as 'ProvozId', t2.NazevProvozu as 'Provoz', t3.NazevUmisteni, t6.Nazev, t5.seriovecislo,";
            sql = sql + @" DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni)) as 'NextRevize',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni))) as 'Next2Revize',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodabaterie), coalesce(t4.datumbaterie, t4.datumprirazeni)) as 'NextBaterie',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodapyro), coalesce(t4.datumpyro, t4.datumprirazeni)) as 'NextPyro',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t4.datumtlkzk, t4.datumprirazeni)) as 'NextTlkZk'";
            sql = sql + @" from[Servis].[dbo].[Region] t0";
            sql = sql + @" left join[Servis].[dbo].[Zakaznik] t1 on t0.id = t1.regionid";
            sql = sql + @" left join[Servis].[dbo].[provoz] t2 on t2.zakaznikid = t1.id";
            sql = sql + @" left join[Servis].[dbo].[umisteni] t3 on t3.provozid = t2.id and t3.SamostatnaRevize = 'false'";
            sql = sql + @" left join[Servis].[dbo].[scprovozu] t4 on t4.provozid = t2.id and t4.umisteni = t3.id";
            sql = sql + @" left join[Servis].[dbo].[SerioveCislo] t5 on t5.Id = t4.SerioveCisloId";
            sql = sql + @" left join[Servis].[dbo].[Artikl] t6 on t5.ArtiklId = T6.Id";
            sql = sql + @" where  t3.id is not null and t4.id is not null";
            sql = sql + @" ) x";
            sql = sql + @" group by x.ZakaznikId, x.Zakaznik,x.ProvozId, x.Provoz";

            SqlConnection cnn = new SqlConnection(conn);
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
                    VypocetPlanuRevizi item = new VypocetPlanuRevizi();
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
                        item.R1 = dr.GetDateTime(dr.GetOrdinal("R1"));
                    }
                    catch { }
                    try
                    {
                        item.R1POL = dr.GetInt32(dr.GetOrdinal("R1POL"));
                    }
                    catch { }
                    try
                    {
                        item.Rok_R1 = dr.GetInt32(dr.GetOrdinal("Rok_R1"));
                    }
                    catch { }
                    try
                    {
                        item.R2 = dr.GetDateTime(dr.GetOrdinal("R2"));
                    }
                    catch { }
                    try
                    {
                        item.R2POL = dr.GetInt32(dr.GetOrdinal("R2POL"));
                    }
                    catch { }
                    try
                    {
                        item.Rok_R2 = dr.GetInt32(dr.GetOrdinal("Rok_R2"));
                    }
                    catch { }

                    listplanrev.Add(item);
                }
            }
            cnn.Close();

            return listplanrev;
        }

        /// <summary>
        /// Prohledani seznamu a dohledani zda již neexistuji revize v danem obdobi
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="listplanrev"></param>
        /// <returns></returns>
        private static List<VypocetPlanuRevizi> LoopAndCreate(string conn, List<VypocetPlanuRevizi> listplanrev)
        {
            foreach (var item in listplanrev)
            {
                
                var r1exist = Revize.ExistRevision(item.ZakaznikId, item.ProvozId, item.Rok_R1, item.R1POL);
                if (r1exist == false)
                {
                    item.Revize1 = Revize.GenerateRevision(item.ProvozId,item.Rok_R1,item.R1POL, item.R1.Value, StatusRevize.Planned());
                    RevizeSC revizesc = new RevizeSC();
                    bool done = RevizeSC.CreateUpdateSC(SCProvozu.GetList(item.ProvozId, null, 1, null), item.Revize1.Id);
                }
                else
                {
                    item.Revize1 = Revize.ReturnRevision(item.ZakaznikId, item.ProvozId, item.Rok_R1, item.R1POL);
                }
                var r2exist = Revize.ExistRevision(item.ZakaznikId, item.ProvozId, item.Rok_R2, item.R2POL);
                if (r2exist == false)
                {
                    item.Revize2 = Revize.GenerateRevision(item.ProvozId, item.Rok_R2, item.R2POL, item.R2.Value, StatusRevize.Planned());
                    bool done = RevizeSC.CreateUpdateSC(SCProvozu.GetList(item.ProvozId, null, 1, null), item.Revize1.Id);
                }
                else
                {
                    item.Revize2 = Revize.ReturnRevision(item.ZakaznikId, item.ProvozId, item.Rok_R2, item.R2POL);
                }             
            }
            return listplanrev;
        }
    }


    

    

    public partial class GRFiltr
    {
        [Key]
        public int Rok { get; set; }        
        public int? Region { get; set; }
        public int? Zakaznik { get; set; }
        

    }


    public partial class Rok
    {
        public int Id;
        public int Value;

        internal protected static List<Rok> GetYears()
        {
            Rok thisyear = new Rok();
            Rok nextyear = new Rok();
            Rok lastyear = new Rok();
            thisyear.Id = DateTime.Now.Year;
            thisyear.Value = DateTime.Now.Year;
            lastyear.Id = thisyear.Id - 1;
            lastyear.Value = thisyear.Id - 1;
            nextyear.Id = thisyear.Id + 1;
            nextyear.Value = thisyear.Id + 1;
            List<Rok> rokl = new List<Rok>();
            rokl.Add(lastyear);
            rokl.Add(thisyear);
            rokl.Add(nextyear);
            return rokl;
        }
        internal protected static Rok ThisYear()
        {
            Rok thisyear = new Rok();

            thisyear.Id = DateTime.Now.Year;
            thisyear.Value = DateTime.Now.Year;

            return thisyear;
        }
    }



}