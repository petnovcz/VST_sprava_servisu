using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class SerioveCislo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SerioveCislo");

        public static int AddSeriovecislo(SCImport scimport)
        {
            int id = 0;
            SerioveCislo seriovecislo = new SerioveCislo();
            seriovecislo.ArtiklId = scimport.ArtiklId;
            seriovecislo.DatumPosledniTlakoveZkousky = scimport.DatumTlkZk;
            seriovecislo.DatumVyroby = scimport.DatumVyroby;
            seriovecislo.SerioveCislo1 = scimport.SerioveCislo;
            using (var dbCtx = new Model1Container())
            {
                try
                {
                    dbCtx.SerioveCislo.Add(seriovecislo);
                    dbCtx.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                id = seriovecislo.Id;
            }
            return id;

        }

        public static SerioveCislo GetSerioveCisloById(int SC)
        {
            SerioveCislo serioveCislo = new SerioveCislo();
            using (var dbCtx = new Model1Container())
            {
                serioveCislo = dbCtx.SerioveCislo.Where(r => r.Id == SC).FirstOrDefault();
                serioveCislo.Artikl = dbCtx.Artikl.Where(r => r.Id == serioveCislo.ArtiklId).FirstOrDefault();

            }

            return serioveCislo;
        }

    }
}