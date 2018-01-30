using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public class GenerovaniRevizeTlakoveZkousky
    {
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


        public static void GenerujReviziTlakoveZkousky(int ZakaznikId, int ProvozId, int? UmisteniId, int Rok)
        {
            GenerovaniRevizeTlakoveZkousky GRTZ = new GenerovaniRevizeTlakoveZkousky();
            GRTZ.Rok = Rok;
            GRTZ.ZakaznikId = ZakaznikId;
            GRTZ.Zakaznik = Zakaznik.GetById(ZakaznikId);
            GRTZ.ProvozId = ProvozId;
            GRTZ.Provoz = Provoz.GetById(ProvozId);
            GRTZ.UmisteniId = UmisteniId;
            if (UmisteniId != null)
            {                
                GRTZ.Umisteni = Umisteni.GetById(UmisteniId.Value);
            }
            // dohledání exisujících revizí pro vybraný rok
            GRTZ.Revize1 = Revize.ReturnRevision(GRTZ.ZakaznikId, GRTZ.ProvozId, Rok, 1, GRTZ.UmisteniId, false);
            GRTZ.Revize2 = Revize.ReturnRevision(GRTZ.ZakaznikId, GRTZ.ProvozId, Rok, 2, GRTZ.UmisteniId, false);
            // načení záznamů, ketré jsou s evidovanou tlakovou zkouškou
            GRTZ.TlakoveZkouskyRev1 = RevizeSC.SeznamTlakovychZkousekRevize(GRTZ.Revize1.Id);
            GRTZ.TlakoveZkouskyRev2 = RevizeSC.SeznamTlakovychZkousekRevize(GRTZ.Revize2.Id);
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
                GRTZ.Revize3 = Revize.GenerateRevision(GRTZ.ProvozId, GRTZ.Rok, 3, System.DateTime.Now,StatusRevize.Planned(), GRTZ.UmisteniId, GRTZ.Revize1.Nabidka, GRTZ.Revize2.Projekt);
            }
            RevizeSC.LoopRevizeSCTlakoveZkousky(GRTZ.Revize3, GRTZ.TlakoveZkouskyRev1);
            RevizeSC.LoopRevizeSCTlakoveZkousky(GRTZ.Revize3, GRTZ.TlakoveZkouskyRev2);
            Revize.UpdateRevizeHeader(GRTZ.Revize1.Id);
            Revize.UpdateRevizeHeader(GRTZ.Revize2.Id);
            Revize.UpdateRevizeHeader(GRTZ.Revize3.Id);


        }
    }
}