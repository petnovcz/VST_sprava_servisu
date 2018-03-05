using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu
{
    public partial class ServisniZasah
    {

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
            if (caz.ZCCena != null) { cena = caz.ZCCena; } else { cena = caz.CenikCena; }
            return cena;
        }

        internal protected static void UpdateHeader(int Id)
        {
            ServisniZasah sz = new ServisniZasah();
            using (var db = new Model1Container())
            {
                sz = db.ServisniZasah.Where(t => t.Id == Id).FirstOrDefault();
                var x = db.ServisniZasahPrvek.Where(t => t.ServisniZasahId == Id).Select(t => t.CenaCelkem).Sum();
                sz.Celkem = sz.CestaCelkem + sz.PraceCelkem + x;
                db.Entry(sz).State = EntityState.Modified;
                db.SaveChanges();
            }



        }

    }
}