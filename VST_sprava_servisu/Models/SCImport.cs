using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class SCImport
    {
        [Display(Name = "Artikl")]
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

        [Display(Name = "Artikl baterie")]
        public Nullable<int> BaterieArtikl { get; set; }

        [Display(Name = "Upravená perioda revize prvku")]
        public Nullable<int> UpravenaPeriodaRevize { get; set; }

        [Display(Name = "Upravená perioda výměny baterie")]
        public Nullable<int> UpravenaPeriodaBaterie { get; set; }

        [Display(Name = "Upravená perioda výměny pyro")]
        public Nullable<int> UpravenaPeriodaPyro { get; set; }

        [Display(Name = "Upravená perioda tlakové zkoušky")]
        public Nullable<int> UpravenaPeriodaTlkZk { get; set; }

        [Display(Name = "DatumRevizeTlakoveNadoby")]
        public Nullable<System.DateTime> DatumRevizeTlakoveNadoby { get; set; }
        [Display(Name = "DatumVnitrniRevizeTlakoveNadoby")]
        public Nullable<System.DateTime> DatumVnitrniRevizeTlakoveNadoby { get; set; }
        [Display(Name = "UpravenaPeriodaRevizeTlakoveNadoby")]
        public Nullable<int> UpravenaPeriodaRevizeTlakoveNadoby { get; set; }
        [Display(Name = "UpravenaPeriodaVnitrniRevizeTlakoveNadoby")]
        public Nullable<int> UpravenaPeriodaVnitrniRevizeTlakoveNadoby { get; set; }

        [Display(Name = "SCLahve")]
        public string SCLahve { get; set; }


    }
}