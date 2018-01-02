using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace VST_sprava_servisu
{

    public partial class CalculatedSCForRevision
    {
        public int ZakaznikId { get; set;}
        public int ProvozId { get; set; }
        public int UmisteniId { get; set; }
        public int SCProvozuId { get; set; }
        public Nullable<DateTime> NextRevize { get; set; }
        public Nullable<DateTime> Next2Revize { get; set; }
        public Nullable<DateTime> NextBaterie { get; set; }
        public Nullable<DateTime> NextPyro { get; set; }
        public Nullable<DateTime> NextTlkZk { get; set; }
    }

    public partial class DnyRevize
    {
        internal DateTime DenRevize1 { get; set; }
        internal DateTime prvnidenobdobiR1 { get; set; }
        internal DateTime poslednidenobdobiR1 { get; set; }
        internal DateTime DenRevize2 { get; set; }
        internal DateTime prvnidenobdobiR2 { get; set; }
        internal DateTime poslednidenobdobiR2 { get; set; }
    }
    public partial class GenRevizeCust
    {
        public int Rok { get; set; }
        public int Skupina { get; set; }
        public string Search { get; set; }
        public int ZakaznikId { get; set; }
        public int ProvozId { get; set; }
        public int UmisteniId { get; set; }

        private Zakaznik Zakaznik { get; set; }
        private Provoz Provoz { get; set; }
        private Umisteni Umisteni { get; set; }

        private Revize Revize1 { get; set; }
        private Revize Revize2 { get; set; }

        public string Nabidka { get; set; }
        public string Projekt { get; set; }

        public DnyRevize dnyrevize { get; set; }


        public static void Run(int ZakaznikId, int ProvozId, int Rok, int? UmisteniId, string Nabidka, string Projekt)
        {
            GenerujRevizi( ZakaznikId, ProvozId, Rok,  UmisteniId,  Nabidka,  Projekt);
        }


        /// <summary>
        /// Vzgenerovani novych revizi pro obdobi a zakaznika, provoz pripadne umisteni. Probehne storno starych revizi. Pokud je datum pred obdobim 
        /// dojde ke generovani k prvnimu dni vybraneho roku.
        /// </summary>
        /// <param name="ZakaznikId"></param>
        /// <param name="ProvozId"></param>
        /// <param name="Rok"></param>
        /// <param name="UmisteniId"></param>
        /// <returns></returns>
        /// 
        internal protected static void GenerujRevizi(int ZakaznikId, int ProvozId, int Rok, int? UmisteniId, string Nabidka, string Projekt)
        {
            bool existrevize1;
            bool existrevize2;


            GenRevizeCust gen = new GenRevizeCust();
            DnyRevize dnyRevize = MinimalniDatum(ZakaznikId, ProvozId, Rok, UmisteniId);
            gen.dnyrevize = dnyRevize;
            

            // REVIZE1
            if ((UmisteniId != null) && (UmisteniId != 0))
            {
                existrevize1 = Revize.ExistRevision(ZakaznikId, ProvozId, Rok, 1, UmisteniId);
                if (existrevize1 == true)
                {
                    //prirazeni revize do modelu
                    gen.Revize1 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 1, UmisteniId);
                }
                else
                {
                    gen.dnyrevize.prvnidenobdobiR1 = Prvnidenobdobi(Rok, 1);
                    gen.dnyrevize.poslednidenobdobiR1 = Poslednidenobdobi(Rok, 1);



                    // Prvni revize v obdobi
                    if (
                        (dnyRevize.DenRevize1 >= gen.dnyrevize.prvnidenobdobiR1)
                        &&
                        (dnyRevize.DenRevize1 <= gen.dnyrevize.poslednidenobdobiR1)
                        )
                     {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, dnyRevize.DenRevize1, 1, UmisteniId, Nabidka, Projekt);
                            
                     }
                    // prvni revize pred obdobim
                    if (
                        (dnyRevize.DenRevize1 < gen.dnyrevize.prvnidenobdobiR1)

                        )
                    {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, gen.dnyrevize.prvnidenobdobiR1, 1, UmisteniId, Nabidka, Projekt);

                    }
                    // prvni revize v druhem pololeti vygeneruje se jako revize 2
                    if (
                        (dnyRevize.DenRevize1 >= gen.dnyrevize.prvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize1 <= gen.dnyrevize.poslednidenobdobiR2)

                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, dnyRevize.DenRevize1, 1, UmisteniId, Nabidka, Projekt);

                    }

                }
            }
            else
            {
                existrevize1 = Revize.ExistRevision(ZakaznikId, ProvozId, Rok, 1, null);
                
                if (existrevize1 == true)
                {
                    //prirazeni revize do modelu
                    gen.Revize1 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 1, null);
                }
                else
                {
                    gen.dnyrevize.prvnidenobdobiR1 = Prvnidenobdobi(Rok, 1);
                    gen.dnyrevize.poslednidenobdobiR1 = Poslednidenobdobi(Rok, 1);

                    // Prvni revize v obdobi
                    if (
                        (dnyRevize.DenRevize1 >= gen.dnyrevize.prvnidenobdobiR1)
                        &&
                        (dnyRevize.DenRevize1 <= gen.dnyrevize.poslednidenobdobiR1)
                        )
                    {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, dnyRevize.DenRevize1, 1, null, Nabidka, Projekt);

                    }
                    // prvni revize pred obdobim
                    if (
                        (dnyRevize.DenRevize1 < gen.dnyrevize.prvnidenobdobiR1)

                        )
                    {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, gen.dnyrevize.prvnidenobdobiR1, 1, null, Nabidka, Projekt);

                    }
                    // prvni revize v druhem pololeti vygeneruje se jako revize 2
                    if (
                        (dnyRevize.DenRevize1 >= gen.dnyrevize.prvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize1 <= gen.dnyrevize.poslednidenobdobiR2)

                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, dnyRevize.DenRevize1, 1, null, Nabidka, Projekt);

                    }
                }
            }

            // REVIZE2
            
            if ((UmisteniId != null) && (UmisteniId != 0))
            {
                existrevize2 = Revize.ExistRevision(ZakaznikId, ProvozId, Rok, 2, UmisteniId);
                if (existrevize2 == true)
                {
                    //prirazeni revize do modelu
                    gen.Revize2 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 2, UmisteniId);
                }
                else
                {
                    gen.dnyrevize.prvnidenobdobiR2 = Prvnidenobdobi(Rok, 2);
                    gen.dnyrevize.poslednidenobdobiR2 = Poslednidenobdobi(Rok, 2);



                    // druha revize v obdobi
                    if (
                        (dnyRevize.DenRevize2 >= gen.dnyrevize.prvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize2 <= gen.dnyrevize.poslednidenobdobiR2)
                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, dnyRevize.DenRevize2, 1, UmisteniId, Nabidka, Projekt);

                    }
                    // druha revize pred obdobim
                    if (
                        (dnyRevize.DenRevize2 < gen.dnyrevize.prvnidenobdobiR2)

                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, gen.dnyrevize.prvnidenobdobiR2, 1, UmisteniId, Nabidka, Projekt);

                    }
                    

                }
            }
            else
            {
                existrevize2 = Revize.ExistRevision(ZakaznikId, ProvozId, Rok, 2, null);

                if (existrevize2 == true)
                {
                    //prirazeni revize do modelu
                    gen.Revize2 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 2, null);
                }
                else
                {
                    gen.dnyrevize.prvnidenobdobiR2 = Prvnidenobdobi(Rok, 2);
                    gen.dnyrevize.poslednidenobdobiR2 = Poslednidenobdobi(Rok, 2);

                    // druha revize v obdobi
                    if (
                        (dnyRevize.DenRevize2 >= gen.dnyrevize.prvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize2 <= gen.dnyrevize.poslednidenobdobiR2)
                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, dnyRevize.DenRevize2, 1, null, Nabidka, Projekt);

                    }
                    // druha revize pred obdobim
                    if (
                        (dnyRevize.DenRevize2 < gen.dnyrevize.prvnidenobdobiR2)

                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, gen.dnyrevize.prvnidenobdobiR2, 1, null, Nabidka, Projekt);

                    }
                }
            }

            List<CalculatedSCForRevision> list = calculatescfrorevision(ZakaznikId, ProvozId, UmisteniId);
            InsertSCtoRevision(gen, list);
            gen.Revize1.UpdateRevizeHeader(gen.Revize1.Id);
            gen.Revize2.UpdateRevizeHeader(gen.Revize2.Id);
            // na zaklade prvku provozu spocitat kdy by mela byt dalsi revize 
            // - pokud v obdobi - spoctene datum ()
            // - pokud starsi vygenerovat k 1.1.daneho roku
            // - pokud do tohoto obdobi nespada tak negenerovat (jenom klapka nema artikly atd.)


        }
        
        /// <summary>
        /// Vraci prvni den obdobi
        /// </summary>
        /// <param name="Rok"></param>
        /// <param name="Pololeti"></param>
        /// <returns></returns>
        internal protected static DateTime Prvnidenobdobi(int Rok, int Pololeti)
        {
            DateTime Prvnidenobdobi = DateTime.Now;
            if (Pololeti == 1)
            {
                try { Prvnidenobdobi = new DateTime(Rok, 1, 1); }
                catch { }
            }
            if (Pololeti == 2)
            {
                try { Prvnidenobdobi = new DateTime(Rok, 6, 1); }
                catch { }
            }
            
            return Prvnidenobdobi;
        }
        
        /// <summary>
        /// Vraci posledni den obdobi
        /// </summary>
        /// <param name="Rok"></param>
        /// <param name="Pololeti"></param>
        /// <returns></returns>
        internal protected static DateTime Poslednidenobdobi(int Rok, int Pololeti)
        {
            DateTime Poslednidenobdobi = DateTime.Now;
            if (Pololeti == 1)
            {
                try { Poslednidenobdobi = new DateTime(Rok, 5, 31); }
                catch { }
            }
            if (Pololeti == 2)
            {
                try { Poslednidenobdobi = new DateTime(Rok, 12, 31); }
                catch { }
            }

            return Poslednidenobdobi;
        }
        
        /// <summary>
        /// Spocita minimalni datum pro nasledujic revizi 1 a revizi 2
        /// </summary>
        /// <param name="ZakaznikId"></param>
        /// <param name="ProvozId"></param>
        /// <param name="Rok"></param>
        /// <param name="UmisteniId"></param>
        /// <returns></returns>
        internal protected static DnyRevize MinimalniDatum(int ZakaznikId, int ProvozId, int Rok, int? UmisteniId)
        {
            if (UmisteniId == null) { UmisteniId = 0; }
            DnyRevize dnyrevize = new DnyRevize() ;
            string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string sql = @" select x.ZakaznikId , x.Zakaznik ,x.ProvozId ,x.Provoz, min(x.NextRevize) 'R1', Year(min(x.NextRevize)) as 'Rok_R1', case when Month(min(x.NextRevize)) <= 6 then 1 else 2 end as 'R1POL', ";
            sql = sql + @" min(x.Next2Revize) 'R2', Year(min(x.Next2Revize)) as 'Rok_R2', case when Month(min(x.Next2Revize)) <= 6 then 1 else 2 end as 'R2POL'";
            sql = sql + @" from (";
            sql = sql + @" select t1.Id as 'ZakaznikId', t1.NazevZakaznika as 'Zakaznik',";
            sql = sql + @" t2.Id as 'ProvozId', t2.NazevProvozu as 'Provoz', t3.Id as 'UmisteniId', t3.NazevUmisteni, t6.Nazev, t5.seriovecislo,";
            sql = sql + @" DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni)) as 'NextRevize',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni))) as 'Next2Revize',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodabaterie), coalesce(t4.datumbaterie, t4.datumprirazeni)) as 'NextBaterie',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodapyro), coalesce(t4.datumpyro, t4.datumprirazeni)) as 'NextPyro',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t4.datumtlkzk, t4.datumprirazeni)) as 'NextTlkZk'";
            sql = sql + @" from Region t0";
            sql = sql + @" left join Zakaznik t1 on t0.id = t1.regionid";
            sql = sql + @" left join Provoz t2 on t2.zakaznikid = t1.id";
            sql = sql + @" left join Umisteni t3 on t3.provozid = t2.id";
            sql = sql + @" left join Scprovozu t4 on t4.provozid = t2.id and t4.umisteni = t3.id";
            sql = sql + @" left join SerioveCislo t5 on t5.Id = t4.SerioveCisloId";
            sql = sql + @" left join Artikl t6 on t5.ArtiklId = T6.Id";
            sql = sql + @" where";
            sql = sql + @" t1.ID = '"+ ZakaznikId +"' and T2.id = '"+ ProvozId + "' and(T3.Id = '"+ UmisteniId + "' or '"+ UmisteniId + "' = '0')";
            sql = sql + @" ) x";
            sql = sql + @" group by x.ZakaznikId, x.Zakaznik,x.ProvozId, x.Provoz";
            
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
                        dnyrevize.DenRevize1 = dr.GetDateTime(dr.GetOrdinal("R1"));
                    }
                    catch { }
                    try
                    {
                        dnyrevize.DenRevize2 = dr.GetDateTime(dr.GetOrdinal("R2"));
                    }
                    catch { }
                }
            }
            cnn.Close();



            return dnyrevize;
        }

        internal protected static bool Stornootevrenychrevizi(int ZakaznikId, int ProvozId, int Rok, int? UmisteniId)
        {
            bool result = false;

            return result;
        }

        internal protected static List<CalculatedSCForRevision> calculatescfrorevision(int ZakaznikId, int ProvozId, int? UmisteniId)
        {
            List<CalculatedSCForRevision> calc = new List<CalculatedSCForRevision>();

            
            string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string sql = @"select t0.id as 'ZakaznikId', t1.id as 'ProvozId',t2.id as 'UmisteniId', t3.Id as 'SCProvozuId',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodarevize), coalesce(t3.datumrevize, t3.datumprirazeni)) as 'NextRevize',";
            sql = sql + @" DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t3.datumrevize, t3.datumprirazeni))) as 'Next2Revize',";
            sql = sql + @" case when T7.VymenaBaterie = 1 then DATEADD(month, convert(int, t6.periodabaterie), coalesce(t3.datumbaterie, t3.datumprirazeni)) else null end as 'NextBaterie',";
            sql = sql + @" case when T7.VymenaPyro = 1 then DATEADD(month, convert(int, t6.periodapyro), coalesce(t3.datumpyro, t3.datumprirazeni)) else null end as 'NextPyro',";
            sql = sql + @" case when T7.tlakovazk = 1 then DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t3.datumtlkzk, t3.datumprirazeni)) else null end as 'NextTlkZk'";
            sql = sql + @" from Zakaznik t0";
            sql = sql + @" left join Provoz T1 on T1.ZakaznikId = T0.Id";
            sql = sql + @" left join Umisteni t2 on t2.ProvozId = t1.Id";
            sql = sql + @" left join SCProvozu T3 on t3.ProvozId = t1.Id and t3.Umisteni = t2.id";
            sql = sql + @" left join SerioveCislo t5 on t5.Id = t3.SerioveCisloId";
            sql = sql + @" left join Artikl t6 on t5.ArtiklId = T6.Id";
            sql = sql + @" left join SkupinaArtiklu t7 on t6.skupinaartiklu = T7.id";
            sql = sql + @" where ";
            sql = sql + @" t0.id = '"+ ZakaznikId +"'";
            sql = sql + @" and t1.Id = '"+ ProvozId +"' ";
            sql = sql + @" and (t2.Id = '"+ UmisteniId + "' or '" + UmisteniId + "' = '0')";
            

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
                    CalculatedSCForRevision item = new CalculatedSCForRevision();
                    try
                    {
                        item.ZakaznikId = dr.GetInt32(dr.GetOrdinal("ZakaznikId"));
                    }
                    catch { }
                    try
                    {
                        item.ProvozId = dr.GetInt32(dr.GetOrdinal("ProvozId"));
                    }
                    catch { }
                    try
                    {
                        item.UmisteniId = dr.GetInt32(dr.GetOrdinal("UmisteniId"));
                    }
                    catch { }
                    try
                    {
                        item.SCProvozuId = dr.GetInt32(dr.GetOrdinal("SCProvozuId"));
                    }
                    catch { }
                    try
                    {
                        item.NextRevize = dr.GetDateTime(dr.GetOrdinal("NextRevize"));
                    }
                    catch { }
                    try
                    {
                        item.Next2Revize = dr.GetDateTime(dr.GetOrdinal("Next2Revize"));
                    }
                    catch { }
                    try
                    {
                        item.NextPyro = dr.GetDateTime(dr.GetOrdinal("NextPyro"));
                    }
                    catch { }
                    try
                    {
                        item.NextBaterie = dr.GetDateTime(dr.GetOrdinal("NextBaterie"));
                    }
                    catch { }
                    try
                    {
                        item.NextTlkZk = dr.GetDateTime(dr.GetOrdinal("NextTlkZk"));
                    }
                    catch { }
                    calc.Add(item);
                }
            }
            cnn.Close();


            return calc;
        }



        internal protected static void InsertSCtoRevision(GenRevizeCust gen, List<CalculatedSCForRevision> list)
        {
            foreach (var item in list)
            {
                RevizeSC RSC1 = new RevizeSC();
                RevizeSC RSC2 = new RevizeSC();







                if (item.NextRevize <= gen.dnyrevize.poslednidenobdobiR1)
                {
                    
                    RSC1.RevizeId = gen.Revize1.Id;
                    RSC1.SCProvozuId = item.SCProvozuId;
                    RSC1.UmisteniId = item.UmisteniId;
                    if (item.NextBaterie <= gen.dnyrevize.poslednidenobdobiR1)
                    {
                        RSC1.Baterie = true;
                    }
                    else
                    {
                        RSC1.Baterie = false;
                    }
                    if (item.NextPyro <= gen.dnyrevize.poslednidenobdobiR1)
                    {
                        RSC1.Pyro = true;
                    }
                    else
                    {
                        RSC1.Pyro = false;
                    }
                    if (item.NextTlkZk <= gen.dnyrevize.poslednidenobdobiR1)
                    {
                        RSC1.TlakovaZkouska = true;
                    }
                    else
                    {
                        RSC1.TlakovaZkouska = false;
                    }
                }
                if (item.NextRevize >= gen.dnyrevize.prvnidenobdobiR2 && item.NextRevize <= gen.dnyrevize.poslednidenobdobiR2)
                {
                    RSC2.RevizeId = gen.Revize2.Id;
                    RSC2.SCProvozuId = item.SCProvozuId;
                    RSC2.UmisteniId = item.UmisteniId;
                    if (item.NextBaterie >= gen.dnyrevize.prvnidenobdobiR2 && item.NextBaterie <= gen.dnyrevize.poslednidenobdobiR2)
                    {
                        RSC2.Baterie = true;
                    }
                    else
                    {
                        RSC2.Baterie = false;
                    }
                    if (item.NextPyro >= gen.dnyrevize.prvnidenobdobiR2 && item.NextPyro <= gen.dnyrevize.poslednidenobdobiR2)
                    {
                        RSC2.Pyro = true;
                    }
                    else
                    {
                        RSC2.Pyro = false;
                    }
                    if (item.NextTlkZk >= gen.dnyrevize.prvnidenobdobiR2 && item.NextTlkZk <= gen.dnyrevize.poslednidenobdobiR2)
                    {
                        RSC2.TlakovaZkouska = true;
                    }
                    else
                    {
                        RSC2.TlakovaZkouska = false;
                    }


                }

                if (item.Next2Revize <=  gen.dnyrevize.poslednidenobdobiR2)
                {
                    RSC2.RevizeId = gen.Revize2.Id;
                    RSC2.SCProvozuId = item.SCProvozuId;
                    RSC2.UmisteniId = item.UmisteniId;
                    if (item.NextBaterie >= gen.dnyrevize.prvnidenobdobiR2 && item.NextBaterie <= gen.dnyrevize.poslednidenobdobiR2)
                    {
                        RSC2.Baterie = true;
                    }
                    else
                    {
                        RSC2.Baterie = false;
                    }
                    if (item.NextPyro >= gen.dnyrevize.prvnidenobdobiR2 && item.NextPyro <= gen.dnyrevize.poslednidenobdobiR2)
                    {
                        RSC2.Pyro = true;
                    }
                    else
                    {
                        RSC2.Pyro = false;
                    }
                    if (item.NextTlkZk >= gen.dnyrevize.prvnidenobdobiR2 && item.NextTlkZk <= gen.dnyrevize.poslednidenobdobiR2)
                    {
                        RSC2.TlakovaZkouska = true;
                    }
                    else
                    {
                        RSC2.TlakovaZkouska = false;
                    }


                }
                using (var dbCtx = new Model1Container())
                {
                    
                    try
                    {
                        dbCtx.RevizeSC.Add(RSC1);
                        dbCtx.SaveChanges();
                    }
                    catch { }
                    try
                    {
                        dbCtx.RevizeSC.Add(RSC2);
                        dbCtx.SaveChanges();
                    }
                    catch { }

                }



            }
        }




    }
}