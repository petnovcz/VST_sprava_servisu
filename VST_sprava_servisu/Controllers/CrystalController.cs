using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using System.Data.SqlClient;

namespace VST_sprava_servisu.Controllers
{
    public class CrystalController : Controller
    {
        // GET: Crystal
        public ActionResult Index()
        {


            return View();
        }
    }

    
}