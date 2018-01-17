using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class SCProvozu
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SCProvozu");

        internal protected static List<SCProvozu> GetList(int? Provoz, int? SerioveCislo, int? Status, int? Umisteni)
        {
            var list = new List<SCProvozu>();
            using (var dbCtx = new Model1Container())
            {
                var listp = dbCtx.SCProvozu.AsQueryable();
                if (Provoz != null)
                {
                    listp = listp.Where(l => l.ProvozId == Provoz);
                }
                if (SerioveCislo != null)
                {
                    listp = listp.Where(l => l.SerioveCisloId == SerioveCislo);
                }
                if (Status != null)
                {
                    listp = listp.Where(l => l.StatusId == Status);
                }
                if (Umisteni != null)
                {
                    listp = listp.Where(l => l.Umisteni == Umisteni);
                }
                list = listp.ToList();
            }
            return list;
        }

        internal protected static void UpdateSC(int id, DateTime datumkontroly, bool Baterie, bool Pyro, bool TlakovaZkouska)
        {
            using (var dbCtx = new Model1Container())
            {
                var sc = dbCtx.SCProvozu.Find(id);
                sc.DatumRevize = datumkontroly;
                sc.DatumPosledniZmeny = datumkontroly;
                if (Baterie == true) { sc.DatumBaterie = datumkontroly; }
                if (Pyro == true) { sc.DatumPyro = datumkontroly; }
                if (TlakovaZkouska == true) { sc.DatumTlkZk = datumkontroly; }

                try
                {

                    dbCtx.Entry(sc).State = EntityState.Modified;
                    dbCtx.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            }
        }
        public static int AddSCProvozu(SCImport scimport, int id)
        {
            int idscprovozu = 0;
            SCProvozu scprovozu = new SCProvozu();
            scprovozu.SerioveCisloId = id;
            scprovozu.ProvozId = scimport.Provozy;
            using (var dbCtx = new Model1Container())
            {
                scprovozu.StatusId = dbCtx.Status.Where(s => s.Aktivni == true).Select(s => s.Id).FirstOrDefault();
            }
            scprovozu.DatumPrirazeni = scimport.DatumDodani;
            if (scimport.DatumPosledniZmeny == null) { scprovozu.DatumPosledniZmeny = scimport.DatumRevize; }
            else { scprovozu.DatumPosledniZmeny = scimport.DatumPosledniZmeny; }
            scprovozu.DatumVymeny = null;
            scprovozu.Umisteni = scimport.Umisteni;
            scprovozu.DatumRevize = scimport.DatumRevize;
            scprovozu.DatumBaterie = scimport.DatumBaterie;
            scprovozu.DatumPyro = scimport.DatumPyro;
            scprovozu.DatumTlkZk = scimport.DatumTlkZk;
            scprovozu.Lokace = scimport.Lokace;
            scprovozu.Znaceni = scimport.Znaceni;
            scprovozu.Baterie = scimport.Baterie;
            scprovozu.Proverit = scimport.Proverit;
            scprovozu.BaterieArtikl = scimport.BaterieArtikl;
            using (var dbCtx = new Model1Container())
            {
                try
                {
                    dbCtx.SCProvozu.Add(scprovozu);
                    dbCtx.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
            }
            idscprovozu = scprovozu.Id;
            return idscprovozu;

        }
    }
}