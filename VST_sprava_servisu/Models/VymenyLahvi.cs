﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class VymenyLahvi
    {

        internal protected static VymenyLahvi GenerujVymenu(SCProvozu oldSCProvozu, SCProvozu newSCProvozu, DateTime DatumVymeny, int RevizeId, string SClahve)
        {
            VymenyLahvi vl = new VymenyLahvi();
            using (var dbCtx = new Model1Container())
            {
                vl.SCProvozuNova = newSCProvozu.Id;
                vl.SCProvozuPuvodni = oldSCProvozu.Id;
                vl.DatumVymeny = DatumVymeny;
                vl.Revize = RevizeId;
                vl.SCLahve = SClahve;
                try
                {
                    dbCtx.VymenyLahvi.Add(vl);
                    dbCtx.SaveChanges();
                }
                catch(Exception ex) {  }

            }
            return vl;
        }
    }
}