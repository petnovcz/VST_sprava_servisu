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

    public partial class Provoz
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Provoz()
        {
            this.Umisteni = new HashSet<Umisteni>();
            this.Revize = new HashSet<Revize>();
            this.SCProvozu = new HashSet<SCProvozu>();
            this.KontakniOsoba = new HashSet<KontakniOsoba>();
            this.ServisniZasah = new HashSet<ServisniZasah>();
        }
    
        public int Id { get; set; }
        [Display(Name = "Z�kazn�k")]
        public int ZakaznikId { get; set; }
        [Display(Name = "N�zev provozu")]
        public string NazevProvozu { get; set; }
        [Display(Name = "Odd�len� v�buchu")]
        public bool OddeleniVybuchu { get; set; }
        [Display(Name = "Potla�en� v�buchu")]
        public bool PotlaceniVybuchu { get; set; }
        [Display(Name = "Odleh�en� v�buchu")]
        public bool OdlehceniVybuchu { get; set; }
        [Display(Name = "Adresa provozu")]
        public string AdresaProvozu { get; set; }
        [Display(Name = "SAP k�d adresy")]
        public string SAPAddress { get; set; }
        [Display(Name = "Pou�ij adresu v tisku")]
        public bool PouzijVTisku { get; set; }
        [Display(Name = "I�")]
        public string IC { get; set; }
        [Display(Name = "DI�")]
        public string DIC { get; set; }

        public virtual Zakaznik Zakaznik { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Umisteni> Umisteni { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Revize> Revize { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCProvozu> SCProvozu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KontakniOsoba> KontakniOsoba { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServisniZasah> ServisniZasah { get; set; }
        
    }
}
