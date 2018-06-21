using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class ZakaznickySeznam
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("BezRevize");

        [Display(Name = "Zákazník")]
        public int ZakaznikId { get; set; }
        [Display(Name = "Zákazník")]
        public string Zakaznik { get; set; }
        [Display(Name = "Provoz")]
        public int ProvozId { get; set; }
        [Display(Name = "Provoz")]
        public string Provoz { get; set; }
        [Display(Name = "Umístění")]
        public int? UmisteniId { get; set; }
        [Display(Name = "Umístění")]
        public string NazevUmisteni { get; set; }

        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum poslední revize (min)")]
        public DateTime? DatumPosledniRevize
        {
            get
            {
                DateTime? datum = DateTime.MinValue;
                using (var db = new Model1Container())
                {
                    if (UmisteniId != null)
                    {
                        datum = db.SCProvozu
                            .Where(t=>t.ProvozId == ProvozId)
                            .Where(t=>t.Umisteni == UmisteniId)
                            .Min(t => t.DatumRevize);
                    }
                    else
                    {
                        datum = db.SCProvozu
                            .Where(t => t.ProvozId == ProvozId)                           
                            .Min(t => t.DatumRevize);
                    }
                }
                return datum;
            }
        }
        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum poslední výměny baterie (min)")]
        public DateTime? DatumPosledniBaterie
        {
            get
            {
                DateTime? datum = DateTime.MinValue;
                using (var db = new Model1Container())
                {
                    if (UmisteniId != null)
                    {
                        datum = db.SCProvozu
                            .Where(t => t.ProvozId == ProvozId)
                            .Where(t => t.Umisteni == UmisteniId)
                            .Min(t => t.DatumBaterie);
                    }
                    else
                    {
                        datum = db.SCProvozu
                            .Where(t => t.ProvozId == ProvozId)
                            .Min(t => t.DatumBaterie);
                    }
                }
                return datum;
            }
        }
        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum poslední výměny pyro (min)")]
        public DateTime? DatumPosledniPyro
        {
            get
            {
                DateTime? datum = DateTime.MinValue;
                using (var db = new Model1Container())
                {
                    if (UmisteniId != null)
                    {
                        datum = db.SCProvozu
                            .Where(t => t.ProvozId == ProvozId)
                            .Where(t => t.Umisteni == UmisteniId)
                            .Min(t => t.DatumPyro);
                    }
                    else
                    {
                        datum = db.SCProvozu
                            .Where(t => t.ProvozId == ProvozId)
                            .Min(t => t.DatumPyro);
                    }
                }
                return datum;
            }
        }



        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum poslední tlakové zkoušky (min)")]
        public DateTime? DatumPosledniTlkZk

        {
            get
            {
                DateTime? datum = DateTime.MinValue;
                using (var db = new Model1Container())
                {
                    if (UmisteniId != null)
                    {
                        datum = db.SCProvozu
                            .Where(t => t.ProvozId == ProvozId)
                            .Where(t => t.Umisteni == UmisteniId)
                            .Min(t => t.DatumTlkZk);
                    }
                    else
                    {
                        datum = db.SCProvozu
                            .Where(t => t.ProvozId == ProvozId)
                            .Min(t => t.DatumTlkZk);
                    }
                }
                return datum;
            }
        }
        [Display(Name = "Počet otevřených revizí")]
        public int PocetOtevrenychRevizi {
            get {
                int count = 0;
                using (var db = new Model1Container())
                {
                    if (UmisteniId != null)
                    {
                        count = db.Revize
                        .Where(t => t.ProvozId == ProvozId)
                        .Where(t=>t.UmisteniId == UmisteniId)
                        .Where(t=>t.StatusRevize.Realizovana != true)
                        .Count();
                    }
                    else
                    {
                        count = db.Revize
                        .Where(t => t.ProvozId == ProvozId)
                        
                        .Where(t => t.StatusRevize.Realizovana != true)
                        .Count();
                    }

                }
                return count;
            }


        }

    }
}