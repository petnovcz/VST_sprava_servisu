using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;

namespace VST_sprava_servisu
{

    public partial class VypocetPlanuRevizi
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("VypocetPlanuRevizi");
        [Key]
        public int ZakaznikId { get; set; }
        public string Zakaznik { get; set; }
        public int ProvozId { get; set; }
        public string Provoz { get; set; }
        public int UmisteniId { get; set; }
        public string NazevUmisteni { get; set; }
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


        internal protected static List<VypocetPlanuRevizi> Run(string conn, int Year)
        {
            //bool done = true;
            List<VypocetPlanuRevizi> listplanrev = new List<VypocetPlanuRevizi>();
            listplanrev = VypocetPlanuRevizi.Calculate(conn, Year);
            listplanrev = VypocetPlanuRevizi.LoopAndCreate(conn, listplanrev);

            List<VypocetPlanuRevizi> listplanrev2 = new List<VypocetPlanuRevizi>();
            listplanrev2 = VypocetPlanuRevizi.Calculate2(conn);
            listplanrev2 = VypocetPlanuRevizi.LoopAndCreate(conn, listplanrev2);
            return listplanrev;
        }

        /// <summary>
        /// Výpočet příští revize dle sériových čísel v provozu pro umisteni která nejsou resena samostatně
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static List<VypocetPlanuRevizi> Calculate(string conn, int year)
        {
            List<VypocetPlanuRevizi> listplanrev = new List<VypocetPlanuRevizi>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select x.ZakaznikId , x.Zakaznik,x.ProvozId ,x.Provoz, ");
            sql.Append(" min(x.NextRevize) 'R1', Year(min(x.NextRevize)) as 'Rok_R1', case when Month(min(x.NextRevize)) <= 6 then 1 else 2 end as 'R1POL',");
            sql.Append(" min(x.Next2Revize) 'R2', Year(min(x.Next2Revize)) as 'Rok_R2', case when Month(min(x.Next2Revize)) <= 6 then 1 else 2 end as 'R2POL'");
            sql.Append(" from");
            sql.Append(" (");
            sql.Append(" select t1.Id as 'ZakaznikId', t1.NazevZakaznika as 'Zakaznik',");
            sql.Append(" t2.Id as 'ProvozId', t2.NazevProvozu as 'Provoz', t3.NazevUmisteni, t6.Nazev, t5.seriovecislo,");
            sql.Append(" DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni)) as 'NextRevize',");
            sql.Append(" DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni))) as 'Next2Revize',");
            sql.Append(" DATEADD(month, convert(int, t6.periodabaterie), coalesce(t4.datumbaterie, t4.datumprirazeni)) as 'NextBaterie',");
            sql.Append(" DATEADD(month, convert(int, t6.periodapyro), coalesce(t4.datumpyro, t4.datumprirazeni)) as 'NextPyro',");
            sql.Append(" DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t4.datumtlkzk, t4.datumprirazeni)) as 'NextTlkZk'");
            sql.Append(" from[Servis].[dbo].[Region] t0");
            sql.Append(" left join[Servis].[dbo].[Zakaznik] t1 on t0.id = t1.regionid");
            sql.Append(" left join[Servis].[dbo].[provoz] t2 on t2.zakaznikid = t1.id");
            sql.Append(" left join[Servis].[dbo].[umisteni] t3 on t3.provozid = t2.id and t3.SamostatnaRevize = 'false'");
            sql.Append(" left join[Servis].[dbo].[scprovozu] t4 on t4.provozid = t2.id and t4.umisteni = t3.id");
            sql.Append(" left join[Servis].[dbo].[SerioveCislo] t5 on t5.Id = t4.SerioveCisloId");
            sql.Append(" left join[Servis].[dbo].[Artikl] t6 on t5.ArtiklId = T6.Id");
            sql.Append(" where  t3.id is not null and t4.id is not null");
            sql.Append(" ) x");
            sql.Append(" group by x.ZakaznikId, x.Zakaznik,x.ProvozId, x.Provoz");

            log.Debug($"Calculate pro revizi pro rok: {year}");
            log.Debug(sql.ToString());

            SqlConnection cnn = new SqlConnection(conn);
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
                    VypocetPlanuRevizi item = new VypocetPlanuRevizi();
                    try
                    {
                        item.ZakaznikId = dr.GetInt32(dr.GetOrdinal("ZakaznikId"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.Zakaznik = dr.GetString(dr.GetOrdinal("Zakaznik"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.ProvozId = dr.GetInt32(dr.GetOrdinal("ProvozId"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.Provoz = dr.GetString(dr.GetOrdinal("Provoz"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.R1 = dr.GetDateTime(dr.GetOrdinal("R1"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.R1POL = dr.GetInt32(dr.GetOrdinal("R1POL"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.Rok_R1 = dr.GetInt32(dr.GetOrdinal("Rok_R1"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.R2 = dr.GetDateTime(dr.GetOrdinal("R2"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.R2POL = dr.GetInt32(dr.GetOrdinal("R2POL"));
                    }
                    catch (Exception ex) { }
                    try
                    {
                        item.Rok_R2 = dr.GetInt32(dr.GetOrdinal("Rok_R2"));
                    }
                    catch (Exception ex) { }

                    listplanrev.Add(item);
                }
            }
            cnn.Close();

            return listplanrev;
        }



        private static List<VypocetPlanuRevizi> Calculate2(string conn)
        {
            List<VypocetPlanuRevizi> listplanrev = new List<VypocetPlanuRevizi>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select x.ZakaznikId , x.Zakaznik,x.ProvozId ,x.Provoz, x.NazevUmisteni, x.UmisteniId, ");
            sql.Append(" min(x.NextRevize) 'R1', Year(min(x.NextRevize)) as 'Rok_R1', case when Month(min(x.NextRevize)) <= 6 then 1 else 2 end as 'R1POL',");
            sql.Append(" min(x.Next2Revize) 'R2', Year(min(x.Next2Revize)) as 'Rok_R2', case when Month(min(x.Next2Revize)) <= 6 then 1 else 2 end as 'R2POL'");
            sql.Append(" from");
            sql.Append(" (");
            sql.Append(" select t1.Id as 'ZakaznikId', t1.NazevZakaznika as 'Zakaznik',");
            sql.Append(" t2.Id as 'ProvozId', t2.NazevProvozu as 'Provoz',t3.Id as 'UmisteniId', t3.NazevUmisteni, t6.Nazev, t5.seriovecislo,");
            sql.Append(" DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni)) as 'NextRevize',");
            sql.Append(" DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni))) as 'Next2Revize',");
            sql.Append(" DATEADD(month, convert(int, t6.periodabaterie), coalesce(t4.datumbaterie, t4.datumprirazeni)) as 'NextBaterie',");
            sql.Append(" DATEADD(month, convert(int, t6.periodapyro), coalesce(t4.datumpyro, t4.datumprirazeni)) as 'NextPyro',");
            sql.Append(" DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t4.datumtlkzk, t4.datumprirazeni)) as 'NextTlkZk'");
            sql.Append(" from[Servis].[dbo].[Region] t0");
            sql.Append(" left join[Servis].[dbo].[Zakaznik] t1 on t0.id = t1.regionid");
            sql.Append(" left join[Servis].[dbo].[provoz] t2 on t2.zakaznikid = t1.id");
            sql.Append(" left join[Servis].[dbo].[umisteni] t3 on t3.provozid = t2.id and t3.SamostatnaRevize = 'true'");
            sql.Append(" left join[Servis].[dbo].[scprovozu] t4 on t4.provozid = t2.id and t4.umisteni = t3.id");
            sql.Append(" left join[Servis].[dbo].[SerioveCislo] t5 on t5.Id = t4.SerioveCisloId");
            sql.Append(" left join[Servis].[dbo].[Artikl] t6 on t5.ArtiklId = T6.Id");
            sql.Append(" where  t3.id is not null and t4.id is not null");
            sql.Append(" ) x");
            sql.Append(" group by x.ZakaznikId, x.Zakaznik,x.ProvozId, x.Provoz, x.NazevUmisteni, x.UmisteniId");

            log.Debug($"Calculate2 ");
            log.Debug(sql.ToString());

            SqlConnection cnn = new SqlConnection(conn);
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
                    VypocetPlanuRevizi item = new VypocetPlanuRevizi();
                    try
                    {
                        item.ZakaznikId = dr.GetInt32(dr.GetOrdinal("ZakaznikId"));
                    }
                    catch (Exception ex) { //log.Info($"ZakaznikId prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Zakaznik = dr.GetString(dr.GetOrdinal("Zakaznik"));
                    }
                    catch (Exception ex) { //log.Info($"Zakaznik prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.ProvozId = dr.GetInt32(dr.GetOrdinal("ProvozId"));
                    }
                    catch (Exception ex) { //log.Info($"ProvozId prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Provoz = dr.GetString(dr.GetOrdinal("Provoz"));
                    }
                    catch (Exception ex) { //log.Info($"Provoz prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.UmisteniId = dr.GetInt32(dr.GetOrdinal("UmisteniId"));
                    }
                    catch (Exception ex) { //log.Info($"UmisteniId prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.NazevUmisteni = dr.GetString(dr.GetOrdinal("NazevUmisteni"));
                    }
                    catch (Exception ex) { //log.Info($"NazevUmisteni prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.R1 = dr.GetDateTime(dr.GetOrdinal("R1"));
                    }
                    catch (Exception ex) { //log.Info($"R1 prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.R1POL = dr.GetInt32(dr.GetOrdinal("R1POL"));
                    }
                    catch (Exception ex) { //log.Info($"R1POL prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Rok_R1 = dr.GetInt32(dr.GetOrdinal("Rok_R1"));
                    }
                    catch (Exception ex) { //log.Info($"Rok_R1 prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.R2 = dr.GetDateTime(dr.GetOrdinal("R2"));
                    }
                    catch (Exception ex) { //log.Info($"R2 prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.R2POL = dr.GetInt32(dr.GetOrdinal("R2POL"));
                    }
                    catch (Exception ex) { //log.Info($"R2POL prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Rok_R2 = dr.GetInt32(dr.GetOrdinal("Rok_R2"));
                    }
                    catch (Exception ex) { //log.Info($"Rok_R2 prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }

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
                if (item.UmisteniId == 0)
                {
                    var r1exist = Revize.ExistRevision(item.ZakaznikId, item.ProvozId, item.Rok_R1, item.R1POL, null);
                    if (r1exist == false)
                    {
                        item.Revize1 = Revize.GenerateRevision(item.ProvozId, item.Rok_R1, item.R1POL, item.R1.Value, StatusRevize.Planned(), null, "","");
                        List<SCList> sclist = new List<SCList>();
                        sclist = SCList.FindScForRevision(conn, item.ProvozId, null, item.Rok_R1, item.R1POL);
                        var t = SCList.AddItemsFromList(sclist, item.Revize1.Id);
                        RevizeSC revizesc = new RevizeSC();
                        item.Revize1.UpdateRevizeHeader(item.Revize1.Id);
                        //bool done = RevizeSC.CreateUpdateSC(SCProvozu.GetList(item.ProvozId, null, 1, null), item.Revize1.Id);
                    }
                    else
                    {
                        item.Revize1 = Revize.ReturnRevision(item.ZakaznikId, item.ProvozId, item.Rok_R1, item.R1POL, null);
                    }
                    var r2exist = Revize.ExistRevision(item.ZakaznikId, item.ProvozId, item.Rok_R2, item.R2POL, null);
                    if (r2exist == false)
                    {
                        item.Revize2 = Revize.GenerateRevision(item.ProvozId, item.Rok_R2, item.R2POL, item.R2.Value, StatusRevize.Planned(), null, "", "");
                        List<SCList> sclist = new List<SCList>();
                        sclist = SCList.FindScForRevision(conn, item.ProvozId, null, item.Rok_R2, item.R2POL);
                        var t = SCList.AddItemsFromList(sclist, item.Revize2.Id);
                        RevizeSC revizesc = new RevizeSC();
                        item.Revize2.UpdateRevizeHeader(item.Revize2.Id);
                    }
                    else
                    {
                        item.Revize2 = Revize.ReturnRevision(item.ZakaznikId, item.ProvozId, item.Rok_R2, item.R2POL, null);
                    }
                    // update hlavicky - počet baterií, palníků atd



                }
                else {
                    var r1exist = Revize.ExistRevision(item.ZakaznikId, item.ProvozId, item.Rok_R1, item.R1POL, item.UmisteniId);
                    if (r1exist == false)
                    {
                        item.Revize1 = Revize.GenerateRevision(item.ProvozId, item.Rok_R1, item.R1POL, item.R1.Value, StatusRevize.Planned(), item.UmisteniId, "", "");
                        
                        List<SCList> sclist = new List<SCList>();
                        sclist = SCList.FindScForRevision(conn, item.ProvozId, item.UmisteniId, item.Rok_R1, item.R1POL);
                        var t = SCList.AddItemsFromList(sclist, item.Revize1.Id);
                        RevizeSC revizesc = new RevizeSC();
                        item.Revize1.UpdateRevizeHeader(item.Revize1.Id);


                    }
                    else
                    {
                        item.Revize1 = Revize.ReturnRevision(item.ZakaznikId, item.ProvozId, item.Rok_R1, item.R1POL, item.UmisteniId);
                    }
                    var r2exist = Revize.ExistRevision(item.ZakaznikId, item.ProvozId, item.Rok_R2, item.R2POL, item.UmisteniId);
                    if (r2exist == false)
                    {
                        item.Revize2 = Revize.GenerateRevision(item.ProvozId, item.Rok_R2, item.R2POL, item.R2.Value, StatusRevize.Planned(), item.UmisteniId, "", "");
                        List<SCList> sclist = new List<SCList>();
                        sclist = SCList.FindScForRevision(conn, item.ProvozId, item.UmisteniId, item.Rok_R2, item.R2POL);
                        var t = SCList.AddItemsFromList(sclist, item.Revize2.Id);
                        RevizeSC revizesc = new RevizeSC();
                        item.Revize1.UpdateRevizeHeader(item.Revize2.Id);
                    }
                    else
                    {
                        item.Revize2 = Revize.ReturnRevision(item.ZakaznikId, item.ProvozId, item.Rok_R2, item.R2POL, item.UmisteniId);
                    }
                    // update hlavicky - počet baterií, palníků atd

                }
            }
            return listplanrev;
        }
    }


    public partial class SCList
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SCList");

        public int Id { get; set; }
        public int Revize { get; set; }
        public int Baterie { get; set; }
        public int Pyro { get; set; }
        public int TlkZk { get; set; }

        public static List<SCList> FindScForRevision(string conn, int Provoz, int? Umisteni, int Rok, int Polol)
        {
            List<SCList> listplanrev = new List<SCList>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select t4.id, ");
            sql.Append($" case when(Year(DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni))) = '{Rok}'");
            sql.Append($" and((Month(DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni))) <= 6  and  '{Polol}' = '1')");
            sql.Append($" or(Month(DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni))) > 6  and  '{Polol}' = '2')))");
            sql.Append($" or(Year(DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni)))) = '{Rok}'");
            sql.Append($" and((Month(DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni)))) <= 6 and  '{Polol}' = '1')");
            sql.Append($" or(Month(DATEADD(month, convert(int, t6.periodarevize), DATEADD(month, convert(int, t6.periodarevize), coalesce(t4.datumrevize, t4.datumprirazeni)))) > 6 and  '{Polol}' = '2')))");
            sql.Append(" then 1 else 0 end as 'Revize',");
            sql.Append($" case when(Year(DATEADD(month, convert(int, t6.periodapyro), coalesce(t4.datumpyro, t4.datumprirazeni))) = '{Rok}'");
            sql.Append($" and((Month(DATEADD(month, convert(int, t6.periodapyro), coalesce(t4.datumpyro, t4.datumprirazeni))) <= 6  and  '{Polol}' = '1')");
            sql.Append($" or(Month(DATEADD(month, convert(int, t6.periodapyro), coalesce(t4.datumpyro, t4.datumprirazeni))) > 6  and  '{Polol}' = '2')");
            sql.Append(" )) then 1 else 0 end as 'Pyro',");
            sql.Append($" case when(Year(DATEADD(month, convert(int, t6.periodabaterie), coalesce(t4.datumbaterie, t4.datumprirazeni))) = '{Rok}'");
            sql.Append($" and((Month(DATEADD(month, convert(int, t6.periodabaterie), coalesce(t4.datumbaterie, t4.datumprirazeni))) <= 6  and  '{Polol}' = '1')");
            sql.Append($" or(Month(DATEADD(month, convert(int, t6.periodabaterie), coalesce(t4.datumbaterie, t4.datumprirazeni))) > 6  and  '{Polol}' = '2')");
            sql.Append(" )) then 1 else 0 end as 'Baterie',");
            sql.Append($" case when(Year(DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t4.datumtlkzk, t4.datumprirazeni))) = '{Rok}'");
            sql.Append($" and((Month(DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t4.datumtlkzk, t4.datumprirazeni))) <= 6  and  '{Polol}' = '1')");
            sql.Append($" or(Month(DATEADD(month, convert(int, t6.periodatlakovazk), coalesce(t4.datumtlkzk, t4.datumprirazeni))) > 6  and  '{Polol}' = '2')");
            sql.Append(" )) then 1 else 0 end as 'TlKZK'");
            sql.Append(" from[Servis].[dbo].[Region] t0");
            sql.Append(" left join[Servis].[dbo].[Zakaznik] t1 on t0.id = t1.regionid");
            sql.Append(" left join[Servis].[dbo].[provoz] t2 on t2.zakaznikid = t1.id");
            if (Umisteni == null)
            {
                sql.Append(" left join[Servis].[dbo].[umisteni] t3 on t3.provozid = t2.id and t3.SamostatnaRevize = 'false' ");
            }
            else
            {
                sql.Append(" left join[Servis].[dbo].[umisteni] t3 on t3.provozid = t2.id and t3.SamostatnaRevize = 'true'");
            }
            sql.Append(" left join[Servis].[dbo].[scprovozu] t4 on t4.provozid = t2.id and t4.umisteni = t3.id");
            sql.Append(" left join[Servis].[dbo].[SerioveCislo] t5 on t5.Id = t4.SerioveCisloId");
            sql.Append(" left join[Servis].[dbo].[Artikl] t6 on t5.ArtiklId = T6.Id");
            sql.Append(" where  t3.id is not null and t4.id is not null");
            sql.Append($" and T2.Id = {Provoz}");
            if (Umisteni != null)
            {
                sql.Append($" and T3.Id = {Umisteni}");
            }
            log.Debug($"FindScForRevision Provoz:{Provoz}, Umisteni: {Umisteni}, Rok: {Rok}, Polol: {Polol} ");
            log.Debug(sql.ToString());

            SqlConnection cnn = new SqlConnection(conn);
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
                    SCList item = new SCList();
                    try
                    {
                        item.Id = dr.GetInt32(dr.GetOrdinal("id"));
                    }
                    catch (Exception ex) { //log.Info($"id prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Revize = dr.GetInt32(dr.GetOrdinal("Revize"));
                    }
                    catch (Exception ex) { //log.Info($"Revize prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Pyro = dr.GetInt32(dr.GetOrdinal("Pyro"));
                    }
                    catch (Exception ex) { //log.Info($"Pyro prázdné {ex.Message} {ex.InnerException} {ex.Data}"); 
                    }
                    try
                    {
                        item.Baterie = dr.GetInt32(dr.GetOrdinal("Baterie"));
                    }
                    catch (Exception ex) { //log.Info($"Baterie prázdné {ex.Message} {ex.InnerException} {ex.Data}"); 
                        }
                    try
                    {
                        item.TlkZk = dr.GetInt32(dr.GetOrdinal("TlkZk"));
                    }
                    catch (Exception ex) { //log.Info($"TlkZk prázdné {ex.Message} {ex.InnerException} {ex.Data}"); 
                            }

                    listplanrev.Add(item);
                }
            }
            cnn.Close();

            return listplanrev;
        }


        public static List<SCProvozu> AddItemsFromList (List<SCList> sclist, int revize)
        {
            List<SCProvozu> list = new List<SCProvozu>();
            SCProvozu sc = new SCProvozu();
            foreach (var item in sclist)
            {
                using (var dbCtx = new Model1Container())
                {
                    if(item.Revize == 1) { 
                        sc = dbCtx.SCProvozu.Where(r => r.Id == item.Id).FirstOrDefault();
                        var exist = dbCtx.RevizeSC.Where(r => r.RevizeId == revize && r.SCProvozuId == sc.Id).Count();
                        if (exist == 0)
                        {
                            RevizeSC revizesc = new RevizeSC();
                            revizesc.UmisteniId = sc.Umisteni;
                            revizesc.RevizeId = revize;
                            revizesc.SCProvozuId = sc.Id;
                            if (item.Baterie == 1) { revizesc.Baterie = true; }
                            if (item.Pyro == 1) { revizesc.Pyro = true; }
                            if (item.TlkZk == 1) { revizesc.TlakovaZkouska = true; }
                            try
                            {
                                dbCtx.RevizeSC.Add(revizesc);
                                dbCtx.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                log.Error($"AddItemsFromList {ex.Message} {ex.InnerException} {ex.Data}");
                            }
                        }
                    }

                    list.Add(sc);
                }


            }


            return list;
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