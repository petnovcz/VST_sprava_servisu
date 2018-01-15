using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class GRFiltr
    {
        [Key]
        public int Rok { get; set; }
        public int? Region { get; set; }
        public int? Zakaznik { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("GRFiltr");
    }
}