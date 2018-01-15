using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Rok
    {
        public int Id;
        public int Value;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Rok");

        internal protected static List<Rok> GetYears()
        {
            Rok thisyear = new Rok();
            Rok nextyear = new Rok();
            Rok lastyear = new Rok();
            thisyear.Id = DateTime.Now.Year;
            thisyear.Value = DateTime.Now.Year;
            lastyear.Id = thisyear.Id - 1;
            lastyear.Value = thisyear.Id - 1;
            nextyear.Id = thisyear.Id + 1;
            nextyear.Value = thisyear.Id + 1;
            List<Rok> rokl = new List<Rok>();
            rokl.Add(lastyear);
            rokl.Add(thisyear);
            rokl.Add(nextyear);
            return rokl;
        }
        internal protected static Rok ThisYear()
        {
            Rok thisyear = new Rok();

            thisyear.Id = DateTime.Now.Year;
            thisyear.Value = DateTime.Now.Year;

            return thisyear;
        }
    }
}