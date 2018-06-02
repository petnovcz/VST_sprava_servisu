using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace VST_sprava_servisu
{
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
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;
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
            sql.Append($" from [{RS_dtb}].[dbo].[Region] t0");
            sql.Append($" left join [{RS_dtb}].[dbo].[Zakaznik] t1 on t0.id = t1.regionid");
            sql.Append($" left join [{RS_dtb}].[dbo].[provoz] t2 on t2.zakaznikid = t1.id");
            if (Umisteni == null)
            {
                sql.Append($" left join [{RS_dtb}].[dbo].[umisteni] t3 on t3.provozid = t2.id and t3.SamostatnaRevize = 'false' ");
            }
            else
            {
                sql.Append($" left join[{RS_dtb}].[dbo].[umisteni] t3 on t3.provozid = t2.id and t3.SamostatnaRevize = 'true'");
            }
            sql.Append($" left join [{RS_dtb}].[dbo].[scprovozu] t4 on t4.provozid = t2.id and t4.umisteni = t3.id");
            sql.Append($" left join [{RS_dtb}].[dbo].[SerioveCislo] t5 on t5.Id = t4.SerioveCisloId");
            sql.Append($" left join [{RS_dtb}].[dbo].[Artikl] t6 on t5.ArtiklId = T6.Id");
            sql.Append(" where  t3.id is not null and t4.id is not null");
            sql.Append($" and T2.Id = {Provoz}");
            if (Umisteni != null)
            {
                sql.Append($" and T3.Id = {Umisteni}");
            }
            //LOGOVANI
            log.Debug($"FindScForRevision Provoz:{Provoz}, Umisteni: {Umisteni}, Rok: {Rok}, Polol: {Polol} ");
            log.Debug(sql.ToString());

            SqlConnection cnn = new SqlConnection(conn);
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
                    SCList item = new SCList();
                    try
                    {
                        item.Id = dr.GetInt32(dr.GetOrdinal("id"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug($"id prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Revize = dr.GetInt32(dr.GetOrdinal("Revize"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug($"Revize prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Pyro = dr.GetInt32(dr.GetOrdinal("Pyro"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug($"Pyro prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Baterie = dr.GetInt32(dr.GetOrdinal("Baterie"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug($"Baterie prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.TlkZk = dr.GetInt32(dr.GetOrdinal("TlkZk"));
                    }
                    catch (Exception ex)
                    {
                        log.Debug($"TlkZk prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }

                    listplanrev.Add(item);
                }
            }
            cnn.Close();

            return listplanrev;
        }


        public static List<SCProvozu> AddItemsFromList(List<SCList> sclist, int revize)
        {
            List<SCProvozu> list = new List<SCProvozu>();
            SCProvozu sc = new SCProvozu();
            foreach (var item in sclist)
            {
                using (var dbCtx = new Model1Container())
                {
                    if (item.Revize == 1)
                    {
                        sc = dbCtx.SCProvozu.Where(r => r.Id == item.Id).FirstOrDefault();
                        var exist = dbCtx.RevizeSC.Where(r => r.RevizeId == revize && r.SCProvozuId == sc.Id).Count();
                        if (exist == 0)
                        {
                            RevizeSC revizesc = new RevizeSC
                            {
                                UmisteniId = sc.Umisteni ,
                                RevizeId = revize,
                                SCProvozuId = sc.Id
                            };
                            
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
}