using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class GenerovaniRevizeTlakoveZkouskyController : Controller
    {
        // GET: GenerovaniRevizeTlakoveZkousky
        public ActionResult Index(int RevizeId)
        {
            var revize = Revize.GetById(RevizeId);
            GenerovaniRevizeTlakoveZkousky.GenerujReviziTlakoveZkousky(revize.Provoz.ZakaznikId, revize.ProvozId, revize.UmisteniId,revize.Rok.Value);
            Revize.UpdateRevizeHeader(RevizeId);
            return RedirectToAction("Nahled", "Revize", null); 
        }
    }
}