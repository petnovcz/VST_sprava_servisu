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

    }
}