using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int UmisteniId { get; set; }
        [Display(Name = "Umístění")]
        public string NazevUmisteni { get; set; }
    }
}