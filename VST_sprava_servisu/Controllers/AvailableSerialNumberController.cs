using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu
{
    public class AvailableSerialNumberController : Controller
    {
        // GET: AvailableSerialNumber
        public ActionResult List(int SZId , int SZPrvekId , string ArtiklKusovnikSAPKod )
        {
            AvailableSNList asnlist = new AvailableSNList();
            asnlist.SZId = SZId;
            asnlist.SZPrvekId = SZPrvekId;
            asnlist.ArtiklKusovnikSAPKod = ArtiklKusovnikSAPKod;
            return View(asnlist);
        }
    }
}