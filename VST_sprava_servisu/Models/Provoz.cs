using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public partial class Provoz
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Provoz");

        internal protected static List<Provoz> GetAll()
        {
            var provozl = new List<Provoz>();
            using (var dbCtx = new Model1Container())
            {
                provozl = dbCtx.Provoz.ToList();
            }
            return provozl;
        }
        internal protected static Provoz GetById(int Id)
        {
            var provoz = new Provoz();
            using (var dbCtx = new Model1Container())
            {
                provoz = dbCtx.Provoz.Where(r => r.Id == Id).FirstOrDefault();
                provoz.Zakaznik = dbCtx.Zakaznik.Where(r => r.Id == provoz.ZakaznikId).FirstOrDefault();
            }
            return provoz;
        }
        internal protected static List<Provoz> GetByName(string Search)
        {
            var provozl = new List<Provoz>();
            using (var dbCtx = new Model1Container())
            {
                provozl = dbCtx.Provoz.Where(r => r.NazevProvozu.Contains(Search)).ToList();
            }
            return provozl;
        }

        [Authorize(Roles = "Administrator,Manager")]
        public static bool Generate(string Address, string CardCode, string Street, string ZipCode, string City, string Country, int Zakaznik)
        {
            Provoz provoz = new Provoz();
            provoz.ZakaznikId = Zakaznik;
            provoz.NazevProvozu = Address;
            provoz.SAPAddress = Address;
            provoz.AdresaProvozu = Street + ", " + ZipCode + ", " + City + ", " + Country;
            using (var dbCtx = new Model1Container())
            {
                
                    try
                    {
                        dbCtx.Provoz.Add(provoz);
                        dbCtx.SaveChanges();
                    }
                    catch (SqlException e)
                    {
                        log.Error("Error number: " + e.Number + " - " + e.Message);
                    }

                
            }
            return true;
        }
    }
}
