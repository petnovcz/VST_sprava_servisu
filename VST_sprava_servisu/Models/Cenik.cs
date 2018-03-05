using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace VST_sprava_servisu.Models
{
    public partial class Cenik
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Cenik");

        public int ZakaznikId { get; set; }
        public Zakaznik Zakaznik {
            get
            {
                Zakaznik zakaznik = new Zakaznik();
                zakaznik = Zakaznik.GetById(ZakaznikId);
                return zakaznik;
            }

        }
        public string Mena {
        get
            {
                string currency = "";
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select Currency from ocrd where ");
                sql.Append($" CardCode = '{Zakaznik.KodSAP}' ");

                log.Debug($"Nacteni meny {sql.ToString()}");
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

                        try
                        {
                            currency = dr.GetString(dr.GetOrdinal("Currency"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        
                    }
                }
                cnn.Close();
                return currency;
            }
        }
        
        public int SAPCenik {
            get
            {
                int pricelist = 0;
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select ListNum from ocrd where ");
                sql.Append($" CardCode = '{Zakaznik.KodSAP}' ");

                log.Debug($"Nacteni ceniku {sql.ToString()}");
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

                        
                        try
                        {
                            pricelist = dr.GetInt16(dr.GetOrdinal("ListNum"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    }
                }
                cnn.Close();
                return pricelist;
            }
        }
        public List<CenikRow> List {
            get
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                List<CenikRow> list = new List<CenikRow>();              
                StringBuilder sql = new StringBuilder();
                sql.Append(" select (select Id from[Servis_test].[dbo].[Artikl] x where x.KodSAP COLLATE DATABASE_DEFAULT = T0.ItemCode COLLATE DATABASE_DEFAULT) as 'ArtiklID',");
                sql.Append(" t1.ItemCode, t1.Price as 'CenikCena' , t1.Currency as 'CenikMena', t3.Price as 'ZCCena', t3.Currency as 'ZCMena' from itm1 t1");
                sql.Append(" left join OITM t0 on t0.ItemCode = t1.itemcode left join (");
                sql.Append(" select coalesce(tx1.price, tx0.price) as 'Price', coalesce(tx1.currency, tx0.currency) as 'Currency', tx0.ItemCode, tx0.CardCode from OSPP tx0 left join");
                sql.Append(" SPP1 tx1 on tx0.ItemCode = tx1.ItemCode and tx0.CardCode = tx1.CardCode");
                sql.Append($" where ((tx1.FromDate <= GETDATE() or tx1.FromDate is null) and (tx1.ToDate >= GETDATE() or tx1.ToDate is NULL)) and tx0.CardCode = '{Zakaznik.KodSAP}' ) t3 on t0.ItemCode = t3.ItemCode ");
                sql.Append($" where PriceList = {SAPCenik} and t0.ItmsGrpCod = 129");
                sql.Append(" and(select COUNT(*) from[Servis_test].[dbo].[Artikl] x where x.KodSAP COLLATE DATABASE_DEFAULT = T0.ItemCode COLLATE DATABASE_DEFAULT) > 0");

                SqlConnection cnn = new SqlConnection(connectionString);
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
                        CenikRow cenikrow = new CenikRow();
                        try
                        {
                            cenikrow.ArtiklID = dr.GetInt32(dr.GetOrdinal("ArtiklID"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {                            
                            cenikrow.CenikCena = dr.GetDecimal(dr.GetOrdinal("CenikCena"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            cenikrow.CenikMena = dr.GetString(dr.GetOrdinal("CenikMena"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            cenikrow.ZCCena = dr.GetDecimal(dr.GetOrdinal("ZCCena"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            cenikrow.ZCMena = dr.GetString(dr.GetOrdinal("ZCMena"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            cenikrow.ZvlastniCena = false;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                        list.Add(cenikrow);
                    }
                }
                cnn.Close();
                return list;
            }

        }




    }


    public partial class CenikRow
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("CenikRow");

        public int ArtiklID { get; set; }
        public Artikl Artikl {
            get
            {
                Artikl artikl = new Artikl();
                artikl = Artikl.GetArtiklById(ArtiklID);
                return artikl;
            }

        }
        public decimal CenikCena { get; set; }
        public string CenikMena { get; set; }
        public bool ZvlastniCena { get; set; }
        public decimal ZCCena { get; set; }
        public string ZCMena { get; set; }


    }

    public partial class CenaArtikluZakaznik
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("CenaArtikluZakaznik");
        public int ZakaznikId { get; set; }
        public Zakaznik Zakaznik {
            get
            {
                Zakaznik zakaznik = new Zakaznik();
                zakaznik = Zakaznik.GetById(ZakaznikId);
                return zakaznik;
            }
        }
        public int ArtiklID { get; set; }
        public Artikl Artikl
        {
            get
            {
                Artikl artikl = new Artikl();
                artikl = Artikl.GetArtiklById(ArtiklID);
                return artikl;
            }

        }
        public int Cenik {
            get
            {
                int pricelist = 0;
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select ListNum from ocrd where ");
                sql.Append($" CardCode = '{Zakaznik.KodSAP}' ");

                log.Debug($"Nacteni ceniku {sql.ToString()}");
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


                        try
                        {
                            pricelist = dr.GetInt16(dr.GetOrdinal("ListNum"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    }
                }
                cnn.Close();
                return pricelist;
            }
        }
        public decimal CenikCena { get; set; }
        public string CenikMena { get; set; }
        public bool ZvlastniCena { get; set; }
        public decimal ZCCena { get; set; }
        public string ZCMena { get; set; }

        internal protected static CenaArtikluZakaznik GetCena(int ArtiklId, int ZakaznikId)
        {
            CenaArtikluZakaznik cena = new CenaArtikluZakaznik();
            Artikl artikl = new Artikl();
            

            cena.ArtiklID = ArtiklId;
            cena.ZakaznikId = ZakaznikId;

            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            List<CenikRow> list = new List<CenikRow>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select (select Id from[Servis_test].[dbo].[Artikl] x where x.KodSAP COLLATE DATABASE_DEFAULT = T0.ItemCode COLLATE DATABASE_DEFAULT) as 'ArtiklID',");
            sql.Append(" t1.ItemCode, t1.Price as 'CenikCena' , t1.Currency as 'CenikMena', t3.Price as 'ZCCena', t3.Currency as 'ZCMena' from itm1 t1");
            sql.Append("  left join OITM t0 on t0.ItemCode = t1.itemcode left join (");
            sql.Append("  select coalesce(tx1.price, tx0.price) as 'Price', coalesce(tx1.currency, tx0.currency) as 'Currency', tx0.ItemCode, tx0.CardCode from OSPP tx0 left join");
            sql.Append(" SPP1 tx1 on tx0.ItemCode = tx1.ItemCode and tx0.CardCode = tx1.CardCode");
            sql.Append(" where ((tx1.FromDate <= GETDATE() or tx1.FromDate is null)");
            sql.Append(" and (tx1.ToDate >= GETDATE() or tx1.ToDate is NULL))");
            sql.Append($" and tx0.cardcode = '{cena.Zakaznik.KodSAP}' ) t3 on t0.ItemCode = t3.ItemCode");
            sql.Append($" where PriceList = {cena.Cenik} and t0.ItmsGrpCod = 129 and t0.ItemCode = '{cena.Artikl.KodSAP}'");
            
            SqlConnection cnn = new SqlConnection(connectionString);
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
                    CenikRow cenikrow = new CenikRow();
                    try
                    {
                        cena.ArtiklID = dr.GetInt32(dr.GetOrdinal("ArtiklID"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.CenikCena = dr.GetDecimal(dr.GetOrdinal("CenikCena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.CenikMena = dr.GetString(dr.GetOrdinal("CenikMena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.ZCCena = dr.GetDecimal(dr.GetOrdinal("ZCCena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.ZCMena = dr.GetString(dr.GetOrdinal("ZCMena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.ZvlastniCena = false;
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    
                }
            }
            cnn.Close();

            return cena;
        }

        internal protected static CenaArtikluZakaznik GetCena(string SAPKod, int ZakaznikId)
        {
            CenaArtikluZakaznik cena = new CenaArtikluZakaznik();
            Artikl artikl = new Artikl();
            artikl = Artikl.GetArtiklBySAP(SAPKod);

            cena.ArtiklID = artikl.Id;
            cena.ZakaznikId = ZakaznikId;

            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            List<CenikRow> list = new List<CenikRow>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select (select Id from[Servis_test].[dbo].[Artikl] x where x.KodSAP COLLATE DATABASE_DEFAULT = T0.ItemCode COLLATE DATABASE_DEFAULT) as 'ArtiklID',");
            sql.Append(" t1.ItemCode, t1.Price as 'CenikCena' , t1.Currency as 'CenikMena', t3.Price as 'ZCCena', t3.Currency as 'ZCMena' from itm1 t1");
            sql.Append("  left join OITM t0 on t0.ItemCode = t1.itemcode left join (");
            sql.Append("  select coalesce(tx1.price, tx0.price) as 'Price', coalesce(tx1.currency, tx0.currency) as 'Currency', tx0.ItemCode, tx0.CardCode from OSPP tx0 left join");
            sql.Append(" SPP1 tx1 on tx0.ItemCode = tx1.ItemCode and tx0.CardCode = tx1.CardCode");
            sql.Append(" where ((tx1.FromDate <= GETDATE() or tx1.FromDate is null)");
            sql.Append(" and (tx1.ToDate >= GETDATE() or tx1.ToDate is NULL))");
            sql.Append($" and tx0.cardcode = '{cena.Zakaznik.KodSAP}' ) t3 on t0.ItemCode = t3.ItemCode ");
            sql.Append($" where PriceList = {cena.Cenik} and t0.ItmsGrpCod = 129 and t0.ItemCode = '{cena.Artikl.KodSAP}'");

            SqlConnection cnn = new SqlConnection(connectionString);
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
                    CenikRow cenikrow = new CenikRow();
                    try
                    {
                        cenikrow.ArtiklID = dr.GetInt32(dr.GetOrdinal("ArtiklID"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.CenikCena = dr.GetDecimal(dr.GetOrdinal("CenikCena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.CenikMena = dr.GetString(dr.GetOrdinal("CenikMena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.ZCCena = dr.GetDecimal(dr.GetOrdinal("ZCCena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.ZCMena = dr.GetString(dr.GetOrdinal("ZCMena"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    try
                    {
                        cena.ZvlastniCena = false;
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    
                }
            }
            cnn.Close();

            return cena;
        }


    }
}