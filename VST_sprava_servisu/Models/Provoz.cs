using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Provoz
    {
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
    }
}
