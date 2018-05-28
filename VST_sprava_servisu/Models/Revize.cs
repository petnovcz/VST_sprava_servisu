using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;



namespace VST_sprava_servisu
{
    public partial class RevizeBaterie
    {
        [Key]
        public int BaterieArtikl { get; set; }
        public string BaterieSAPKod { get; set; }
        public string BaterieName { get; set; }
        public int Pocet { get; set; } 


    }

    public partial class Revize
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Revize");

        [NotMapped]
        private Zakaznik Zakaznik { get; set; }
        [NotMapped]
        public int Region { get; set; }
        [NotMapped]
        public virtual ICollection<RevizeBaterie> RevizeBaterie { get { return CalculateRevizeBaterie(Id); } }

        [NotMapped]
        public string PoznamkazPredchoziRevize { get {


                string Poznamka = "";
                StringBuilder sql = new StringBuilder();
                sql.Append(" select Top 1 Poznamka as 'Poznamka' from Revize where");
                sql.Append($" ProvozId = (select ProvozId from Revize where Id = '{Id}') and");
                sql.Append($" (UmisteniId = (select UmisteniId from Revize where Id = '{Id}' )");
                sql.Append($" or(UmisteniId is null and(select UmisteniId from Revize where Id = '{Id}' ) is null))");
                sql.Append($" and (select DatumRevize from Revize where Id = '{Id}' ) > KontrolaProvedenaDne and Id <> {Id}");
                sql.Append($" order by KontrolaProvedenaDne desc");
                sql.Append($"");

                SqlConnection cnn = new SqlConnection(connectionString);
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
                        
                        try
                        {
                            Poznamka = dr.GetString(dr.GetOrdinal("Poznamka"));
                        }
                        catch (Exception ex)
                        { //log.Info($"Baterie Artikl prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                        }
                        
                    }
                }
                cnn.Close();




                return Poznamka;
            } }

        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        /// <summary>
        /// Projde list revizí a aktualizuje ICollection s baterkama a počtem
        /// </summary>
        /// <param name="revizelist"></param>
        /// <returns></returns>
        internal protected static List<Revize> LoopRevizeAndUpdateBatery(List<Revize> revizelist)
        {
            
            for (var x = 0; x < revizelist.Count; x++)
            {
                int revizeid = revizelist[x].Id;
                UpdateRevizeHeader(revizeid);
                Revize revize = revizelist[x];
                //revize.RevizeBaterie = revize.CalculateRevizeBaterie(revizeid);
                revizelist[x] = revize;
                

            }

            return revizelist;
        }



        /// <summary>
        /// Dle čísla revize vrací ICColection Baterií a počtu baterií
        /// </summary>
        /// <param name="revize"></param>
        /// <returns></returns>
        internal protected List<RevizeBaterie> CalculateRevizeBaterie(int revize)
        {
            List<RevizeBaterie> listplanrev = new List<RevizeBaterie>();
            StringBuilder sql = new StringBuilder();
            sql.Append(" select t1.BaterieArtikl as 'BaterieArtikl', COUNT(t1.BaterieArtikl) as 'Pocet', t3.Nazev as 'BaterieName', t3.KodSAP as 'BaterieSAPKod'");
            sql.Append(" from RevizeSC t0 inner join SCProvozu t1 on t0.SCProvozuID = t1.Id inner join Artikl t3 on t1.BaterieArtikl = t3.Id");
            sql.Append($" where t1.BaterieArtikl is not Null and t0.RevizeId = '{revize}' and t0.Baterie = 1");
            sql.Append(" group by t1.BaterieArtikl, t3.Nazev, t3.KodSAP");

            //  LOGOVANI
            //log.Debug($"CalculateRevizeBaterie pro revizi č.{revize.ToString()}");
            //log.Debug(sql.ToString());

            SqlConnection cnn = new SqlConnection(connectionString);
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
                    RevizeBaterie item = new RevizeBaterie();
                    try
                    {
                        item.BaterieArtikl = dr.GetInt32(dr.GetOrdinal("BaterieArtikl"));
                    }
                    catch(Exception ex) { //log.Info($"Baterie Artikl prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.BaterieSAPKod = dr.GetString(dr.GetOrdinal("BaterieSAPKod"));
                    }
                    catch (Exception ex) { //log.Info($"Baterie SAPKod prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.BaterieName = dr.GetString(dr.GetOrdinal("BaterieName"));
                    }
                    catch (Exception ex) {// log.Info($"Baterie nazev prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    try
                    {
                        item.Pocet = dr.GetInt32(dr.GetOrdinal("Pocet"));
                    }
                    catch (Exception ex) { //log.Info($"pocet prázdné {ex.Message} {ex.InnerException} {ex.Data}");
                    }
                    

                    listplanrev.Add(item);
                }
            }
            cnn.Close();
            
            return listplanrev;


        }

        public static void UpdateRevizeHeader (int id)
        {
            using (var dbCtx = new Model1Container())
            {
                Revize revize = new Revize();
                revize = dbCtx.Revize.Find(id);
                revize.Baterie = dbCtx.RevizeSC.Where(r=>r.RevizeId == id && r.Baterie == true).Count();
                revize.Pyro = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.Pyro == true).Count() + dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.Pyro == true && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id >= 134 && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id <= 135).Count();
                revize.TlkZk = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.TlakovaZkouska == true).Count();
                revize.AP = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 131).Count();
                revize.S = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 1).Count();
                revize.RJ = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 130).Count();
                revize.V = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id >= 132 && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id <= 133).Count();
                revize.F = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id >= 134 && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id <= 135).Count();
                revize.M = dbCtx.RevizeSC.Where(r => r.RevizeId == id && r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.Id == 136 ).Count();
                //revize.RevizeBaterie = revize.CalculateRevizeBaterie(id);
                try
                {
                    dbCtx.Entry(revize).State = EntityState.Modified;
                    dbCtx.SaveChanges();
                }
                catch(Exception ex)
                {
                    log.Error($"Update Revize Header pro revizi č.{id} s chybovou hláškou {ex.Message} {ex.Data} {ex.InnerException}");
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
        public static Revize GetById(int Id)
        {
            var provoz = new Revize();
            using (var dbCtx = new Model1Container())
            {
                provoz = dbCtx.Revize.Where(r => r.Id == Id).Include(r=>r.Provoz).FirstOrDefault();
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
        internal protected static Revize ReturnRevision(int Zakaznik, int Provoz, int Rok, int Pololeti, int? Umisteni, bool? closed)
        {
            
            Revize revize = new Revize();
            
            
            using (var dbCtx = new Model1Container())
            {
                var x = dbCtx.Revize
                    //.Where(r => r.Zakaznik.Id == Zakaznik)
                    .Where(r => r.ProvozId == Provoz)
                    .Where(r => r.Rok == Rok)
                    .Where(r => r.Pololeti == Pololeti);
                var x1 = x.ToList();
                var status = dbCtx.StatusRevize.Where(s => s.Realizovana == true).FirstOrDefault();
                if (Umisteni == null)
                {}
                else
                {
                    x = x.Where(r => r.UmisteniId == Umisteni);
                    var x2 = x.ToList();
                }
                if (closed == true)
                {
                    x = x.Where(r => r.StatusRevizeId == status.Id);
                    var x3 = x.ToList();
                }
                if (closed == false)
                {
                    x = x.Where(r => r.StatusRevizeId != status.Id);
                    var x3 = x.ToList();
                }
                ;
                revize = x.FirstOrDefault();
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
        internal protected static Revize GenerateRevision(int Provoz, int Rok, int Pololeti, DateTime DatumRevize, int StatusRevize, int? Umisteni, string Nabidka, string Projekt)
        {

            Revize revize = new Revize();
            revize.DatumRevize = DatumRevize;
            revize.Pololeti = Pololeti;
            revize.ProvozId = Provoz;
            revize.Rok = Rok;
            revize.StatusRevizeId = StatusRevize;
            revize.Nabidka = Nabidka;
            revize.Projekt = Projekt;
            if (Umisteni != 0) { revize.UmisteniId = Umisteni; }
            using (var dbCtx = new Model1Container())
            {
                try
                {
                    dbCtx.Revize.Add(revize);
                    dbCtx.SaveChanges();
                }
                catch (Exception ex)
                { log.Error($"Generovani revize GenerateRevision {ex.Data} {ex.InnerException} {ex.Message}"); }
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
                    item.Zakaznik = dbCtx.Zakaznik
                        .Include(z=>z.Region)
                        .Where(p => p.Id == ZakId).FirstOrDefault();
                }
                if (Region != 0)
                {
                    list = list.Where(r => r.Zakaznik.Region.Skupina == Region).ToList();
                }
            }
            return list;


        }
        internal protected static List<Revize> GetByRegion(int Region)
        {
            List<Revize> list = new List<Revize>();
            using (var dbCtx = new Model1Container())
            {
                var listx = dbCtx.Revize
                            .Include(r => r.Umisteni)
                            .Include(r => r.Provoz)
                            .Include(r => r.StatusRevize)                            
                            ;
                list = listx.ToList();
                foreach (var item in list)
                {
                    var ZakId = dbCtx.Provoz.Where(p => p.Id == item.ProvozId).Select(p => p.ZakaznikId).FirstOrDefault();
                    item.Zakaznik = dbCtx.Zakaznik
                        .Include(z => z.Region)
                        .Where(p => p.Id == ZakId).FirstOrDefault();
                }
                if (Region != 0)
                {
                    list = list.Where(r => r.Zakaznik.Region.Skupina == Region).ToList();
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
                catch(Exception ex)
                {
                    log.Error($"CloseRevize revize c.{Id} {ex.Data} {ex.InnerException} {ex.HResult} {ex.Message}");
                }
                revizesclist = VST_sprava_servisu.RevizeSC.GetListByRevizeId(Id,null);
                CallSCProvozupdate(revizesclist, revize.KontrolaProvedenaDne.Value);
            }

                return revize;
        }

        internal protected static void CallSCProvozupdate(List<RevizeSC> revizesclist,DateTime datumkontroly)
        {
            foreach (var item in revizesclist)
            {
                SCProvozu.UpdateSC(item.SCProvozuId, datumkontroly, item.Baterie, item.Pyro, item.TlakovaZkouska);
            }


        }
    }

    public partial class RevizeListInput
    {
        
        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Od")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Do")]
        public DateTime? DateTo { get; set; }
        [Display(Name = "Skupina")]
        public int? Skupina { get; set; }
        [Display(Name = "Zakaznik")]
        public int? Zakaznik { get; set; }
        [Display(Name = "Status")]
        public int? Status { get; set; }

        public virtual ICollection<Revize> Revize { get; set; }

    }
}
