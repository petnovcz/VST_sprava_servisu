//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VST_sprava_servisu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Artikl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Artikl()
        {
            this.SerioveCislo = new HashSet<SerioveCislo>();
            this.SCProvozu = new HashSet<SCProvozu>();
            this.ServisniZasahPrvek = new HashSet<ServisniZasahPrvek>();
        }
        [Display(Name = "Artikl")]
        public int Id { get; set; }
        [Display(Name = "Artikl")]
        public string Nazev { get; set; }
        [Display(Name = "Ozna�en�")]
        public string Oznaceni { get; set; }
        [Display(Name = "Typ")]
        public string Typ { get; set; }
        [Display(Name = "Rozsah provozn�ch teplot")]
        public string RozsahProvoznichTeplot { get; set; }
        [Display(Name = "SAP k�d")]
        public string KodSAP { get; set; }
        [Display(Name = "Revize")]
        public bool Revize { get; set; }
        [Display(Name = "Perioda revize")]
        public string PeriodaRevize { get; set; }
        [Display(Name = "Tlakov� zkou�ka")]
        public bool TlakovaZk { get; set; }
        [Display(Name = "Perioda tlakov� zkou�ky")]
        public string PeriodaTlakovaZk { get; set; }
        [Display(Name = "V�m�na baterie")]
        public bool VymenaBaterie { get; set; }
        [Display(Name = "Perioda v�m�ny baterie")]
        public string PeriodaBaterie { get; set; }
        [Display(Name = "SAP k�d baterie")]
        public string ArtiklBaterieSAP { get; set; }
        [Display(Name = "V�m�na pyroinici�toru")]
        public bool VymenaPyro { get; set; }
        [Display(Name = "Perioda v�m�ny pyroinici�toru")]
        public string PeriodaPyro { get; set; }
        [Display(Name = "SAP k�d pyroinici�toru")]
        public string ArtoklPyro { get; set; }
        [Display(Name = "Skupina artiklu")]
        public Nullable<int> SkupinaArtiklu { get; set; }
        [Display(Name = "Tlakov� n�doba")]
        public bool TlakovaNadoba { get; set; }
        [Display(Name = "Perioda revize tlakov� n�doby")]
        public string PeriodaRevizeTlakoveNadoby { get; set; }
        [Display(Name = "Vnit�n� perioda revize")]
        public string PeriodaVnitrniRevize { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SerioveCislo> SerioveCislo { get; set; }
        public virtual SkupinaArtiklu SkupinaArtiklu1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCProvozu> SCProvozu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServisniZasahPrvek> ServisniZasahPrvek { get; set; }
    }
}
