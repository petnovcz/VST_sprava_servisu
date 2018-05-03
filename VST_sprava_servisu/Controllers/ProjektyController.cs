using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VST_sprava_servisu;
using VST_sprava_servisu.Models;
using System.Data;
using System.Data.Entity;

using System.Net;


namespace VST_sprava_servisu.Controllers
{
    public class ProjektyController : Controller
    {
        // GET: Projekty
        [Authorize(Roles = "Administrator,Vedení")]
        public ActionResult Index(bool? Send, int? Rok, string ciselnarada, string typprojektu,int? pracovnik, string stavyprojektu,int? region,int? skupinaartiklu, bool? pr, bool? sp, bool? r, bool? tp, bool? cr, bool? sa )
        {
            //Read Session
            Projekty projekty = new Projekty();
            if (projekty.VybraneRokyList == null)
            {                
                projekty.VybraneRokyList = new List<Roky>();
                projekty.VybraneRokyList = Session["VybraneRokyList"] != null ? (List<Roky>)Session["VybraneRokyList"] : null;
            }
            if (projekty.VybranaCiselnaRadaList == null)
            {
                projekty.VybranaCiselnaRadaList = new List<CiselnaRadaProjektu>();
                projekty.VybranaCiselnaRadaList = Session["VybranaCiselnaRadaList"] != null ? (List<CiselnaRadaProjektu>)Session["VybranaCiselnaRadaList"] : null;
            }
            if (projekty.VybraneRegionyList == null)
            {
                projekty.VybraneRegionyList = new List<Regiony>();
                projekty.VybraneRegionyList = Session["VybraneRegionyList"] != null ? (List<Regiony>)Session["VybraneRegionyList"] : null;
            }
            if (projekty.VybraneStavyProjektuList == null)
            {
                projekty.VybraneStavyProjektuList = new List<StavyProjektu>();
                projekty.VybraneStavyProjektuList = Session["VybraneStavyProjektuList"] != null ? (List<StavyProjektu>)Session["VybraneStavyProjektuList"] : null;
            }
            if (projekty.VybraneTypyProjektuList == null)
            {
                projekty.VybraneTypyProjektuList = new List<TypProjektu>();
                projekty.VybraneTypyProjektuList = Session["VybraneTypyProjektuList"] != null ? (List<TypProjektu>)Session["VybraneTypyProjektuList"] : null;
            }
            if (projekty.VybraniPracovniciList == null)
            {
                projekty.VybraniPracovniciList = new List<Pracovnici>();
                projekty.VybraniPracovniciList = Session["VybraniPracovniciList"] != null ? (List<Pracovnici>)Session["VybraniPracovniciList"] : null;
            }
            if (projekty.VybranaSkupinaArtikluList == null)
            {
                projekty.VybranaSkupinaArtikluList = new List<SAPSkupinaArtiklu>();
                projekty.VybranaSkupinaArtikluList = Session["VybranaSkupinaArtikluList"] != null ? (List<SAPSkupinaArtiklu>)Session["VybranaSkupinaArtikluList"] : null;
            }

            // Nasavení hodnot pri spusteni
            if (Send == false || Send == null)
            {
                Roky rok = new Roky();
                rok.Id = Convert.ToInt32(DateTime.Now.Year);

                try
                {
                    if (projekty.VybraneRokyList == null)
                    {
                        //It's null - create it
                        projekty.VybraneRokyList = new List<Roky>();
                    }
                    projekty.VybraneRokyList.Add(rok);
                }
                
                catch (Exception ex) { }
                try
                {  if (projekty.VybranaCiselnaRadaList == null)
                    {
                        projekty.VybranaCiselnaRadaList = new List<CiselnaRadaProjektu>();
                        projekty.VybranaCiselnaRadaList = projekty.CiselnaRadaList;
                    }
                }
                catch (Exception ex) { }
                try
                {
                    if (projekty.VybraneTypyProjektuList == null)
                    {
                        projekty.VybraneTypyProjektuList = new List<TypProjektu>();
                        projekty.VybraneTypyProjektuList = projekty.TypProjektuList;
                    }
                }
                catch (Exception ex) { }
                try
                {
                    if (projekty.VybraneRegionyList == null)
                    {
                        projekty.VybraneRegionyList = new List<Regiony>();
                        projekty.VybraneRegionyList = projekty.RegionyList;
                    }
                }
                catch (Exception ex) { }
                try
                {
                    if (projekty.VybraneStavyProjektuList == null)
                    {
                        projekty.VybraneStavyProjektuList = new List<StavyProjektu>();
                        projekty.VybraneStavyProjektuList = projekty.StavyProjektuList;
                    }
                }
                catch (Exception ex) { }
                try
                {
                    if (projekty.VybraniPracovniciList == null)
                    {
                        projekty.VybraniPracovniciList = new List<Pracovnici>();
                        projekty.VybraniPracovniciList = projekty.PracovniciList;
                    }
                }
                catch (Exception ex) { }
                try
                {
                    if (projekty.VybranaSkupinaArtikluList == null)
                    {
                        projekty.VybranaSkupinaArtikluList = new List<SAPSkupinaArtiklu>();
                        projekty.VybranaSkupinaArtikluList = projekty.SkupinaArtikluList;
                    }
                }
                catch (Exception ex) { }
            }
            // Nastaveni pri odeslani hodnot
            if (Send == true)
            {
                if (tp == true)
                    {
                    projekty.VybraneTypyProjektuList = new List<TypProjektu>();
                    projekty.VybraneTypyProjektuList = projekty.TypProjektuList;
                }
                if (tp == false)
                {
                    projekty.VybraneTypyProjektuList = null;                   
                }
                if (cr == true)
                {
                    projekty.VybranaCiselnaRadaList = new List<CiselnaRadaProjektu>();
                    projekty.VybranaCiselnaRadaList = projekty.CiselnaRadaList;
                }
                if (cr == false)
                {
                    projekty.VybranaCiselnaRadaList = null;
                }
                if (sp == true)
                {
                    projekty.VybraneStavyProjektuList = new List<StavyProjektu>();
                    projekty.VybraneStavyProjektuList = projekty.StavyProjektuList;
                }
                if (sp == false)
                {
                    projekty.VybraneStavyProjektuList = null;
                }
                if (r == true)
                {
                    projekty.VybraneRegionyList = new List<Regiony>();
                    projekty.VybraneRegionyList = projekty.RegionyList;
                }
                if (r == false)
                {
                    projekty.VybraneRegionyList = null;
                }
                if (pr == true)
                {
                    projekty.VybraniPracovniciList = new List<Pracovnici>();
                    projekty.VybraniPracovniciList = projekty.PracovniciList;
                }
                if (pr == false)
                {
                    projekty.VybraniPracovniciList = null;
                }
                if (sa == true)
                {
                    projekty.VybranaSkupinaArtikluList = new List<SAPSkupinaArtiklu>();
                    projekty.VybranaSkupinaArtikluList = projekty.SkupinaArtikluList;
                }
                if (sa == false)
                {
                    projekty.VybranaSkupinaArtikluList = null;
                }
                if (Rok != null)
                {
                    Roky rok = new Roky();
                    rok.Id = Convert.ToInt32(Rok);
                    try
                    {
                        if (projekty.VybraneRokyList == null)
                        {
                            //It's null - create it
                            projekty.VybraneRokyList = new List<Roky>();
                            projekty.VybraneRokyList.Add(rok);
                        }
                        else
                        {
                            var isinlist = false;
                            foreach (var itemx in projekty.VybraneRokyList)
                            {
                                if (itemx.Id == Rok)
                                {
                                    isinlist = true;
                                    var x = itemx;
                                    projekty.VybraneRokyList.Remove(x);
                                    projekty.VybraneRokyList.RemoveAll(t => t.Id == itemx.Id);
                                }
                            }
                            if (isinlist == false)
                            { projekty.VybraneRokyList.Add(rok); }
                        }

                    }
                    catch (Exception ex) { }
                }

                //skupinaartiklu
                if (skupinaartiklu != null)
                {
                    SAPSkupinaArtiklu rok = new SAPSkupinaArtiklu();
                    rok.Id = Convert.ToInt32(skupinaartiklu);
                    try
                    {
                        if (projekty.VybranaSkupinaArtikluList == null)
                        {
                            //It's null - create it
                            projekty.VybranaSkupinaArtikluList = new List<SAPSkupinaArtiklu>();
                            projekty.VybranaSkupinaArtikluList.Add(rok);
                        }
                        else
                        {
                            var isinlist = false;
                            foreach (var itemx in projekty.VybranaSkupinaArtikluList)
                            {
                                if (itemx.Id == skupinaartiklu)
                                {
                                    isinlist = true;
                                    var x = itemx;
                                    projekty.VybranaSkupinaArtikluList.Remove(x);
                                    projekty.VybranaSkupinaArtikluList.RemoveAll(t => t.Id == itemx.Id);
                                }
                            }
                            if (isinlist == false)
                            { projekty.VybranaSkupinaArtikluList.Add(rok); }
                        }

                    }
                    catch (Exception ex) { }
                }

                if (region != null)
                {
                    Regiony reg = new Regiony();
                    reg.TerritryID = Convert.ToInt32(region);
                    try
                    {
                        if (projekty.VybraneRegionyList == null)
                        {
                            //It's null - create it
                            projekty.VybraneRegionyList = new List<Regiony>();
                            projekty.VybraneRegionyList.Add(reg);
                        }
                        else
                        {
                            var isinlist = false;
                            foreach (var itemx in projekty.VybraneRegionyList)
                            {
                                if (itemx.TerritryID == region)
                                {
                                    isinlist = true;
                                    var x = itemx;
                                    projekty.VybraneRegionyList.Remove(x);
                                    projekty.VybraneRegionyList.RemoveAll(t => t.TerritryID == itemx.TerritryID);
                                }
                            }
                            if (isinlist == false)
                            { projekty.VybraneRegionyList.Add(reg); }
                        }

                    }
                    catch (Exception ex) { }
                }
                if (pracovnik != null)
                {
                    Pracovnici prac = new Pracovnici();
                    prac.Kod = Convert.ToInt32(pracovnik);
                    try
                    {
                        if (projekty.VybraniPracovniciList == null)
                        {
                            //It's null - create it
                            projekty.VybraniPracovniciList = new List<Pracovnici>();
                            projekty.VybraniPracovniciList.Add(prac);
                        }
                        else
                        {
                            var isinlist = false;
                            foreach (var itemx in projekty.VybraniPracovniciList)
                            {
                                if (itemx.Kod == pracovnik)
                                {
                                    isinlist = true;
                                    var x = itemx;
                                    projekty.VybraniPracovniciList.Remove(x);
                                    projekty.VybraniPracovniciList.RemoveAll(t => t.Kod == itemx.Kod);
                                }
                            }
                            if (isinlist == false)
                            { projekty.VybraniPracovniciList.Add(prac); }
                        }

                    }
                    catch (Exception ex) { }
                }
                if ( String.IsNullOrEmpty(ciselnarada) == false )
                {
                    CiselnaRadaProjektu cisrada = new CiselnaRadaProjektu();
                    cisrada.Kod = Convert.ToString(ciselnarada);
                    try
                    {
                        if (projekty.VybranaCiselnaRadaList == null)
                        {
                            //It's null - create it
                            projekty.VybranaCiselnaRadaList = new List<CiselnaRadaProjektu>();
                            projekty.VybranaCiselnaRadaList.Add(cisrada);
                        }
                        else
                        {
                            var isinlist = false;
                            foreach (var itemx in projekty.VybranaCiselnaRadaList)
                            {
                                if (itemx.Kod == ciselnarada)
                                {
                                    isinlist = true;
                                    var x = itemx;
                                    projekty.VybranaCiselnaRadaList.Remove(x);
                                    projekty.VybranaCiselnaRadaList.RemoveAll(t => t.Kod == itemx.Kod);
                                }
                            }
                            if (isinlist == false)
                            { projekty.VybranaCiselnaRadaList.Add(cisrada); }
                        }

                    }
                    catch (Exception ex) { }
                }
                if (String.IsNullOrEmpty(stavyprojektu) == false)
                {
                    StavyProjektu stavpr = new StavyProjektu();
                    stavpr.Kod = Convert.ToString(stavyprojektu);
                    try
                    {
                        if (projekty.VybraneStavyProjektuList == null)
                        {
                            //It's null - create it
                            projekty.VybraneStavyProjektuList = new List<StavyProjektu>();
                            projekty.VybraneStavyProjektuList.Add(stavpr);
                        }
                        else
                        {
                            var isinlist = false;
                            foreach (var itemx in projekty.VybraneStavyProjektuList)
                            {
                                if (itemx.Kod == stavyprojektu)
                                {
                                    isinlist = true;
                                    var x = itemx;
                                    projekty.VybraneStavyProjektuList.Remove(x);
                                    projekty.VybraneStavyProjektuList.RemoveAll(t => t.Kod == itemx.Kod);
                                }
                            }
                            if (isinlist == false)
                            { projekty.VybraneStavyProjektuList.Add(stavpr); }
                        }

                    }
                    catch (Exception ex) { }
                }
                if (String.IsNullOrEmpty(typprojektu) == false)
                {
                    TypProjektu typproj = new TypProjektu();
                    typproj.Kod = Convert.ToString(typprojektu);
                    try
                    {
                        if (projekty.VybraneTypyProjektuList == null)
                        {
                            //It's null - create it
                            projekty.VybraneTypyProjektuList = new List<TypProjektu>();
                            projekty.VybraneTypyProjektuList.Add(typproj);
                        }
                        else
                        {
                            var isinlist = false;
                            foreach (var itemx in projekty.VybraneTypyProjektuList)
                            {
                                if (itemx.Kod == typprojektu)
                                {
                                    isinlist = true;
                                    var x = itemx;
                                    projekty.VybraneTypyProjektuList.Remove(x);
                                    projekty.VybraneTypyProjektuList.RemoveAll(t => t.Kod == itemx.Kod);
                                }
                            }
                            if (isinlist == false)
                            { projekty.VybraneTypyProjektuList.Add(typproj); }
                        }

                    }
                    catch (Exception ex) { }
                }
            }

            //write to session
            Session["VybraneRokyList"] = projekty.VybraneRokyList;
            Session["VybranaCiselnaRadaList"] = projekty.VybranaCiselnaRadaList;
            Session["VybraneRegionyList"] = projekty.VybraneRegionyList;
            Session["VybraneStavyProjektuList"] = projekty.VybraneStavyProjektuList;
            Session["VybraneTypyProjektuList"] = projekty.VybraneTypyProjektuList;
            Session["VybraniPracovniciList"] = projekty.VybraniPracovniciList;
            Session["VybranaSkupinaArtikluList"] = projekty.VybranaSkupinaArtikluList;

            projekty = Projekty.GetProjectList(projekty);
            return View(projekty);
        }
        [Authorize(Roles = "Administrator,Vedení")]
        public ActionResult Details(Projekt project)
        {
            
                ORDRforProject ordrfp = new ORDRforProject();
                ordrfp.Projekt = project.Code;
                ordrfp.ProjektName = project.Name;
                
            
            return PartialView("Details", ordrfp);
        }
        [Authorize(Roles = "Administrator,Vedení")]
        public ActionResult DetailsOinv(Projekt project)
        {
            
                OINVforProject ordrfp = new OINVforProject();
                ordrfp.Projekt = project.Code;
                ordrfp.ProjektName = project.Name;
                
            return PartialView("DetailsOinv", ordrfp);
        }
        [Authorize(Roles = "Administrator,Vedení")]
        public ActionResult DetailsOpch(Projekt project)
        {
            
                OPCHforProject ordrfp = new OPCHforProject();
                ordrfp.Projekt = project.Code;
                ordrfp.ProjektName = project.Name;
                
            return PartialView("DetailsOpch", ordrfp);
        }
    }
}