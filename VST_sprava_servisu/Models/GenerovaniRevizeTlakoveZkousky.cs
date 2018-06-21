using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class GenerovaniRevizeTlakoveZkousky
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("GenerovaniRevizeTlakoveZkousky");


        private int Rok { get; set; }
        private int ZakaznikId { get; set; }
        private int ProvozId { get; set; }
        private int? UmisteniId { get; set; }
        private DateTime Datum { get; set; }
        private Zakaznik Zakaznik { get; set; }
        private Provoz Provoz { get; set; }
        private Umisteni Umisteni { get; set; }
        private Revize Revize1 { get; set; }
        private Revize Revize2 { get; set; }
        private Revize Revize3 { get; set; }
        private List<RevizeSC> TlakoveZkouskyRev1 { get; set; }
        private List<RevizeSC> TlakoveZkouskyRev2 { get; set; }

        [Authorize(Roles = "Administrator,Manager")]
        public static void GenerujReviziTlakoveZkousky(int ZakaznikId, int ProvozId, int? UmisteniId, int Rok)
        {
            GenerovaniRevizeTlakoveZkousky GRTZ = new GenerovaniRevizeTlakoveZkousky
            {
                Rok = Rok,
                ZakaznikId = ZakaznikId,
                Zakaznik = Zakaznik.GetById(ZakaznikId),
                ProvozId = ProvozId,
                Provoz = Provoz.GetById(ProvozId),
                UmisteniId = UmisteniId,
                // dohledání exisujících revizí pro vybraný rok
                Revize1 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 1, UmisteniId, false),
                Revize2 = Revize.ReturnRevision(ZakaznikId, ProvozId, Rok, 2, UmisteniId, false)
            };
            if (UmisteniId != null)
            {
                GRTZ.Umisteni = Umisteni.GetById(UmisteniId.Value);
            } 
            int pocetTlkZkR1 = 0; int pocetTlkZkR2 = 0;int revize1 = 0; int revize2 = 0;

            try
            {
                revize1 = GRTZ.Revize1.Id;
            }
            catch (Exception ex)
            {
                log.Debug($"Nenalazena Revize1 {ex.Data} {ex.HResult} {ex.InnerException} {ex.Message}");
            }
            try
            {
                revize2 = GRTZ.Revize2.Id;
            }
            catch(Exception ex)
            {
                log.Debug($"Nenalazena Revize2 {ex.Data} {ex.HResult} {ex.InnerException} {ex.Message}");
            }
            using (var db = new Model1Container())
            {
                //Výpočet počtu artiklů, které mají příznak tlakové zkoušky pro jednotlivé revize
                pocetTlkZkR1 = db.RevizeSC.Where(t => t.RevizeId == revize1 && t.TlakovaZkouska == true).Count();
                pocetTlkZkR2 = db.RevizeSC.Where(t => t.RevizeId == revize2 && t.TlakovaZkouska == true).Count();

            }

            int pocetTlkZ = pocetTlkZkR1 + pocetTlkZkR2;


            
            // Pokud existují artikly v revizích, které mají příznak tlakové zkoušky dojde ke generování třetí revize a přesun do speciální tlakové zkoušky
            if (pocetTlkZ > 0)
            {
                // načení záznamů, ketré jsou s evidovanou tlakovou zkouškou
                if (pocetTlkZkR1 > 0)
                {
                    try
                    {
                        GRTZ.TlakoveZkouskyRev1 = RevizeSC.SeznamTlakovychZkousekRevize(GRTZ.Revize1.Id);
                    }
                    catch (Exception ex) { log.Debug($" GRTZ.TlakoveZkouskyRev1 -  {ex.Data} {ex.HResult} {ex.InnerException} {ex.Message}"); }
                }
                if (pocetTlkZkR2 > 0)
                {
                    try
                    {
                        GRTZ.TlakoveZkouskyRev2 = RevizeSC.SeznamTlakovychZkousekRevize(GRTZ.Revize2.Id);
                    }
                    catch (Exception ex) { log.Debug($" GRTZ.TlakoveZkouskyRev2 -  {ex.Data} {ex.HResult} {ex.InnerException} {ex.Message}"); }
                }
                // pokud neexisuje revize na tlakovou zkoušku vygenerování revize
                var exist = Revize.ExistRevision(GRTZ.ZakaznikId, GRTZ.ProvozId, GRTZ.Rok, 3, GRTZ.UmisteniId);
                if (exist == true)
                {
                    //pokud exisuje nacte se 
                    GRTZ.Revize3 = Revize.ReturnRevision(GRTZ.ZakaznikId, GRTZ.ProvozId, Rok, 3, GRTZ.UmisteniId, null);
                }
                else
                {
                    //pokud neexistuje vygeneruje se
                    try
                    {
                        GRTZ.Revize3 = Revize.GenerateRevision(GRTZ.ProvozId, GRTZ.Rok, 3, System.DateTime.Now, 1, GRTZ.UmisteniId, GRTZ.Revize1.Nabidka, GRTZ.Revize1.Projekt);
                    }
                    catch (Exception ex)
                    {
                        log.Debug($" GRTZ.Revize3 -  {ex.Data} {ex.HResult} {ex.InnerException} {ex.Message}");
                    }
                }
                //přesun tlakových zkoušek do speciální revize
                if (pocetTlkZkR1 > 0)
                {
                    try
                    {
                        RevizeSC.LoopRevizeSCTlakoveZkousky(GRTZ.Revize3, GRTZ.TlakoveZkouskyRev1);
                    }
                    catch (Exception ex) { }
                }
                if (pocetTlkZkR1 > 0)
                {
                    try
                    {
                        RevizeSC.LoopRevizeSCTlakoveZkousky(GRTZ.Revize3, GRTZ.TlakoveZkouskyRev2);
                    }
                    catch (Exception ex) { }
                }
                //aktualizace hlavičkových údajů na jednotlivých revizích
                try
                {
                    Revize.UpdateRevizeHeader(GRTZ.Revize1.Id);
                }
                catch (Exception ex)
                { }
                try
                {
                    Revize.UpdateRevizeHeader(GRTZ.Revize2.Id);
                }
                catch (Exception ex)
                { }
                try
                {
                    Revize.UpdateRevizeHeader(GRTZ.Revize3.Id);
                }
                catch (Exception ex)
                { }
            }

        }
    }
}