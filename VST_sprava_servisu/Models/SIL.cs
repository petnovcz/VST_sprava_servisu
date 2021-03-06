﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity;

using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace VST_sprava_servisu
{
    public class SIL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SIL");

        public static SIL_dobaprovozu Calc_PrumerDobyProvozu(int Rok)
        {
            SIL_dobaprovozu sil = new SIL_dobaprovozu();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append($" declare @rok as int = '{Rok}' ");
            sql.Append(" select AVG(X.I) as 'I',AVG(X.II) as 'II',CAST(AVG(X.PocetdnimeziIaIIrevizi) as decimal) as 'DniMeziRevizemi', AVG(((x.II - X.I) * 1.0 / x.PocetdnimeziIaIIrevizi) * 1.0) as 'TAKU',AVG(((x.II - X.I) * 1.0 / x.PocetdnimeziIaIIrevizi * 1.0) * 365.25) as 'TAKUII' from(");
            sql.Append(" select t0.NazevZakaznika, t1.NazevProvozu, t2.NazevUmisteni, t5.Nazev, t4.SerioveCislo,");
            sql.Append(" (SELECT HodinyProvozu FROM(");
            sql.Append(" SELECT");
            sql.Append(" ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t1.HodinyProvozu from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append(" WHERE rownumber = 2) as 'I',");
            sql.Append(" (SELECT HodinyProvozu FROM(");
            sql.Append(" SELECT");
            sql.Append(" ROW_NUMBER() OVER (ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t1.HodinyProvozu from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append(" WHERE rownumber = 1) as 'II',");
            sql.Append(" DateDiff(D,");
            sql.Append(" ((SELECT KontrolaProvedenaDne FROM(");
            sql.Append(" SELECT");
            sql.Append(" ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append(" WHERE rownumber = 2)),");
            sql.Append(" (SELECT KontrolaProvedenaDne FROM(");
            sql.Append(" SELECT");
            sql.Append(" ROW_NUMBER() OVER (ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append("  WHERE rownumber = 1)) as 'PocetdnimeziIaIIrevizi',");
            sql.Append(" (select COUNT(*) from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok = @rok  and t1.SCProvozuId = t3.id) as 'count'");
            sql.Append(" from Zakaznik t0 left join Provoz t1 on t1.ZakaznikId = t0.Id left join Umisteni t2 on t2.ProvozId = t1.Id left join SCProvozu t3 on t3.Umisteni = t2.Id left join SerioveCislo t4 on t4.Id = t3.SerioveCisloId");
            sql.Append(" left join Artikl t5 on t5.Id = t4.ArtiklId left join SkupinaArtiklu t6 on t6.Id = t5.SkupinaArtiklu ");
            sql.Append(" where t6.Id = 130");
            sql.Append(" and (select COUNT(*) from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok = @rok  and t1.SCProvozuId = t3.id) > 0 and t3.StatusId = 1 and");
            sql.Append("  (SELECT HodinyProvozu FROM( SELECT ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t1.HodinyProvozu from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append(" WHERE rownumber = 2) is not Null and (SELECT HodinyProvozu FROM( SELECT ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t1.HodinyProvozu from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append(" WHERE rownumber = 1) is not Null and DateDiff(D, ((SELECT KontrolaProvedenaDne FROM( SELECT ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append(" WHERE rownumber = 2)), (SELECT KontrolaProvedenaDne FROM( SELECT ROW_NUMBER() OVER (ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber,");
            sql.Append(" t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo");
            sql.Append(" WHERE rownumber = 1)) is not null) X");

            // log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandText = sql.ToString()
            };
            cnn.Open();

            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {

                    // sil.ServisniZasahId = ServisniZasahId;
                    try
                    {
                        sil.DobaprovozuI = dr.GetInt64(dr.GetOrdinal("I"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.DobaprovozuII = dr.GetInt64(dr.GetOrdinal("II"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }

                    try
                    {
                        sil.DniMeziRevizemi = dr.GetDecimal(dr.GetOrdinal("DniMeziRevizemi"));

                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.TAKU = dr.GetDecimal(dr.GetOrdinal("TAKU"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.TAKUII = dr.GetDecimal(dr.GetOrdinal("TAKUII"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }


                }
            }
            cnn.Close();

            return sil;
        }
        public static int Calc_PocetNebezpecnychPoruch(int Rok)
        {
            int count = 0;
            using (var db = new Model1Container())
            {
                count = db.ServisniZasahPrvek
                    .Include(t => t.Porucha)
                    .Include(t => t.ServisniZasah)
                    .Where(t => t.Porucha.SIL == true && t.ServisniZasah.DatumOdstraneni.Year == Rok).Count();

            }
            return count;
        }
        public static List<ServisniZasahPrvek> Calc_SeznamZavaznychPoruch(int Rok)
        {
            List<ServisniZasahPrvek> list = new List<ServisniZasahPrvek>();
            using (var db = new Model1Container())
            {
                list = db.ServisniZasahPrvek
                    .Include(t => t.Porucha)
                    //.Include(t=>t.)
                    .Include(t => t.ServisniZasah)
                    .Include(t => t.ServisniZasah.Zakaznik)
                    .Where(t => t.Porucha.SIL == true && t.ServisniZasah.DatumOdstraneni.Year == Rok).ToList();

            }
            return list;
        }
        public static List<SIL_dobaprovozu> Calc_SIL_DobaProvozu(int Rok, SIL_dobaprovozu PrumerDobyProvozu)
        {
            List<SIL_dobaprovozu> list = new List<SIL_dobaprovozu>();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            StringBuilder sql = new StringBuilder();


            sql.Append($" declare @rok as int = {Rok} ");
            sql.Append($" declare @prumerTAKU float = '{PrumerDobyProvozu.TAKU.ToString().Replace(",", ".")}'");
            sql.Append($" declare @prumerTAKUII float = '{PrumerDobyProvozu.TAKUII.ToString().Replace(",", ".")}'");
            sql.Append(" select *, " +
                "case when (((x.II - X.I) * 1.0 / x.PocetdnimeziIaIIrevizi) * 1.0 IS NULL) then @prumerTAKU else CAST(((x.II - X.I) * 1.0 / x.PocetdnimeziIaIIrevizi) * 1.0 as float) end as 'TAKU',"
                + " case when (((x.II - X.I) * 1.0 / x.PocetdnimeziIaIIrevizi * 1.0) * 365.25 IS null) then @prumerTAKUII else CAST(((x.II - X.I) * 1.0 / x.PocetdnimeziIaIIrevizi * 1.0) * 365.25 as float) end as 'TAKUII' "
                + " from( select t0.NazevZakaznika," +
                "t0.Id as 'ZakaznikId', " +
                "t1.NazevProvozu,t1.Id as 'ProvozId', " +
                "t2.NazevUmisteni,t2.Id as 'UmisteniId', " +
                "t5.Nazev as 'Artikl'," +
                "t5.Id as 'ArtiklId', " +
                "t4.SerioveCislo, " +
                "t4.Id as 'SerioveCisloId'," +
                "t3.Id as 'SCProvozuId', " +
                "(SELECT HodinyProvozu FROM( SELECT ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber, t1.HodinyProvozu from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo  WHERE rownumber = 2) as 'I',");
            sql.Append("(SELECT KontrolaProvedenaDne FROM( SELECT ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber, t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo  WHERE rownumber = 2 ) as 'DatumI', ");
            sql.Append(" (SELECT HodinyProvozu FROM( SELECT ROW_NUMBER() OVER (ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber, t1.HodinyProvozu from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo WHERE rownumber = 1) as 'II',");
            sql.Append(" (SELECT KontrolaProvedenaDne FROM( SELECT ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber, t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo  WHERE rownumber = 1 ) as 'DatumII',");
            sql.Append(" CAST(DateDiff(D, ((SELECT KontrolaProvedenaDne FROM( SELECT ROW_NUMBER() OVER(ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber, t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo WHERE rownumber = 2)),");
            sql.Append(" (SELECT KontrolaProvedenaDne FROM( SELECT ROW_NUMBER() OVER (ORDER BY coalesce(t0.KontrolaProvedenaDne, t0.Datumrevize) desc) AS rownumber, t0.KontrolaProvedenaDne from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok <= @rok  and t1.SCProvozuId = t3.id) as foo WHERE rownumber = 1)) as decimal) as 'PocetdnimeziIaIIrevizi',");
            sql.Append(" (select COUNT(*) from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok = @rok  and t1.SCProvozuId = t3.id) as 'count' from Zakaznik t0 left join Provoz t1 on t1.ZakaznikId = t0.Id left join Umisteni t2 on t2.ProvozId = t1.Id ");
            sql.Append(" left join SCProvozu t3 on t3.Umisteni = t2.Id left join SerioveCislo t4 on t4.Id = t3.SerioveCisloId left join Artikl t5 on t5.Id = t4.ArtiklId left join SkupinaArtiklu t6 on t6.Id = t5.SkupinaArtiklu");
            sql.Append(" where t6.Id = 130 and (select COUNT(*) from Revize t0 left join RevizeSC t1 on t0.Id = t1.RevizeId where Rok = @rok  and t1.SCProvozuId = t3.id) > 0 and t3.StatusId = 1  ) X");

            //log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand
            {
                Connection = cnn,
                CommandText = sql.ToString()
            };
            cnn.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {
                    SIL_dobaprovozu sil = new SIL_dobaprovozu();
                    try
                    {
                        sil.ZakaznikId = dr.GetInt32(dr.GetOrdinal("ZakaznikId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: ZakaznikId " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.ProvozId = dr.GetInt32(dr.GetOrdinal("ProvozId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: ProvozId" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }

                    try
                    {
                        sil.UmisteniId = dr.GetInt32(dr.GetOrdinal("UmisteniId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: UmisteniId" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.SCProvozuId = dr.GetInt32(dr.GetOrdinal("SCProvozuId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: SCProvozuId" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.SerioveCisloId = dr.GetInt32(dr.GetOrdinal("SerioveCisloId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: SerioveCisloId" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.ArtiklId = dr.GetInt32(dr.GetOrdinal("ArtiklId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: ArtiklId" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.DobaprovozuI = dr.GetInt64(dr.GetOrdinal("I"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: DobaprovozuI" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.DobaprovozuII = dr.GetInt64(dr.GetOrdinal("II"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: DobaprovozuII" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.DniMeziRevizemi = dr.GetDecimal(dr.GetOrdinal("PocetdnimeziIaIIrevizi"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: DniMeziRevizemi" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.PocetReviziVRoce = dr.GetInt32(dr.GetOrdinal("count"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: PocetReviziVRoce" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.TAKU = Convert.ToDecimal(dr.GetDouble(dr.GetOrdinal("TAKU")));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: TAKU" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.TAKUII = Convert.ToDecimal(dr.GetDouble(dr.GetOrdinal("TAKUII")));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: TAKUII" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.DatumI = (dr.GetDateTime(dr.GetOrdinal("DatumI")));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: TAKUII" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        sil.DatumII = (dr.GetDateTime(dr.GetOrdinal("DatumII")));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Error number: TAKUII" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }


                    sil.Zakaznik = Zakaznik.GetById(sil.ZakaznikId);
                    sil.Provoz = Provoz.GetById(sil.ProvozId);
                    sil.Umisteni = Umisteni.GetById(sil.UmisteniId);
                    sil.SerioveCislo = SerioveCislo.GetSerioveCisloById(sil.SerioveCisloId);
                    sil.SCProvozu = SCProvozu.GetSCProvozuById(sil.SCProvozuId);
                    sil.Artikl = Artikl.GetArtiklById(sil.ArtiklId);

                    list.Add(sil);
                }
            }
            cnn.Close();
            return list;
        }

        public static SIL Run(int Rok)
        {
            SIL sil = new SIL();
            sil.PrumerDobyProvozu = Calc_PrumerDobyProvozu(Rok);
            sil.PocetNebezpecnychPoruch = Calc_PocetNebezpecnychPoruch(Rok);
            sil.Rok = Rok;
            sil.SeznamZavaznychPoruch = Calc_SeznamZavaznychPoruch(Rok);
            sil.SIL_DobaProvozu = Calc_SIL_DobaProvozu(Rok, sil.PrumerDobyProvozu);

            return sil;
        }

        public int Rok { get; set; }
        public SIL_dobaprovozu PrumerDobyProvozu {get; set;}


        public List<SIL_dobaprovozu> SIL_DobaProvozu {get; set;}


        public partial class SIL_dobaprovozu 
            {

            public int ZakaznikId { get; set; }
            public virtual Zakaznik Zakaznik { get; set; }
            public int ProvozId { get; set; }
            public virtual Provoz Provoz { get; set; }
            public int UmisteniId { get; set; }
            public virtual Umisteni Umisteni { get; set; }
            public int SCProvozuId { get; set; }
            public virtual SCProvozu SCProvozu { get; set; }
            public int ArtiklId { get; set; }
            public virtual Artikl Artikl { get; set; }
            public int SerioveCisloId { get; set; }
            public virtual SerioveCislo SerioveCislo { get; set; }
            public long DobaprovozuI { get; set; }
            public long DobaprovozuII { get; set; }
            public decimal DniMeziRevizemi { get; set; }
            public int PocetReviziVRoce { get; set; }
            [DisplayFormat(DataFormatString = "{0:#.##}")]
            public decimal TAKU { get; set; }
            [DisplayFormat(DataFormatString = "{0:#.##}")]
            public decimal TAKUII { get; set; }
            public DateTime? DatumI { get; set; }
            public DateTime DatumII { get; set; }
        }

        public int PocetNebezpecnychPoruch {get; set;}

        public List<ServisniZasahPrvek> SeznamZavaznychPoruch {get; set;}


    }

    
}