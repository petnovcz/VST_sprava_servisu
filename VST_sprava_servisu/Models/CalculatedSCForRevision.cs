using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class CalculatedSCForRevision
    {
        public int ZakaznikId { get; set; }
        public int ProvozId { get; set; }
        public int UmisteniId { get; set; }
        public int SCProvozuId { get; set; }
        public Nullable<DateTime> NextRevize { get; set; }
        public Nullable<DateTime> Next2Revize { get; set; }
        public Nullable<DateTime> NextBaterie { get; set; }
        public Nullable<DateTime> NextPyro { get; set; }
        public Nullable<DateTime> NextTlkZk { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("CalculatedSCForRevision");
    }
}