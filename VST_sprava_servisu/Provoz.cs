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
    
    public partial class Provoz
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Provoz()
        {
            this.Umisteni = new HashSet<Umisteni>();
            this.Revize = new HashSet<Revize>();
            this.SCProvozu = new HashSet<SCProvozu>();
            this.KontakniOsoba = new HashSet<KontakniOsoba>();
        }
    
        public int Id { get; set; }
        public int ZakaznikId { get; set; }
        public string NazevProvozu { get; set; }
        public bool OddeleniVybuchu { get; set; }
        public bool PotlaceniVybuchu { get; set; }
        public bool OdlehceniVybuchu { get; set; }
        public string AdresaProvozu { get; set; }
        public string SAPAddress { get; set; }
    
        public virtual Zakaznik Zakaznik { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Umisteni> Umisteni { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Revize> Revize { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCProvozu> SCProvozu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KontakniOsoba> KontakniOsoba { get; set; }
    }
}
