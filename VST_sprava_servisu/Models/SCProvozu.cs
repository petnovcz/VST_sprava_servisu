using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Data;

namespace VST_sprava_servisu
{
    public partial class SCProvozu
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SCProvozu");

        public bool OpenRevize {
            get
            {
                bool result = false;
                using (var dbCtx = new Model1Container())
                {
                    var x = dbCtx.RevizeSC.Include(r => r.Revize).Where(r => r.SCProvozuId == Id).Where(r => r.Revize.StatusRevize.Planovana == true || r.Revize.StatusRevize.Potvrzena == true).Count();
                    if (x > 0) { result = true; } else { result = false; }
                }
                return result;
            }

        }

        internal protected static DateTime? GetDatumZaruky(int Id)
        {
            DateTime? datum;
            using (var dbCtx = new Model1Container())
            {
                datum = dbCtx.SCProvozu.Where(r => r.Id == Id).Select(r => r.UkonceniZaruky).FirstOrDefault();
            }
            return datum;
        }




        internal protected static void ZneaktivniSCProvozu(SCProvozu oldSCProvozu, DateTime DatumRevize)
        {
            SCProvozu sc = new SCProvozu();

            using (var dbCtx = new Model1Container())
            {
                sc = dbCtx.SCProvozu.Find(oldSCProvozu.Id);
                sc.StatusId = dbCtx.Status.Where(r => r.Neaktivni == true).Select(r => r.Id).FirstOrDefault();
                sc.DatumVymeny = DatumRevize;
                sc.DatumPosledniZmeny = DatumRevize;
                try
                {

                    dbCtx.Entry(sc).State = EntityState.Modified;
                    dbCtx.SaveChanges();
                }
                catch { }


            }
        }


        internal protected static SCProvozu GetSCProvozuById(int SCProvozuId)
        {
            SCProvozu sCProvozu = new SCProvozu();
            using (var dbCtx = new Model1Container())
            {
                sCProvozu = dbCtx.SCProvozu.Where(r => r.Id == SCProvozuId).Include(r=>r.SerioveCislo).Include(r=>r.Artikl).FirstOrDefault();
                if (sCProvozu.SerioveCislo.ArtiklId != null)
                {
                    sCProvozu.Artikl = dbCtx.Artikl.Where(a => a.Id == sCProvozu.SerioveCislo.ArtiklId).FirstOrDefault();
                }
            }

            return sCProvozu;
        }

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
                if (sc.DatumRevize <= datumkontroly)
                {
                    sc.DatumRevize = datumkontroly;
                }
                if (sc.DatumPosledniZmeny <= datumkontroly)
                {
                    sc.DatumPosledniZmeny = datumkontroly;
                }
                if (Baterie == true && sc.DatumBaterie <= datumkontroly)
                {
                    sc.DatumBaterie = datumkontroly;
                }
                if (Pyro == true && sc.DatumPyro <= datumkontroly)
                {
                    sc.DatumPyro = datumkontroly;
                }
                if (TlakovaZkouska == true && sc.DatumTlkZk <= datumkontroly)
                {
                    sc.DatumTlkZk = datumkontroly;
                }
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
            SCProvozu scprovozu = new SCProvozu { SerioveCisloId = id, ProvozId = scimport.Provozy };
            
            using (var dbCtx = new Model1Container())
            {
                scprovozu.StatusId = dbCtx.Status.Where(s => s.Aktivni == true).Select(s => s.Id).FirstOrDefault();
            }
            scprovozu.DatumPrirazeni = scimport.DatumDodani;
            if (scimport.DatumPosledniZmeny == null)
            {
                scprovozu.DatumPosledniZmeny = scimport.DatumRevize;
            }
            else
            {
                scprovozu.DatumPosledniZmeny = scimport.DatumPosledniZmeny;
            }
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
            scprovozu.UpravenaPeriodaRevize = scimport.UpravenaPeriodaRevize;
            scprovozu.UpravenaPeriodaBaterie = scimport.UpravenaPeriodaBaterie;
            scprovozu.UpravenaPeriodaPyro = scimport.UpravenaPeriodaPyro;
            scprovozu.UpravenaPeriodaTlkZk = scimport.UpravenaPeriodaTlkZk;
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