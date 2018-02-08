using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class ProvedeniVymenyLahveController : Controller
    {
        // GET: ProvedeniVymenyLahve
        [HttpGet]
        public ActionResult VyhledaniSC(int RevizeSCId)
        {
            ProvedeniVymenyLahve pvl = new ProvedeniVymenyLahve();
            pvl = ProvedeniVymenyLahve.Main(RevizeSCId);
            return View(pvl);
        }
        [HttpPost]
        public ActionResult VyhledaniSCImport(int RevizeSCId, string SC)
        {
            ProvedeniVymenyLahve pvl = new ProvedeniVymenyLahve();
            pvl = ProvedeniVymenyLahve.Main(RevizeSCId);
            pvl.SAPSerioveCisloList = SAPSerioveCislo.LoadSCFromSAP(SC, 1);
            return View(pvl);
        }
        [HttpPost]
        public ActionResult ImportSCtoServis(int RevizeSCId, int ArticlId, string SerioveCislo, DateTime DatumVyroby, DateTime DatumDodani)
        {
            ProvedeniVymenyLahve pvl = new ProvedeniVymenyLahve();
            pvl = ProvedeniVymenyLahve.Main(RevizeSCId);
            ProvedeniVymenyLahve.VymenaLahve(RevizeSCId, ArticlId, SerioveCislo, DatumVyroby, DatumDodani);
            

            return RedirectToAction("Details","Revize",new { id = pvl.Revize.Id});
        }
    }
}