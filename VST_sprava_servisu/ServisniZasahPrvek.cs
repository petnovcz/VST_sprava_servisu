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

    public partial class ServisniZasahPrvek
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServisniZasahPrvek()
        {
            this.ServisniZasahPrvekSarze = new HashSet<ServisniZasahPrvekSarze>();
            this.ServisniZasahPrvekSerioveCislo = new HashSet<ServisniZasahPrvekSerioveCislo>();
            
        }
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Servisn� Z�sah")]
        public int ServisniZasahId { get; set; }
        [Display(Name = "S�riov� ��slo provozu")]
        public Nullable<int> SCProvozuID { get; set; }
        [Display(Name = "Porucha")]
        public int PoruchaID { get; set; }
        [Display(Name = "Artikl")]
        public Nullable<int> ArtiklID { get; set; }
        [Display(Name = "Mno�stv�")]
        public decimal Pocet { get; set; }
        [Display(Name = "Cena za kus")]
        public Nullable<decimal> CenaZaKus { get; set; }
        [Display(Name = "Cena celkem")]
        public Nullable<decimal> CenaCelkem { get; set; }
        [Display(Name = "Reklamace?")]
        public bool Reklamace { get; set; }
        [Display(Name = "Poru�en� z�ru�n�ch podm�nek")]
        public bool PoruseniZarucnichPodminek { get; set; }
    
        public virtual Artikl Artikl { get; set; }
        public virtual Porucha Porucha { get; set; }
        public virtual SCProvozu SCProvozu { get; set; }
        public virtual ServisniZasah ServisniZasah { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServisniZasahPrvekSarze> ServisniZasahPrvekSarze { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServisniZasahPrvekSerioveCislo> ServisniZasahPrvekSerioveCislo { get; set; }
    }
}
