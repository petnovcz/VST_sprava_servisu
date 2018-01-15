using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class DnyRevize
    {
        internal DateTime DenRevize1 { get; set; }
        internal DateTime PrvnidenobdobiR1 { get; set; }
        internal DateTime PoslednidenobdobiR1 { get; set; }
        internal DateTime DenRevize2 { get; set; }
        internal DateTime PrvnidenobdobiR2 { get; set; }
        internal DateTime PoslednidenobdobiR2 { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("DnyRevize");
    }
}