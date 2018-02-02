using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ProvedeniVymenyLahve");

        private static string DohledaniSeriovehoCislaLahveDleSeriovehoCislaAkcnihoprvku(string SC, string SAPKodArtiklu)
        {
            string sclahve = "";

            
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();

            sql.Append(" select coalesce(t0.IntrSerial, t0.SysSerial) as 'Serial'  ");
            sql.Append($"  from OSRI t0 ");
            sql.Append($"  inner join SRI1 t1 on t0.SysSerial = t1.SysSerial and t0.ItemCode = t1.ItemCode ");
            sql.Append($"  left join OIGE t2 on t1.BaseNum = t2.DocNum");
            sql.Append($"  left join IGE1 t3 on t2.DocEntry = t3.DocEntry and t3.ItemCode = t1.ItemCode");
            sql.Append($"  left join oitm t4 on t1.ItemCode = t4.ItemCode");
            sql.Append($"  where t3.BaseRef = ");
            sql.Append($"  (select t3.BaseRef from OSRI t0 ");
            sql.Append($"  inner join SRI1 t1 on t0.SysSerial = t1.SysSerial and t0.ItemCode = t1.ItemCode and t1.BaseType = '59'");
            sql.Append($"  left join OIGN t2 on t1.BaseNum = t2.DocNum");
            sql.Append($"  left join IGN1 t3 on t2.DocEntry = t3.DocEntry");
            sql.Append($"  where t0.IntrSerial = '{SC}' and t3.BaseRef is not null and T0.ItemCode = '{SAPKodArtiklu}') and t4.ItmsGrpCod = '138' ");
            


            SqlConnection cnn = new SqlConnection(connectionString);
            //SqlConnection con = new SqlConnection(cnn);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = sql.ToString();
            cnn.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //MAKES IT HERE   
                while (dr.Read())
                {
                    var x  = dr.GetInt32(0);
                    sclahve = x.ToString();
                }
            }
            cnn.Close();
            




            return sclahve;
        }




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
            newSerioveCislo = SerioveCislo.GetSerioveCisloById(id);
            // vyvoreni noveho provozusc
            var idscprovozu = SCProvozu.AddSCProvozu(sCImport, id);
            newSCProvozu = SCProvozu.GetSCProvozuById(idscprovozu);
            //Dohledani serioveho cisla lahve
            var seriovecislolahve = DohledaniSeriovehoCislaLahveDleSeriovehoCislaAkcnihoprvku(newSerioveCislo.SerioveCislo1, newSerioveCislo.Artikl.KodSAP);

            // vytvoření vazby vymeny mezi jednotlivými SCProvozu
            vymenyLahvi = VymenyLahvi.GenerujVymenu(oldSCProvozu, newSCProvozu, revize.DatumRevize, revize.Id, seriovecislolahve);
            // zneaktivneni stareho provozusc
            SCProvozu.ZneaktivniSCProvozu(oldSCProvozu, revize.DatumRevize);

            // odebrani stare revizesc
            RevizeSC.Remove(oldRevizeSC.Id);

            // vymena v nasledujicich otevrenych revizich stareho revizesc za noveho
            RevizeSC.ChangeRevizeSCForUpcomingOpenRevision(oldSCProvozu, newSCProvozu, revize.DatumRevize);



        }
    }


    
}