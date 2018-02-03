using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Region
    {

        public static bool ValidateValue(int ID)
        {
            bool success = false;
            using (var dbCtx = new Model1Container())
            {
                var count = dbCtx.Region.Where(j => j.Id == ID).Count();
                if (count > 0) { success = true; }


            }
            return success;
        }

        internal protected static List<Region> GetAll()
        {
            var regionl = new List<Region>();
            using (var dbCtx = new Model1Container())
            {
                regionl = dbCtx.Region.ToList();
            }
            return regionl;
        }
        internal protected static Region GetById(int Id)
        {
            var region = new Region();
            using (var dbCtx = new Model1Container())
            {
                region = dbCtx.Region.Where(r=>r.Id == Id).FirstOrDefault();
            }
            return region;
        }
        internal protected static List<Region> GetByName(string Search)
        {
            var regionl = new List<Region>();
            using (var dbCtx = new Model1Container())
            {
                regionl = dbCtx.Region.Where(r=>r.NazevRegionu.Contains(Search)).ToList();
            }
            return regionl;
        }
        
    }
}