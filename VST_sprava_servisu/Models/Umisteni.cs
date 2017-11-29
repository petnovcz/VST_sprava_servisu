using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Umisteni
    {
        internal protected static List<Umisteni> GetAll()
        {
            var umistenil = new List<Umisteni>();
            using (var dbCtx = new Model1Container())
            {
                umistenil = dbCtx.Umisteni.ToList();
            }
            return umistenil;
        }
        internal protected static Umisteni GetById(int Id)
        {
            var umisteni = new Umisteni();
            using (var dbCtx = new Model1Container())
            {
                umisteni = dbCtx.Umisteni.Where(r => r.Id == Id).FirstOrDefault();
            }
            return umisteni;
        }
        internal protected static List<Umisteni> GetByName(string Search)
        {
            var umistenil = new List<Umisteni>();
            using (var dbCtx = new Model1Container())
            {
                umistenil = dbCtx.Umisteni.Where(r => r.NazevUmisteni.Contains(Search)).ToList();
            }
            return umistenil;
        }
    }
}
