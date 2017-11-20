using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

}