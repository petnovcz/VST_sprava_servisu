using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace VST_sprava_servisu
{
    public partial class Revize
    {
        [NotMapped]
        private Zakaznik Zakaznik { get; set; }
        

        
        internal protected void UpdateRevizeHeader (int id)
        {
            using (var dbCtx = new Model1Container())
            {
                Revize revize = new Revize();
                revize = dbCtx.Revize.Find(id);
                revize.Baterie = dbCtx.RevizeSC.Where(r=>r.RevizeId == id && r.Baterie == true).Count();
                revize.Pyro = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.Pyro == true).Count();
                revize.TlkZk = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.TlakovaZkouska == true).Count();
                revize.AP = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 131).Count();
                revize.S = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 1).Count();
                revize.RJ = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 130).Count();
                revize.V = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id >= 132 && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id <= 135).Count();
                revize.M = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 136 ).Count();
                try
                {
                    dbCtx.Entry(revize).State = EntityState.Modified;
                    dbCtx.SaveChanges();
                }
                catch
                {

                }

                
            }
            
        }



        /// <summary>
        /// Vrací prostý seznam všech revizí
        /// </summary>
        /// <returns></returns>
        internal protected static List<Revize> GetAll()
        {
            var provozl = new List<Revize>();
            using (var dbCtx = new Model1Container())
            {
                provozl = dbCtx.Revize.ToList();
            }
            return provozl;
        }
    
        /// <summary>
        /// Vrací Revizi dohledanou dle ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal protected static Revize GetById(int Id)
        {
            var provoz = new Revize();
            using (var dbCtx = new Model1Container())
            {
                provoz = dbCtx.Revize.Where(r => r.Id == Id).FirstOrDefault();
            }
            return provoz;
        }
        
        /// <summary>
        /// Metoda, ktera na zaklade zakaznika, provozu, roku a pololeti dohleda zda existuje revize a vraci bool zda existuje
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        internal protected static bool ExistRevision(int Zakaznik, int Provoz, int Rok, int Pololeti, int? Umisteni)
        {
            bool exist = false;
            int? x = 0;
            using (var dbCtx = new Model1Container())
            {
                if (Umisteni == null) {
                    x = dbCtx.Revize
                    .Where(r => r.ProvozId == Provoz)
                    .Where(r => r.Rok == Rok)
                    .Where(r => r.Pololeti == Pololeti)
                    .Select(r => r.Id).FirstOrDefault();
                }
                else {

                    x = dbCtx.Revize
                    .Where(r => r.ProvozId == Provoz)
                    .Where(r => r.Rok == Rok)
                    .Where(r => r.Pololeti == Pololeti)
                    .Where(r => r.UmisteniId == Umisteni)
                    .Select(r => r.Id).FirstOrDefault();
                }


                
            }
            if (x > 0) { exist = true; } 
            return exist;
        }
   
        /// <summary>
        /// Dohledání zda existuje revize a vrati dohledanou revizi
        /// Metoda, ktera na zaklade zakaznika, provozu, roku a pololeti dohleda zda existuje revize a vraci revizi
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        internal protected static Revize ReturnRevision(int Zakaznik, int Provoz, int Rok, int Pololeti, int? Umisteni)
        {
            
            Revize revize = new Revize();
            int? x = 0;
            using (var dbCtx = new Model1Container())
            {
                if (Umisteni == null)
                {
                    x = dbCtx.Revize
                    .Where(r => r.ProvozId == Provoz)
                    .Where(r => r.Rok == Rok)
                    .Where(r => r.Pololeti == Pololeti)
                    .Select(r => r.Id).FirstOrDefault();
                }
                else
                {

                    x = dbCtx.Revize
                    .Where(r => r.ProvozId == Provoz)
                    .Where(r => r.Rok == Rok)
                    .Where(r => r.Pololeti == Pololeti)
                    .Where(r => r.UmisteniId == Umisteni)
                    .Select(r => r.Id).FirstOrDefault();
                }
            }            
            return revize;
        }

        /// <summary>
        /// Na základě parametrů vygeneruje nové revize
        /// </summary>
        /// <param name="Provoz"></param>
        /// <param name="Rok"></param>
        /// <param name="Pololeti"></param>
        /// <param name="DatumRevize"></param>
        /// <param name="StatusRevize"></param>
        /// <returns></returns>
        internal protected static Revize GenerateRevision(int Provoz, int Rok, int Pololeti, DateTime DatumRevize, int StatusRevize, int? Umisteni)
        {

            Revize revize = new Revize();
            revize.DatumRevize = DatumRevize;
            revize.Pololeti = Pololeti;
            revize.ProvozId = Provoz;
            revize.Rok = Rok;
            revize.StatusRevizeId = StatusRevize;
            if (Umisteni != 0) { revize.UmisteniId = Umisteni; }
            using (var dbCtx = new Model1Container())
            {
                dbCtx.Revize.Add(revize);
                dbCtx.SaveChanges();
            }
            return revize;
        }

        internal protected static bool RevizeExist(int RevizeId)
        {
            bool exist = false;
            using (var dbCtx = new Model1Container())
            {
                var item = dbCtx.Revize.Where(r=>r.Id == RevizeId).Count();
                if (item > 0) { exist = true; }
            }
            return exist;

        }

        internal protected static List<Revize> GetByDate (int Mesic, int Rok, int Den, int Region)
        {
            List<Revize> list = new List<Revize>();
            using (var dbCtx = new Model1Container())
            {
                var listx = dbCtx.Revize
                            .Include(r => r.Umisteni)
                            .Include(r => r.Provoz)
                            .Include(r => r.StatusRevize)
                            
                            .Where(r=>r.DatumRevize.Month == Mesic && r.DatumRevize.Year == Rok && r.DatumRevize.Day == Den)
                            ;
                list = listx.ToList();
                foreach (var item in list)
                {
                    var ZakId = dbCtx.Provoz.Where(p => p.Id == item.ProvozId).Select(p=>p.ZakaznikId).FirstOrDefault();
                    item.Zakaznik = dbCtx.Zakaznik.Where(p => p.Id == ZakId).FirstOrDefault();
                }
                if (Region != 0)
                {
                    list.Where(r => r.Zakaznik.RegionId == Region).ToList();
                }
            }
            return list;


        }

        internal protected static Revize CloseRevize(int Id)
        {
            Revize revize = new Revize();
            RevizeSC revizesc = new RevizeSC();            
            List<RevizeSC> revizesclist = new List<RevizeSC>();
            
            using (var dbCtx = new Model1Container())
            {
                revize = dbCtx.Revize.Find(Id);
                revize.StatusRevizeId = dbCtx.StatusRevize.Where(s => s.Realizovana == true).Select(s=>s.Id).FirstOrDefault();
                try
                {
                    dbCtx.Entry(revize).State = EntityState.Modified;
                    dbCtx.SaveChanges();
                }
                catch{ }
                revizesclist = VST_sprava_servisu.RevizeSC.GetListByRevizeId(Id,null);
                CallSCProvozupdate(revizesclist, revize.KontrolaProvedenaDne.Value);
            }

                return revize;
        }

        internal protected static void CallSCProvozupdate(List<RevizeSC> revizesclist,DateTime datumkontroly)
        {
            foreach (var item in revizesclist)
            {
                VST_sprava_servisu.SCProvozu.UpdateSC(item.SCProvozuId, datumkontroly, item.Baterie, item.Pyro, item.TlakovaZkouska);
            }


        }
    }
}
