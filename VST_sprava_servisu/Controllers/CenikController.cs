using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu.Models;

namespace VST_sprava_servisu.Controllers
{
    public class CenikController : Controller
    {
        // GET: Cenik
        public ActionResult Index(int Id)
        {
            Cenik cenik = new Cenik();
            cenik.ZakaznikId = Id;
            return View(cenik);
        }
    }
}