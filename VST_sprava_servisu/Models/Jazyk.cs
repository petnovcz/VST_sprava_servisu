using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Jazyk
    {
        public static bool ValidateValue(int ID)
        {
            bool success = false;
            using (var dbCtx = new Model1Container())
            {
                var count = dbCtx.Jazyk.Where(j => j.Id == ID).Count();
                if (count > 0) { success = true; }


            }
            return success;
        }
    }
}