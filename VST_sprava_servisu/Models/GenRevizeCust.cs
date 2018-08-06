using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public partial class GenRevizeCust
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("GenRevizeCust");

        public int Rok { get; set; }
        public int Skupina { get; set; }
        public string Search { get; set; }
        public int ZakaznikId { get; set; }
        public int ProvozId { get; set; }
        public int? UmisteniId { get; set; }
        private Zakaznik Zakaznik { get { Zakaznik zakaznik = Zakaznik.GetById(ZakaznikId); return zakaznik; } }
        private Provoz Provoz { get { Provoz provoz = Provoz.GetById(ProvozId); return provoz; } }
        private Umisteni Umisteni { get { Umisteni umisteni = Umisteni.GetById(UmisteniId.Value); return umisteni; } }
        private Revize Revize1 { get; set; }
        private Revize Revize2 { get; set; }
        public string Nabidka { get; set; }
        public string Projekt { get; set; }

        public List<ListObject> ProjektList {
            get
            {
                List<ListObject> list = new List<ListObject>();
                string con = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();
                sql.Append($"Select Code, U_Descript as 'Name' from [dbo].[@VCZ_CT_PRJ] T1 where t1.U_CardCode = '{Zakaznik.KodSAP}' and t1.U_Status not in ('2','7','8')");

                //LOGOVANI
                log.Debug($"ProjektList pro Zakaznika: {Zakaznik.KodSAP}");
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
                    while (dr.Read())
                    {
                        ListObject item = new ListObject();
                        try
                        {
                            item.Code = dr.GetString(dr.GetOrdinal("Code"));
                        }
                        catch (Exception ex) {
                            log.Debug("ProjektList - načtení Code: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Name = dr.GetString(dr.GetOrdinal("Name"));
                        }
                        catch (Exception ex)
                        {
                            log.Debug("ProjektList - načtení Name: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        list.Add(item);
                    }
                }
                cnn.Close();

                return list;
            }

        }
        public List<ListObject> NabidkaList {
            get {
                List<ListObject> list = new List<ListObject>();
                string con = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();
                sql.Append($"Select t0.Docnum as 'Code', t1.U_Descript as 'Name' from Oqut t0 left join [dbo].[@VCZ_CT_PRJ] T1 on t0.Project = t1.Code where CardCode = '{Zakaznik.KodSAP}' and t1.U_Status not in ('2','7','8')");

                //LOGOVANI
                log.Debug($"MinimalniDatum pro revizi pro ZakaznikID: {ZakaznikId},ProvozId : {ProvozId}, Rok: {Rok}, UmisteniId: {UmisteniId}");
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
                    while (dr.Read())
                    {
                        ListObject item = new ListObject();
                        try
                        {
                            item.Code = Convert.ToString(dr.GetInt32(dr.GetOrdinal("Code")));
                        }
                        catch (Exception ex)
                        {
                            log.Debug("NabidkaList - načtení Code: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Name = dr.GetString(dr.GetOrdinal("Name"));
                        }
                        catch (Exception ex)
                        {
                            log.Debug("NabidkaList - načtení Name: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        string Name;
                        Name = item.Code + " - " + item.Name;
                        item.Name = Name;
                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;
            }


        }


        public DnyRevize Dnyrevize { get; set; }

        

        public partial class ListObject
        {
            public string Code { get; set; }
            public string Name { get; set; }

        }

        [Authorize(Roles = "Administrator,Manager")]
        public static void Run(int ZakaznikId, int ProvozId, int Rok, int? UmisteniId, string Nabidka, string Projekt)
        {
            GenerujRevizi( ZakaznikId, ProvozId, Rok,  UmisteniId,  Nabidka,  Projekt);
            GenerovaniRevizeTlakoveZkousky.GenerujReviziTlakoveZkousky(ZakaznikId, ProvozId, UmisteniId, Rok);
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
        [Authorize(Roles = "Administrator,Manager")]
        internal protected static void GenerujRevizi(int ZakaznikId, int ProvozId, int Rok, int? UmisteniId, string Nabidka, string Projekt)
        {
            bool existrevize1;
            bool existrevize2;


            GenRevizeCust gen = new GenRevizeCust();
            DnyRevize dnyRevize = MinimalniDatum(ZakaznikId, ProvozId, Rok, UmisteniId);
            gen.Dnyrevize = dnyRevize;
            

            // REVIZE1
            if ((UmisteniId != null) && (UmisteniId != 0))
            {
                existrevize1 = Revize.ExistRevision(ZakaznikId, ProvozId, Rok, 1, UmisteniId);
                if (existrevize1 == true)
                {
                    //prirazeni revize do modelu
                    gen.Revize1 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 1, UmisteniId,null);
                }
                else
                {
                    gen.Dnyrevize.PrvnidenobdobiR1 = Prvnidenobdobi(Rok, 1);
                    gen.Dnyrevize.PoslednidenobdobiR1 = Poslednidenobdobi(Rok, 1);
                    gen.Dnyrevize.PrvnidenobdobiR2 = Prvnidenobdobi(Rok, 2);
                    gen.Dnyrevize.PoslednidenobdobiR2 = Poslednidenobdobi(Rok, 2);
                    // Prvni revize v obdobi
                    if (
                        (dnyRevize.DenRevize1 >= gen.Dnyrevize.PrvnidenobdobiR1)
                        &&
                        (dnyRevize.DenRevize1 <= gen.Dnyrevize.PoslednidenobdobiR1)
                        )
                     {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, dnyRevize.DenRevize1, 1, UmisteniId, Nabidka, Projekt);
                            
                     }
                    // prvni revize pred obdobim
                    if (
                        (dnyRevize.DenRevize1 < gen.Dnyrevize.PrvnidenobdobiR1)

                        )
                    {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, gen.Dnyrevize.PrvnidenobdobiR1, 1, UmisteniId, Nabidka, Projekt);

                    }
                    // prvni revize v druhem pololeti vygeneruje se jako revize 2
                    if (
                        (dnyRevize.DenRevize1 >= gen.Dnyrevize.PrvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize1 <= gen.Dnyrevize.PoslednidenobdobiR2)

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
                    gen.Revize1 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 1, null,null);
                }
                else
                {
                    gen.Dnyrevize.PrvnidenobdobiR1 = Prvnidenobdobi(Rok, 1);
                    gen.Dnyrevize.PoslednidenobdobiR1 = Poslednidenobdobi(Rok, 1);
                    gen.Dnyrevize.PrvnidenobdobiR2 = Prvnidenobdobi(Rok, 2);
                    gen.Dnyrevize.PoslednidenobdobiR2 = Poslednidenobdobi(Rok, 2);

                    // Prvni revize v obdobi
                    if (
                        (dnyRevize.DenRevize1 >= gen.Dnyrevize.PrvnidenobdobiR1)
                        &&
                        (dnyRevize.DenRevize1 <= gen.Dnyrevize.PoslednidenobdobiR1)
                        )
                    {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, dnyRevize.DenRevize1, 1, null, Nabidka, Projekt);

                    }
                    // prvni revize pred obdobim
                    if (
                        (dnyRevize.DenRevize1 < gen.Dnyrevize.PrvnidenobdobiR1)

                        )
                    {
                        gen.Revize1 = Revize.GenerateRevision(ProvozId, Rok, 1, gen.Dnyrevize.PrvnidenobdobiR1, 1, null, Nabidka, Projekt);

                    }
                    // prvni revize v druhem pololeti vygeneruje se jako revize 2
                    if (
                        (dnyRevize.DenRevize1 >= gen.Dnyrevize.PrvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize1 <= gen.Dnyrevize.PoslednidenobdobiR2)
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
                    gen.Revize2 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 2, UmisteniId,null);
                }
                else
                {
                    gen.Dnyrevize.PrvnidenobdobiR2 = Prvnidenobdobi(Rok, 2);
                    gen.Dnyrevize.PoslednidenobdobiR2 = Poslednidenobdobi(Rok, 2);
                    // druha revize v obdobi
                    if (
                        (dnyRevize.DenRevize2 >= gen.Dnyrevize.PrvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize2 <= gen.Dnyrevize.PoslednidenobdobiR2)
                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, dnyRevize.DenRevize2, 1, UmisteniId, Nabidka, Projekt);

                    }
                    // druha revize pred obdobim
                    if (
                        (dnyRevize.DenRevize2 < gen.Dnyrevize.PrvnidenobdobiR2)

                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, gen.Dnyrevize.PrvnidenobdobiR2, 1, UmisteniId, Nabidka, Projekt);
                    }                   
                }
            }
            else
            {
                existrevize2 = Revize.ExistRevision(ZakaznikId, ProvozId, Rok, 2, null);

                if (existrevize2 == true)
                {
                    //prirazeni revize do modelu
                    gen.Revize2 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 2, null,null);
                }
                else
                {
                    gen.Dnyrevize.PrvnidenobdobiR2 = Prvnidenobdobi(Rok, 2);
                    gen.Dnyrevize.PoslednidenobdobiR2 = Poslednidenobdobi(Rok, 2);

                    // druha revize v obdobi
                    if (
                        (dnyRevize.DenRevize2 >= gen.Dnyrevize.PrvnidenobdobiR2)
                        &&
                        (dnyRevize.DenRevize2 <= gen.Dnyrevize.PoslednidenobdobiR2)
                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, dnyRevize.DenRevize2, 1, null, Nabidka, Projekt);

                    }
                    // druha revize pred obdobim
                    if (
                        (dnyRevize.DenRevize2 < gen.Dnyrevize.PrvnidenobdobiR2)

                        )
                    {
                        gen.Revize2 = Revize.GenerateRevision(ProvozId, Rok, 2, gen.Dnyrevize.PrvnidenobdobiR2, 1, null, Nabidka, Projekt);

                    }
                }
            }

            List<CalculatedSCForRevision> list = Calculatescfrorevision(ZakaznikId, ProvozId, UmisteniId);
            InsertSCtoRevision(gen, list);
            try
            {
                Revize.UpdateRevizeHeader(gen.Revize1.Id);
            }
            catch(Exception ex)
            {
                log.Error("Revize1 - update revize header: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
            }
            try
            {
                Revize.UpdateRevizeHeader(gen.Revize2.Id);
            }
            catch(Exception ex)
            {
                log.Error("Revize2 - update revize header: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
            }
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
                try { Prvnidenobdobi = new DateTime(Rok, 7, 1); }
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
                try { Poslednidenobdobi = new DateTime(Rok, 6, 30); }
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
            StringBuilder sql = new StringBuilder();
            sql.Append(" select x.ZakaznikId , x.Zakaznik ,x.ProvozId ,x.Provoz, min(x.NextRevize) 'R1', Year(min(x.NextRevize)) as 'Rok_R1', case when Month(min(x.NextRevize)) <= 6 then 1 else 2 end as 'R1POL', ");
            sql.Append(" min(x.Next2Revize) 'R2', Year(min(x.Next2Revize)) as 'Rok_R2', case when Month(min(x.Next2Revize)) <= 6 then 1 else 2 end as 'R2POL'");
            sql.Append(" from (");
            sql.Append(" select t1.Id as 'ZakaznikId', t1.NazevZakaznika as 'Zakaznik',");
            sql.Append(" t2.Id as 'ProvozId', t2.NazevProvozu as 'Provoz', t3.Id as 'UmisteniId', t3.NazevUmisteni, t6.Nazev, t5.seriovecislo,");
            sql.Append(" DATEADD(month, convert(int, coalesce(t4.UpravenaPeriodaRevize,t6.periodarevize)), coalesce(t4.datumrevize, t4.datumprirazeni)) as 'NextRevize',");
            sql.Append(" DATEADD(month, convert(int, coalesce(t4.UpravenaPeriodaRevize,t6.periodarevize)), DATEADD(month, convert(int, coalesce(t4.UpravenaPeriodaRevize,t6.periodarevize)), coalesce(t4.datumrevize, t4.datumprirazeni))) as 'Next2Revize',");
            sql.Append(" DATEADD(month, convert(int, coalesce(t4.UpravenaPeriodaBaterie,t6.periodabaterie)), coalesce(t4.datumbaterie, t4.datumprirazeni)) as 'NextBaterie',");
            sql.Append(" DATEADD(month, convert(int, coalesce(t4.UpravenaPeriodaPyro,t6.periodapyro)), coalesce(t4.datumpyro, t4.datumprirazeni)) as 'NextPyro',");
            sql.Append(" DATEADD(month, convert(int, coalesce(t4.UpravenaPeriodaTlkZk,t6.periodatlakovazk)), coalesce(t4.datumtlkzk, t4.datumprirazeni)) as 'NextTlkZk'");
            sql.Append(" from Region t0");
            sql.Append(" left join Zakaznik t1 on t0.id = t1.regionid");
            sql.Append(" left join Provoz t2 on t2.zakaznikid = t1.id");
            sql.Append(" left join Umisteni t3 on t3.provozid = t2.id");
            sql.Append(" left join Scprovozu t4 on t4.provozid = t2.id and t4.umisteni = t3.id");
            sql.Append(" left join SerioveCislo t5 on t5.Id = t4.SerioveCisloId");
            sql.Append(" left join Artikl t6 on t5.ArtiklId = T6.Id");
            sql.Append(" where");
            sql.Append($" t1.ID = '{ZakaznikId}' and T2.id = '{ProvozId}' and(T3.Id = '{UmisteniId}' or '{UmisteniId}' = '0')");
            sql.Append(" ) x");
            sql.Append(" group by x.ZakaznikId, x.Zakaznik,x.ProvozId, x.Provoz");

            //LOGOVANI
            log.Debug($"MinimalniDatum pro revizi pro ZakaznikID: {ZakaznikId},ProvozId : {ProvozId}, Rok: {Rok}, UmisteniId: {UmisteniId}");
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
                        dnyrevize.DenRevize1 = dr.GetDateTime(dr.GetOrdinal("R1"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("MinimalniDatum - Načtení R1: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        dnyrevize.DenRevize2 = dr.GetDateTime(dr.GetOrdinal("R2"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("MinimalniDatum - Načtení R2: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                }
            }
            cnn.Close();



            return dnyrevize;
        }

        /*internal protected static bool Stornootevrenychrevizi(int ZakaznikId, int ProvozId, int Rok, int? UmisteniId)
        {
            bool result = false;

            return result;
        }*/

        internal protected static List<CalculatedSCForRevision> Calculatescfrorevision(int ZakaznikId, int ProvozId, int? UmisteniId)
        {
            List<CalculatedSCForRevision> calc = new List<CalculatedSCForRevision>();

            string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            StringBuilder sql = new StringBuilder();
            sql.Append("select t0.id as 'ZakaznikId', t1.id as 'ProvozId',t2.id as 'UmisteniId', t3.Id as 'SCProvozuId',");
            sql.Append(" DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaRevize,t6.periodarevize)), coalesce(t3.datumrevize, t3.datumprirazeni)) as 'NextRevize',");
            sql.Append(" DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaRevize,t6.periodarevize)), DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaRevize,t6.periodarevize)), coalesce(t3.datumrevize, t3.datumprirazeni))) as 'Next2Revize',");
            sql.Append(" case when T7.VymenaBaterie = 1 then DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaBaterie,t6.periodabaterie)), coalesce(t3.datumbaterie, t3.datumprirazeni)) else null end as 'NextBaterie',");
            sql.Append(" case when T7.VymenaPyro = 1 then DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaPyro,t6.periodapyro)), coalesce(t3.datumpyro, t3.datumprirazeni)) else null end as 'NextPyro',");
            sql.Append(" case when T7.tlakovazk = 1 then DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaTlkZk,t6.periodatlakovazk)), coalesce(t3.datumtlkzk, t3.datumprirazeni)) else null end as 'NextTlkZk',");
            // revize tlakove nadoby
            sql.Append(" case when T6.TlakovaNadoba = 1 then DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaRevizeTlakoveNadoby,t6.PeriodaRevizeTlakoveNadoby)), coalesce(t3.DatumRevizeTlakoveNadoby, t3.datumprirazeni)) else null end as 'RevizeTlakoveNadoby',");
            // vnitrni revize tlakove nadoby
            sql.Append(" case when T6.TlakovaNadoba = 1 then DATEADD(month, convert(int, coalesce(t3.UpravenaPeriodaVnitrniRevizeTlakoveNadoby,t6.PeriodaVnitrniRevize)), coalesce(t3.DatumVnitrniRevizeTlakoveNadoby, t3.datumprirazeni)) else null end as 'VnitrniRevizeTlakoveNadoby'");

            sql.Append(" from Zakaznik t0");
            sql.Append(" left join Provoz T1 on T1.ZakaznikId = T0.Id");
            sql.Append(" left join Umisteni t2 on t2.ProvozId = t1.Id");
            sql.Append(" left join SCProvozu T3 on t3.ProvozId = t1.Id and t3.Umisteni = t2.id");
            sql.Append(" left join SerioveCislo t5 on t5.Id = t3.SerioveCisloId");
            sql.Append(" left join Artikl t6 on t5.ArtiklId = T6.Id");
            sql.Append(" left join SkupinaArtiklu t7 on t6.skupinaartiklu = T7.id");
            sql.Append(" where ");
            sql.Append($" t0.id = '{ZakaznikId}'");
            sql.Append($" and t1.Id = '{ProvozId}' ");
            sql.Append($" and (t2.Id = '{UmisteniId}' or '{UmisteniId}' = '')");
            sql.Append($" and T3.StatusId <> 2");

            //LOGOVANI
            log.Debug($"calculatescfrorevision pro ZakaznikId: {ZakaznikId}, ProvozId: {ProvozId}, UmisteniId: {UmisteniId}");
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
                    CalculatedSCForRevision item = new CalculatedSCForRevision();
                    try
                    {
                        item.ZakaznikId = dr.GetInt32(dr.GetOrdinal("ZakaznikId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení ZakaznikId: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.ProvozId = dr.GetInt32(dr.GetOrdinal("ProvozId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení ProvozId: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.UmisteniId = dr.GetInt32(dr.GetOrdinal("UmisteniId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení UmisteniId: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.SCProvozuId = dr.GetInt32(dr.GetOrdinal("SCProvozuId"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení SCProvozuId: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.NextRevize = dr.GetDateTime(dr.GetOrdinal("NextRevize"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení NextRevize: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.Next2Revize = dr.GetDateTime(dr.GetOrdinal("Next2Revize"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení Next2Revize: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.NextPyro = dr.GetDateTime(dr.GetOrdinal("NextPyro"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení NextPyro: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.NextBaterie = dr.GetDateTime(dr.GetOrdinal("NextBaterie"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení NextBaterie: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.NextTlkZk = dr.GetDateTime(dr.GetOrdinal("NextTlkZk"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení NextTlkZk: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        item.RevizeTlakoveNadoby = dr.GetDateTime(dr.GetOrdinal("RevizeTlakoveNadoby"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení RevizeTlakoveNadoby: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    
                        try
                    {
                        item.VnitrniRevizeTlakoveNadoby = dr.GetDateTime(dr.GetOrdinal("VnitrniRevizeTlakoveNadoby"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Calculatescfrorevision - načtení VnitrniRevizeTlakoveNadoby: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    calc.Add(item);
                }
            }
            cnn.Close();


            return calc;
        }


        [Authorize(Roles = "Administrator,Manager")]
        internal protected static void InsertSCtoRevision(GenRevizeCust gen, List<CalculatedSCForRevision> list)
        {
            foreach (var item in list)
            {
                RevizeSC RSC1 = new RevizeSC();
                RevizeSC RSC2 = new RevizeSC();
                if (item.NextRevize <= gen.Dnyrevize.PoslednidenobdobiR1)
                {
                    RSC1.RevizeId = gen.Revize1.Id;
                    RSC1.SCProvozuId = item.SCProvozuId;
                    RSC1.UmisteniId = item.UmisteniId;
                    if (item.NextBaterie <= gen.Dnyrevize.PoslednidenobdobiR1)
                    {
                        RSC1.Baterie = true;
                    }
                    else
                    {
                        RSC1.Baterie = false;
                    }
                    if (item.NextPyro <= gen.Dnyrevize.PoslednidenobdobiR1)
                    {
                        RSC1.Pyro = true;
                    }
                    else
                    {
                        RSC1.Pyro = false;
                    }
                    if (item.NextTlkZk <= gen.Dnyrevize.PoslednidenobdobiR1)
                    {
                        RSC1.TlakovaZkouska = true;
                    }
                    else
                    {
                        RSC1.TlakovaZkouska = false;
                    }

                    if (item.VnitrniRevizeTlakoveNadoby <= gen.Dnyrevize.PoslednidenobdobiR1)
                    {
                        RSC1.VnitrniRevizeTlakoveNadoby = true;
                    }
                    else
                    {
                        RSC1.VnitrniRevizeTlakoveNadoby = false;
                    }

                    if ((item.RevizeTlakoveNadoby <= gen.Dnyrevize.PoslednidenobdobiR1) && (RSC1.VnitrniRevizeTlakoveNadoby != true))
                    {
                        RSC1.RevizeTlakoveNadoby = true;
                    }
                    else
                    {
                        RSC1.RevizeTlakoveNadoby = false;
                    }
                    
                }
                if (item.NextRevize >= gen.Dnyrevize.PrvnidenobdobiR2 && item.NextRevize <= gen.Dnyrevize.PoslednidenobdobiR2)
                {
                    RSC2.RevizeId = gen.Revize2.Id;
                    RSC2.SCProvozuId = item.SCProvozuId;
                    RSC2.UmisteniId = item.UmisteniId;
                    if (item.NextBaterie >= gen.Dnyrevize.PrvnidenobdobiR2 && item.NextBaterie <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.Baterie = true;
                    }
                    else
                    {
                        RSC2.Baterie = false;
                    }
                    if (item.NextPyro >= gen.Dnyrevize.PrvnidenobdobiR2 && item.NextPyro <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.Pyro = true;
                    }
                    else
                    {
                        RSC2.Pyro = false;
                    }
                    if (item.NextTlkZk >= gen.Dnyrevize.PrvnidenobdobiR2 && item.NextTlkZk <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.TlakovaZkouska = true;
                    }
                    else
                    {
                        RSC2.TlakovaZkouska = false;
                    }

                    if (item.VnitrniRevizeTlakoveNadoby >= gen.Dnyrevize.PrvnidenobdobiR2 && item.VnitrniRevizeTlakoveNadoby <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.VnitrniRevizeTlakoveNadoby = true;
                    }
                    else
                    {
                        RSC2.VnitrniRevizeTlakoveNadoby = false;
                    }

                    if ((item.RevizeTlakoveNadoby >= gen.Dnyrevize.PrvnidenobdobiR2 && item.RevizeTlakoveNadoby <= gen.Dnyrevize.PoslednidenobdobiR2) && RSC2.VnitrniRevizeTlakoveNadoby != true)
                    {
                        RSC2.RevizeTlakoveNadoby = true;
                    }
                    else
                    {
                        RSC2.RevizeTlakoveNadoby = false;
                    }
                    
                }

                if (item.Next2Revize <=  gen.Dnyrevize.PoslednidenobdobiR2)
                {
                    RSC2.RevizeId = gen.Revize2.Id;
                    RSC2.SCProvozuId = item.SCProvozuId;
                    RSC2.UmisteniId = item.UmisteniId;
                    if (item.NextBaterie >= gen.Dnyrevize.PrvnidenobdobiR2 && item.NextBaterie <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.Baterie = true;
                    }
                    else
                    {
                        RSC2.Baterie = false;
                    }
                    if (item.NextPyro >= gen.Dnyrevize.PrvnidenobdobiR2 && item.NextPyro <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.Pyro = true;
                    }
                    else
                    {
                        RSC2.Pyro = false;
                    }
                    if (item.NextTlkZk >= gen.Dnyrevize.PrvnidenobdobiR2 && item.NextTlkZk <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.TlakovaZkouska = true;
                    }
                    else
                    {
                        RSC2.TlakovaZkouska = false;
                    }

                    if (item.VnitrniRevizeTlakoveNadoby >= gen.Dnyrevize.PrvnidenobdobiR2 && item.VnitrniRevizeTlakoveNadoby <= gen.Dnyrevize.PoslednidenobdobiR2)
                    {
                        RSC2.VnitrniRevizeTlakoveNadoby = true;
                    }
                    else
                    {
                        RSC2.VnitrniRevizeTlakoveNadoby = false;
                    }

                    if ((item.RevizeTlakoveNadoby >= gen.Dnyrevize.PrvnidenobdobiR2 && item.RevizeTlakoveNadoby <= gen.Dnyrevize.PoslednidenobdobiR2) && RSC2.VnitrniRevizeTlakoveNadoby != true)
                    {
                        RSC2.RevizeTlakoveNadoby = true;
                    }
                    else
                    {
                        RSC2.RevizeTlakoveNadoby = false;
                    }

                    
                }
                using (var dbCtx = new Model1Container())
                {

                    if (RSC1.RevizeId != 0)
                    {
                        try
                        {
                            RSC1.Stav = true;
                            dbCtx.RevizeSC.Add(RSC1);
                            dbCtx.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            log.Error($"InsertSCtoRevision - insert REVIZESC do Revize1 {ex.Message} {ex.InnerException} {ex.Data}");
                        }
                    }

                    if (RSC2.RevizeId != 0)
                    {
                        try
                        {
                            RSC2.Stav = true;
                            dbCtx.RevizeSC.Add(RSC2);
                            dbCtx.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            log.Error($"InsertSCtoRevision - insert REVIZESC do Revize2 {ex.Message} {ex.InnerException} {ex.Data}");
                        }
                    }
                }
            }
        }
    }
}