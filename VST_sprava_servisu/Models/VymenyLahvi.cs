using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class VymenyLahvi
    {

        internal protected static VymenyLahvi GenerujVymenu(SCProvozu oldSCProvozu, SCProvozu newSCProvozu, DateTime DatumVymeny)
        {
            VymenyLahvi vl = new VymenyLahvi();
            using (var dbCtx = new Model1Container())
            {
                vl.SCProvozuNova = newSCProvozu.Id;
                vl.SCProvozuPuvodni = oldSCProvozu.Id;
                vl.DatumVymeny = DatumVymeny;
                vl.SCLahve = "fdsfsd";
                try
                {
                    dbCtx.VymenyLahvi.Add(vl);
                    dbCtx.SaveChanges();
                }
                catch { }

            }
            return vl;
        }
    }
}