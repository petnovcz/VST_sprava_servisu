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
        public string ZakaznikSAPKod { get; set; }
        public int Zakaznik { get; set; }
        public int ProvozId { get; set; }
        public IEnumerable<SelectListItem> Provoz { get; set; }
        

    }

    public class SCImport
    {
        public int ArtiklId { get; set; }
        public string ArtiklSAPKod { get; set; }
        [Key]
        public string SerioveCislo { get; set; }
        public DateTime DatumVyroby { get; set; }
        public DateTime DatumDodani { get; set; }
        public string ZakaznikSAPKod { get; set; }
        public int Zakaznik { get; set; }
        public int Provozy { get; set; } 



    }
}