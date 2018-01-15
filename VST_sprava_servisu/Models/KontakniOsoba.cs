using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public partial class KontakniOsoba
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("KontakniOsoba");

        [Authorize(Roles = "Administrator,Manager")]
        public static bool Generate(int ZakaznikId, string JmenoPrijmeni, string Pozice, string Telefon, string Email, int SAPId)
        {
            KontakniOsoba ko = new KontakniOsoba();
            ko.ZakaznikId = ZakaznikId;
            ko.JmenoPrijmeni = JmenoPrijmeni;
            if (Pozice == null) { Pozice = ""; }
            ko.Pozice = Pozice;
            if (Telefon == null) { Telefon = ""; }
            ko.Telefon = Telefon;
            if (Email == null) { Email = ""; }
            ko.Email = Email;
            ko.SAPId = SAPId;
            //ko.ProvozId = ProvozId;
            using (var dbCtx = new Model1Container())
            {

                try
                {
                    dbCtx.KontakniOsoba.Add(ko);
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