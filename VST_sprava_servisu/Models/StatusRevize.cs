using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class StatusRevize
    {

        public static int Planned()
        {
            int id = 0;
            using (var dbCtx = new Model1Container())
            {
                id = dbCtx.StatusRevize.Where(s => s.Planovana == true).Select(s=>s.Id).FirstOrDefault();
            }
            return id;
        }
        public static int Confirmed()
        {
            int id = 0;
            using (var dbCtx = new Model1Container())
            {
                id = dbCtx.StatusRevize.Where(s => s.Potvrzena == true).Select(s => s.Id).FirstOrDefault();
            }
            return id;
        }
        public static int Realised()
        {
            int id = 0;
            using (var dbCtx = new Model1Container())
            {
                id = dbCtx.StatusRevize.Where(s => s.Realizovana == true).Select(s => s.Id).FirstOrDefault();
            }
            return id;
        }
        public static int Storno()
        {
            int id = 0;
            using (var dbCtx = new Model1Container())
            {
                id = dbCtx.StatusRevize.Where(s => s.Stornovana == true).Select(s => s.Id).FirstOrDefault();
            }
            return id;
        }

    }
}