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
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    
                }

            }
        }
    }
}