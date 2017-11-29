using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace VST_sprava_servisu
{
    public partial class Revize
    {
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
        internal protected static bool ExistRevision(int Zakaznik, int Provoz, int Rok, int Pololeti)
        {
            bool exist = false;
            int? x = 0;
            using (var dbCtx = new Model1Container())
            {
                x = dbCtx.Revize
                    .Where(r => r.ProvozId == Provoz)
                    .Where(r => r.Rok == Rok)
                    .Where(r => r.Pololeti == Pololeti)
                    .Select(r => r.Id).FirstOrDefault();
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
        internal protected static Revize ReturnRevision(int Zakaznik, int Provoz, int Rok, int Pololeti)
        {
            
            Revize revize = new Revize(); 
            using (var dbCtx = new Model1Container())
            {
                revize = dbCtx.Revize
                    .Where(r => r.ProvozId == Provoz)
                    .Where(r => r.Rok == Rok)
                    .Where(r => r.Pololeti == Pololeti).FirstOrDefault();
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
        internal protected static Revize GenerateRevision(int Provoz, int Rok, int Pololeti, DateTime DatumRevize, int StatusRevize)
        {

            Revize revize = new Revize();
            revize.DatumRevize = DatumRevize;
            revize.Pololeti = Pololeti;
            revize.ProvozId = Provoz;
            revize.Rok = Rok;
            revize.StatusRevizeId = StatusRevize;
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

        internal protected static List<Revize> GetByDate (int Mesic, int Rok)
        {
            List<Revize> list = new List<Revize>();
            using (var dbCtx = new Model1Container())
            {
                list = dbCtx.Revize.Where(r=>r.DatumRevize.Month == Mesic && r.DatumRevize.Year == Rok).ToList();
            }
            return list;


        }
    }
}
