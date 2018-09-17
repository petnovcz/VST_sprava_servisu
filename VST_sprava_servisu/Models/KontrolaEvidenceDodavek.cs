using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public class KontrolaEvidenceDodavek
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("KontrolaEvidenceDodavek");

        public class EvidenceDodavekReport
        {
            public int Year { get; set; }
            public List<EvidenceDodavekHeaders> EvidenceHeaders { get; set; }


        }

        public class EvidenceDodavekHeaders
        { }

        public class EvidenceDodacekLines
        { }

    }
}