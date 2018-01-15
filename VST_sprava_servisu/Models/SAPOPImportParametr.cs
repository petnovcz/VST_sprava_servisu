using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class SAPOPImportParametr
    {
        [Key]
        public string Search { get; set; }


        public List<SAPOP> ListSAPOP { get; set; }

    }
}