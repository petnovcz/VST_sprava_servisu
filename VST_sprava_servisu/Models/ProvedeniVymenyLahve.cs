using System;
using System.Collections.Generic;
using System.Linq;
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
        







        public static ProvedeniVymenyLahve Main(int RevizeSCId)
        {
            ProvedeniVymenyLahve pvl = new ProvedeniVymenyLahve();
            pvl.RevizeSCId = RevizeSCId;
            pvl.RevizeSC = RevizeSC.GetRevizeSCByRevizeSCid(RevizeSCId);
            pvl.Revize = Revize.GetById(pvl.RevizeSC.RevizeId);

            return pvl;
        }
    }


    
}