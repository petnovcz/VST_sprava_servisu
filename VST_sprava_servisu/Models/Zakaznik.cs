using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Zakaznik
    {
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
    }

    public partial class ZakaznikForm
    {
        public int? Skupina { get; set; }
        public string Search { get; set; }
        public IEnumerable<Zakaznik> ZakaznikList {get; set;}


    }

    
}
