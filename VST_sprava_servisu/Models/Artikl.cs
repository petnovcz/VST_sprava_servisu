using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public partial class Artikl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Artikl");

        [Authorize(Roles = "Administrator,Manager")]
        public static bool CreateFromSAPdata(SAPItem sapItem)
        {
            using (var dbCtx = new Model1Container())
            {
                
                Artikl artikl = new Artikl();
                artikl.KodSAP = sapItem.ItemCode;
                artikl.Nazev = sapItem.ItemName;

                artikl.Typ = sapItem.ItmsGrpNam;
                artikl.Oznaceni = sapItem.ItemName;
                artikl.RozsahProvoznichTeplot = " ";
                artikl.SkupinaArtiklu = sapItem.ItmsGrpCod;
                try
                {
                    dbCtx.Artikl.Add(artikl);
                    dbCtx.SaveChanges();
                }
                catch (SqlException e)
                {
                    log.Error("Error number: " + e.Number + " - " + e.Message);
                }
            }




            return true;
        }
    }
}