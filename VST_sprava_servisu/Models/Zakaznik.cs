using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public partial class Zakaznik
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Zakaznik");

        internal protected static List<Zakaznik> GetAll()
            {
                var zakaznikl = new List<Zakaznik>();
                using (var dbCtx = new Model1Container())
                {
                    zakaznikl = dbCtx.Zakaznik.ToList();
                }
                return zakaznikl;
            }
            internal protected static Zakaznik GetById(int Id)
            {
                var zakaznik = new Zakaznik();
                using (var dbCtx = new Model1Container())
                {
                    zakaznik = dbCtx.Zakaznik.Where(r => r.Id == Id).FirstOrDefault();
                }
                return zakaznik;
            }
            internal protected static List<Zakaznik> GetByName(string Search)
            {
                var zakaznikl = new List<Zakaznik>();
                using (var dbCtx = new Model1Container())
                {
                    zakaznikl = dbCtx.Zakaznik.Where(r => r.NazevZakaznika.Contains(Search)).ToList();
                }
                return zakaznikl;
            }

        [Authorize(Roles = "Administrator,Manager")]
        public static bool CreateFromSAPdata(SAPOP sapOP)
        {
            using (var dbCtx = new Model1Container())
            {

                Zakaznik zakaznik = new Zakaznik();
                zakaznik.KodSAP = sapOP.CardCode;
                zakaznik.NazevZakaznika = sapOP.CardName;
                zakaznik.Adresa = (sapOP.Address + ", " + sapOP.City + ", " + sapOP.ZipCode + ", " + sapOP.Country);
                zakaznik.DIC = sapOP.LicTradNum;
                zakaznik.IC = sapOP.VatIdUnCmp;
                zakaznik.JazykId = sapOP.JazykId;
                zakaznik.RegionId = sapOP.RegionId;
                zakaznik.Telefon = sapOP.Phone;
                zakaznik.Email = sapOP.Email;
                zakaznik.Kontakt = "d";
                try
                {
                    dbCtx.Zakaznik.Add(zakaznik);
                    dbCtx.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            }




            return true;
        }
    }


    public partial class ZakaznikForm
    {
        public int? Skupina { get; set; }
        public string Search { get; set; }
        public IEnumerable<Zakaznik> ZakaznikList {get; set;}


    }


}
