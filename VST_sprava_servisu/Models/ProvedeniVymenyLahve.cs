using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class ProvedeniVymenyLahve
    {
        public int RevizeSCId { get; set; }
        public string SC { get; set; }
        public IEnumerable<SAPSerioveCislo> SAPSerioveCisloList { get; set; }
        public Revize Revize { get; set; }
        public RevizeSC RevizeSC { get; set; }
        public SAPSerioveCislo SAPSerioveCislo { get; set;}
        







        public static ProvedeniVymenyLahve Main(int RevizeSCId)
        {
            ProvedeniVymenyLahve pvl = new ProvedeniVymenyLahve();
            pvl.RevizeSCId = RevizeSCId;
            pvl.RevizeSC = RevizeSC.GetRevizeSCByRevizeSCid(RevizeSCId);
            pvl.Revize = Revize.GetById(pvl.RevizeSC.RevizeId);

            return pvl;
        }

        public static void VymenaLahve(int RevizeSCid, int AriclId, string SC, DateTime DatumVyroby, DateTime DatumDodani)
        {
            RevizeSC oldRevizeSC = new RevizeSC();
            Revize revize = new Revize();
            Provoz provoz = new Provoz();
            
            RevizeSC newRevizeSC = new RevizeSC();
            SCProvozu oldSCProvozu = new SCProvozu();
            SCProvozu newSCProvozu = new SCProvozu();
            SerioveCislo oldSerioveCislo = new SerioveCislo();
            SerioveCislo newSerioveCislo = new SerioveCislo();
            VymenyLahvi vymenyLahvi = new VymenyLahvi();

            
            oldRevizeSC = RevizeSC.GetRevizeSCByRevizeSCid(RevizeSCid);
            oldSCProvozu = SCProvozu.GetSCProvozuById(oldRevizeSC.SCProvozuId);
            oldSerioveCislo = SerioveCislo.GetSerioveCisloById(oldSCProvozu.SerioveCisloId);
            revize = Revize.GetById(oldRevizeSC.RevizeId);
            provoz = Provoz.GetById(revize.ProvozId);

            SCImport sCImport = new SCImport();
            sCImport.ArtiklId = oldSerioveCislo.ArtiklId;
            sCImport.ArtiklSAPKod = oldSerioveCislo.Artikl.KodSAP;
            sCImport.Baterie = oldSCProvozu.Baterie;
            sCImport.BaterieArtikl = oldSCProvozu.BaterieArtikl;
            sCImport.DatumBaterie = oldSCProvozu.DatumBaterie;
            sCImport.DatumDodani = DatumDodani;
            sCImport.DatumPosledniZmeny = revize.DatumRevize;
            sCImport.DatumPrirazeni = revize.DatumRevize;
            sCImport.DatumPyro = oldSCProvozu.DatumPyro;
            sCImport.DatumRevize = revize.DatumRevize;
            sCImport.DatumTlkZk = revize.DatumRevize;
            sCImport.DatumVymeny = null;
            sCImport.DatumVyroby = DatumVyroby;
            sCImport.Lokace = oldSCProvozu.Lokace;
            sCImport.Proverit = false;
            sCImport.Provozy = oldSCProvozu.ProvozId;
            sCImport.SerioveCislo = SC;
            sCImport.Submitted = true;
            sCImport.Umisteni = oldSCProvozu.Umisteni.Value;
            sCImport.Zakaznik = provoz.ZakaznikId;
            sCImport.ZakaznikSAPKod = provoz.Zakaznik.KodSAP;
            sCImport.Znaceni = oldSCProvozu.Znaceni;

            // vytvoreni Serioveho cisla akcniho prvku
            var id = SerioveCislo.AddSeriovecislo(sCImport);
            // vyvoreni noveho provozusc
            var idscprovozu = SCProvozu.AddSCProvozu(sCImport, id);
            newSCProvozu = SCProvozu.GetSCProvozuById(idscprovozu);
            // vytvoření vazby vymeny mezi jednotlivými SCProvozu
            vymenyLahvi = VymenyLahvi.GenerujVymenu(oldSCProvozu, newSCProvozu, revize.DatumRevize);
            // zneaktivneni stareho provozusc
            SCProvozu.ZneaktivniSCProvozu(oldSCProvozu, revize.DatumRevize);
            // vymena v nasledujicich otevrenych revizich 




        }
    }


    
}