﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{


    public class SAPOPImportParametr
    {
        [Key]
        public string Search { get; set; }


        public List<SAPOP> ListSAPOP { get; set; }

    }

    public class SAPOP
    {

        [Key]
        public string CardCode { get; set; }
        public string CardName { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string LicTradNum { get; set; }
        public string VatIdUnCmp { get; set; }

        public int RegionId { get; set; }
        public int JazykId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }


        public int Open { get; set; }
        public int Total { get; set; }


    }

    public class SAPContactPerson
    {
        [Key]
        public int CntctCode { get; set; }
        public string CardCode { get; set; } 
        public string Name { get; set; }
        public string Position { get; set; }
        public string Tel1 { get; set; }
        public string Cellolar { get; set; }
        public string E_MaiL { get; set; }

    }

    public class SAPDeliveryAddress
    {
        [Key]
        public string Address { get; set; }
        public string CardCode { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City {get; set;}
        public string Country { get; set; }



    }

    public class SAPItem
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItmsGrpNam { get; set; }
        public int ItmsGrpCod { get; set; }

    


    }

    public class SAPSerioveCislo
    {
        [Key]
        public string SerioveCislo { get; set; }
        public int ArticlId { get; set; }
        public string NazevArtiklu { get; set; }
        public string KodSAP { get; set; }
        public DateTime DatumVyroby { get; set; }
        public DateTime DatumDodani { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int BaseType { get; set; }
        public int BaseNum { get; set; }
        public string PrjCode { get; set; }
        public string PrjName { get; set; }
        
        public string ZakaznikSAPKod { get; set; }
        public int Zakaznik { get; set; }
        public int ProvozId { get; set; }
        public IEnumerable<SelectListItem> Provoz { get; set; }
        

    }

    public class SCImport
    {
        [Display(Name ="Artikl")]
        public int ArtiklId { get; set; }

        public string ArtiklSAPKod { get; set; }
        [Key]
        [Display(Name = "Sériové číslo")]
        public string SerioveCislo { get; set; }
        [Display(Name = "Datum výroby")]
        public DateTime DatumVyroby { get; set; }
        [Display(Name = "Datum dodání")]
        public DateTime DatumDodani { get; set; }
        public string ZakaznikSAPKod { get; set; }
        public int Zakaznik { get; set; }
        public int Provozy { get; set; }
        public int Umisteni { get; set; }
        [Display(Name = "Lokace Prvku")]
        public string Lokace { get; set; }
        [Display(Name = "Značení prvku")]
        public string Znaceni { get; set; }
        [Display(Name = "Datum přiřazení")]
        public DateTime DatumPrirazeni { get; set; }
        
        public Nullable<DateTime> DatumPosledniZmeny { get; set; }
        
        public DateTime? DatumVymeny { get; set; }
        [Display(Name = "Datum poslední revize")]
        public Nullable<DateTime> DatumRevize { get; set; }
        [Display(Name = "Datum výměny baterie")]
        public Nullable<DateTime> DatumBaterie { get; set; }
        [Display(Name = "Datum výměny pyroiniciátorů")]
        public Nullable<DateTime> DatumPyro { get; set; }
        [Display(Name = "Datum tlakové zkoušky")]
        public Nullable<DateTime> DatumTlkZk { get; set; }
        public bool? Submitted { get; set; }
        [Display(Name = "Prověřit")]
        public bool Proverit { get; set; }
        [Display(Name = "Baterie [A -malá, N - velká]")]
        public bool Baterie { get; set; }
    }


    public class SCTest
    {
        [Key]
        public string SC { get; set; }
        public int Artikl { get; set; }
        public int Zakaznik { get; set; }
        public int Provoz { get; set; }
        public int Umisteni { get; set; }
        public IEnumerable<SAPSerioveCislo> SAPSerioveCIslo { get; set; }
        public IEnumerable<SerioveCislo> SerioveCisloList { get; set; }
        public IEnumerable<SCProvozu> SCProvozuList { get; set; }
    }
}