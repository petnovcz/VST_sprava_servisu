using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class BezRevizeController : Controller
    {
        // GET: BezRevize
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index(int? Rok, int? Skupina, string Search)
        {
            BezRevize BR = new BezRevize();

            List<SelectListItem> skupinalist = new List<SelectListItem>();
            //Add select list item to list of selectlistitems
            skupinalist.Add(new SelectListItem() { Value = "0", Text = "Vše" });
            skupinalist.Add(new SelectListItem() { Value = "1", Text = "Česká Republika" });
            skupinalist.Add(new SelectListItem() { Value = "2", Text = "Polsko" });
            skupinalist.Add(new SelectListItem() { Value = "3", Text = "Slovensko a Maďarsko" });
            skupinalist.Add(new SelectListItem() { Value = "4", Text = "Ostatní" });

            //Return the list of selectlistitems as a selectlist
            if (Skupina == null)
            {
                try
                {
                    var x = Session["List_Skupina"].ToString();
                    Skupina = Convert.ToInt32(x);
                }
                catch(Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);  
                }
                if (Skupina == null)
                {
                    Skupina = 0;
                }
            }
            if (Skupina != null)
            {
                ViewBag.Skupina = new SelectList(skupinalist, "Value", "Text", Skupina);
            }
            else
            {
                ViewBag.Skupina = new SelectList(skupinalist, "Value", "Text", 0);
            }

            List<SelectListItem> roklist = new List<SelectListItem>();
            //Add select list item to list of selectlistitems
            roklist.Add(new SelectListItem() { Value = DateTime.Now.Year.ToString(), Text = DateTime.Now.Year.ToString() });
            roklist.Add(new SelectListItem() { Value = (DateTime.Now.Year + 1).ToString(), Text = (DateTime.Now.Year + 1).ToString() });
            roklist.Add(new SelectListItem() { Value = (DateTime.Now.Year - 1).ToString(), Text = (DateTime.Now.Year - 1).ToString() });
            //Return the list of selectlistitems as a selectlist

            if (Rok == null) { BR.Rok = DateTime.Now.Year; Rok = DateTime.Now.Year; }
            if (Rok != null)
            {
                ViewBag.Rok = new SelectList(roklist, "Value", "Text", Rok);
                BR.Rok = Rok ?? 0;
            }
            else
            {
                ViewBag.Rok = new SelectList(roklist, "Value", "Text", null);
                BR.Rok = Rok ?? 0;
            }
            if (Search == null) { Search = ""; }
            BR.ZakaznickySeznam = BezRevize.GetCustomerListWithoutRevision(Rok.Value, Skupina.Value, Search);

            return View(BR);
        }
    }
}