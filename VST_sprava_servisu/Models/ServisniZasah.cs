﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu
{
    public partial class ServisniZasah
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ServisniZasah");

        public PoruchaList Poruchy { get; set;
            
        }

        public static PoruchaList Recalculcateporuchy(int Id)
        {
            PoruchaList item = new PoruchaList();
            using (var db = new Model1Container())
            {
                item.SeznamPoruch = db.Porucha.Where(t => t.SkupinaArtikluId == null).ToList();
                item.VybranaPorucha = db.ServisniZasahPrvek.Include(t => t.Porucha).Where(t => t.ServisniZasahId == Id && t.Porucha.SkupinaArtikluId == null).Select(t => t.Porucha).FirstOrDefault();
                item.ServisniZasahId = Id;
            }
            return item;
        }

        public List<ServisniZasahPrvek> Artikly
        {
            get
            {
                List<ServisniZasahPrvek> item = new List<ServisniZasahPrvek>();
                using (var db = new Model1Container())
                {
                    item = db.ServisniZasahPrvek.Where(t => t.Porucha.SkupinaArtikluId != null && t.ServisniZasahId == Id).ToList();
                }
                return item;

            }
        }



        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"),Display(Name="Konec záruky")]
        public DateTime? DatumVyprseniZaruky {
            get
            {
                DateTime? ukonceniZaruky = null;
                using (var db = new Model1Container())
                {
                    ukonceniZaruky = db.Umisteni
                        .Where(t=>t.Id == UmisteniId)
                        .Select(t=>t.UkonceniZaruky)
                        .FirstOrDefault();


                }



                    return ukonceniZaruky;
            }



        }

        internal protected static string GetProjekt(int ZakaznikId)
        {
            string projekt = "";


            return projekt;
        }


        internal protected static string GetCurrency(int ZakaznikId)
        {
            
            string currency = "";
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();
            var zakaznik = Zakaznik.GetById(ZakaznikId);
            sql.Append(" select Currency from ocrd where ");
            sql.Append($" CardCode = '{zakaznik.KodSAP}' ");

            log.Debug($"Nacteni meny {sql.ToString()}");
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

                    try
                    {
                        currency = dr.GetString(dr.GetOrdinal("Currency"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                }
            }
            cnn.Close();
            if (currency == "##")
            {


            }


            return currency;


            
        }

        internal protected static decimal GetDistance(string Origin, string Kam, string Zpet)
        {

            

            float km1 = 0;
            float km2 = 0;
            string url = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + Origin + "&destinations=" + Kam + "&sensor=false";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();

            DataSet ds = new DataSet();
            ds.ReadXml(new XmlTextReader(new StringReader(responsereader)));
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables["element"].Rows[0]["status"].ToString() == "OK")
                {
                    string x = ds.Tables["distance"].Rows[0]["text"].ToString();
                    var cx = x.Substring(0, x.Length - 3).Replace(".", ",");

                    km1 = float.Parse(cx);

                }
            }

            string url2 = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + Kam + "&destinations=" + Zpet + "&sensor=false";

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url2);
            WebResponse response2 = request2.GetResponse();
            Stream dataStream2 = response2.GetResponseStream();
            StreamReader sreader2 = new StreamReader(dataStream2);
            string responsereader2 = sreader2.ReadToEnd();
            response2.Close();

            DataSet ds2 = new DataSet();
            ds2.ReadXml(new XmlTextReader(new StringReader(responsereader2)));
            if (ds2.Tables.Count > 0)
            {
                if (ds2.Tables["element"].Rows[0]["status"].ToString() == "OK")
                {

                    string x = ds2.Tables["distance"].Rows[0]["text"].ToString();
                    var cx = x.Substring(0, x.Length - 3).Replace(".", ",");
                    km2 = float.Parse(cx);
                }
            }
            var celkem = km1 + km2;
            var doubl = Math.Round(celkem);
            decimal total = Int32.Parse(doubl.ToString());
            return total;
        }

        internal protected static decimal GetCenaForprvek(ServisniZasahPrvek szp)
        {
            decimal cena;
            ServisniZasah sz = new ServisniZasah();

            using (var db = new Model1Container())
            {
                sz = db.ServisniZasah.Where(t => t.Id == szp.ServisniZasahId).FirstOrDefault();
            }
            CenaArtikluZakaznik caz = new CenaArtikluZakaznik();
            caz = CenaArtikluZakaznik.GetCena(szp.ArtiklID.Value, sz.ZakaznikID);
            if (caz.ZCCena != 0) { cena = caz.ZCCena; } else { cena = caz.CenikCena; }
            return cena;
        }

        internal protected static string GetCurrencyForprvek(int szp)
        {
            string currency;
            ServisniZasah sz = new ServisniZasah();

            using (var db = new Model1Container())
            {
                sz = db.ServisniZasah.Where(t => t.Id == szp).FirstOrDefault();
            }
            CenaArtikluZakaznik caz = new CenaArtikluZakaznik();
            caz = CenaArtikluZakaznik.GetCena(215, sz.ZakaznikID);
            if (caz.ZCMena == "") { currency = caz.ZCMena; } else { currency = caz.CenikMena; }
            return currency;
        }

        internal protected static void UpdateHeader(int Id)
        {
            ServisniZasah sz = new ServisniZasah();
            using (var db = new Model1Container())
            {
                sz = db.ServisniZasah.Where(t => t.Id == Id).FirstOrDefault();
                //načtení ceny za dopravu
                var km = CenaArtikluZakaznik.GetCena("SP02", sz.ZakaznikID);
                decimal kmcena;
                //pokud není nastavena zvláštní cena tak potom ceníková
                if (km.ZCCena != 0)
                {
                    kmcena = km.ZCCena;
                } else
                {
                    kmcena = km.CenikCena;
                }
                //výpočet ceny celkem za dopravu
                sz.CestaCelkem = sz.Km * kmcena;
                //načtení ceny za práci servisních techniků
                var prace = CenaArtikluZakaznik.GetCena("SP01", sz.ZakaznikID);
                decimal pracecena;
                // pokud není nastavena zvláštní cena tak potom ceníková
                if (prace.ZCCena != 0)
                {
                    pracecena = prace.ZCCena;
                }
                else
                {
                    pracecena = prace.CenikCena;
                }
                sz.PraceSazba = pracecena;
                //výpočet celkové ceny za práci
                sz.PraceCelkem = sz.Pracelidi * sz.PraceSazba * sz.PraceHod;
                //výpočet počtu prvků na servisním zásahu
                var prvku = db.ServisniZasahPrvek.Where(t=>t.ServisniZasahId == sz.Id).Count();
        

                var reklamprvku = db.ServisniZasahPrvek.Where(t => t.Reklamace == true && t.ServisniZasahId == sz.Id).Count();

                var poruseni = db.ServisniZasahPrvek.Where(t => t.Reklamace == true && t.PoruseniZarucnichPodminek == true && t.ServisniZasahId == sz.Id).Count();

                
                var x = db.ServisniZasahPrvek.Where(t => t.ServisniZasahId == Id)
                    .Where(t => t.Reklamace == true && t.PoruseniZarucnichPodminek == true || t.Reklamace == false)
                    .Select(t => t.CenaCelkem)
                    .Sum();

                if (x == null) { x = 0; }
                if (sz.Reklamace == false || (sz.Reklamace == true && sz.PoruseniZarucnichPodminek == true))
                {
                    sz.Celkem = sz.CestaCelkem + sz.PraceCelkem + x;
                }
                else
                {
                    sz.Celkem = x;
                }

                sz.Mena = GetCurrencyForprvek(sz.Id);
                try
                {
                    db.Entry(sz).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);  };
            }



        }

        internal protected static ServisniZasah GetZasah(int Id)
        {
            ServisniZasah sz = new ServisniZasah();

            using (var db = new Model1Container())
            {
                sz = db.ServisniZasah.Where(t => t.Id == Id)
                    .Include(t => t.ServisniZasahPrvek)
                    .Include(x => x.ServisniZasahPrvek.Select(y => y.ServisniZasahPrvekSerioveCislo))
                    .Include(x => x.ServisniZasahPrvek.Select(y=>y.Artikl))
                    //.Include(x => x.ServisniZasahPrvek.Select(y => y.RadkyKusovniku))
                    .Include(t=>t.Zakaznik)
                    
                    .FirstOrDefault();

            }
            sz.Poruchy = ServisniZasah.Recalculcateporuchy(sz.Id);
            return sz;
        }

        internal protected static void UpdateQuotation(string DocEntry, int ServisniZasahId)
        {

            int currency = 0 ;

            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();
            
            sql.Append(" select DocEntry, Docnum from ODRF where ");
            sql.Append($" DocEntry = '{DocEntry}' and ObjType = 23");

            log.Debug($"Nacteni meny {sql.ToString()}");
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

                    try
                    {
                        currency = dr.GetInt32(dr.GetOrdinal("DocNum"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                }
            }
            cnn.Close();

            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(ServisniZasahId);
            sz.Nabidka = DocEntry;
            sz.NabidkaDocNum = currency.ToString();

            using (var db = new Model1Container())
            {
                try
                {
                    db.Entry(sz).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
            }

            


        }

        internal protected static void UpdateOrder(string DocEntry, int ServisniZasahId)
        {

            int currency = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" select DocEntry, Docnum from ordr where ");
            sql.Append($" DocEntry = '{DocEntry}' ");

            log.Debug($"Nacteni meny {sql.ToString()}");
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

                    try
                    {
                        currency = dr.GetInt32(dr.GetOrdinal("DocNum"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                }
            }
            cnn.Close();

            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(ServisniZasahId);
            sz.Zakazka = DocEntry;
            sz.ZakazkaDocNUm = currency.ToString();

            using (var db = new Model1Container())
            {
                db.Entry(sz).State = EntityState.Modified;
                db.SaveChanges();
            }




        }

        internal protected static void UpdateDelivery(string DocEntry, int ServisniZasahId)
        {

            int currency = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" select DocEntry, Docnum from ODRF where objtype = 15 and ");
            sql.Append($" DocEntry = '{DocEntry}' ");

            log.Debug($"Nacteni meny {sql.ToString()}");
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

                    try
                    {
                        currency = dr.GetInt32(dr.GetOrdinal("DocNum"));
                    }
                    catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                }
            }
            cnn.Close();

            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(ServisniZasahId);
            sz.DodaciList = DocEntry;
            sz.DodaciListDocNum = currency.ToString();

            using (var db = new Model1Container())
            {
                db.Entry(sz).State = EntityState.Modified;
                db.SaveChanges();
            }




        }
    }
}