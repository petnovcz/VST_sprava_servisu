using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            List<SelectListItem> skupinalist = new List<SelectListItem>
            { 
            //Add select list item to list of selectlistitems
                new SelectListItem() { Value = "0", Text = "Vše" },
                new SelectListItem() { Value = "1", Text = "Česká Republika" },
                new SelectListItem() { Value = "2", Text = "Polsko" },
                new SelectListItem() { Value = "3", Text = "Slovensko a Maďarsko" },
                new SelectListItem() { Value = "4", Text = "Ostatní" }
            };
            //Return the list of selectlistitems as a selectlist
            if (Skupina == null)
            {
                try
                {
                    string x = Session["List_Skupina"].ToString();
                    if (!Regex.IsMatch(x, @"\w{1-35}")) throw new ArgumentException("Nepovolené znaky v proměné List_Skupina");
                    
                    Skupina = Convert.ToInt32(x);
                }
                catch(Exception ex)
                {
                    log.Error("BezRevize - index - načtení ze session List_Skupina: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);  
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

            List<SelectListItem> roklist = new List<SelectListItem>
            {
                new SelectListItem() { Value = DateTime.Now.Year.ToString(), Text = DateTime.Now.Year.ToString() },
                new SelectListItem() { Value = (DateTime.Now.Year + 1).ToString(), Text = (DateTime.Now.Year + 1).ToString() },
                new SelectListItem() { Value = (DateTime.Now.Year - 1).ToString(), Text = (DateTime.Now.Year - 1).ToString() }
            };
            
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