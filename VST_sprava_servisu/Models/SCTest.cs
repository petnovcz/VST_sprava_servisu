using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class SCTest
    {
        [Key]
        public string SC { get; set; }
        public int Artikl { get; set; }
        public int Zakaznik { get; set; }
        public int Provoz { get; set; }
        public int Umisteni { get; set; }
        public IEnumerable<SAPSerioveCislo> SAPSerioveCIslo { get; set; }
        public IEnumerable<SerioveCislo> SerioveCisloList { get; set; }
        public IEnumerable<SCProvozu> SCProvozuList { get; set; }
    }
}