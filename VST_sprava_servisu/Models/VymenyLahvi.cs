using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class VymenyLahvi
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("VymenyLahvi");

        internal protected static VymenyLahvi GenerujVymenu(SCProvozu oldSCProvozu, SCProvozu newSCProvozu, DateTime DatumVymeny, int RevizeId, string SClahve)
        {
            VymenyLahvi vl = new VymenyLahvi();
            using (var dbCtx = new Model1Container())
            {
                vl.SCProvozuNova = newSCProvozu.Id;
                vl.SCProvozuPuvodni = oldSCProvozu.Id;
                vl.DatumVymeny = DatumVymeny;
                vl.Revize = RevizeId;
                vl.Umisteni = oldSCProvozu.Umisteni;
                vl.SCLahve = SClahve;
                if (SClahve.Trim().StartsWith("R"))
                {
                    vl.Repase = true;
                    vl.Popis = "viz. přiložený atest";
                }
                else
                {
                    vl.Repase = false;
                    vl.Popis = "viz. přiložené prohlášení o shodě";
                }
                
                try
                {
                    dbCtx.VymenyLahvi.Add(vl);
                    dbCtx.SaveChanges();
                }
                catch(Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            }
            return vl;
        }
    }
}