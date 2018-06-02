using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class SPZController : Controller
    {
        // GET: SPZ
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            SPZ item = new SPZ { ImportedRecords = SPZ.ImportSPZData(), SPZ_NotImported = SPZ.GetNotImportableData() };
            
            if (item.SPZ_NotImported.Count > 0)
            {
                item.SendEmailWithSPZNotImportableData(item.SPZ_NotImported);
            }
            return View(item);
        }
    }
}