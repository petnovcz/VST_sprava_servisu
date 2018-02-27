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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SerioveCislo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SerioveCislo()
        {
            this.SCProvozu = new HashSet<SCProvozu>();
        }
    
        public int Id { get; set; }
        [Display(Name = "Artikl")]
        public int ArtiklId { get; set; }
        [Required, Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum v�roby")]
        public Nullable<System.DateTime> DatumVyroby { get; set; }
        [Required, Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Posledn� tlakov� zkou�ka")]
        public Nullable<System.DateTime> DatumPosledniTlakoveZkousky { get; set; }
        [Display(Name = "S�riov� ��slo")]
        public string SerioveCislo1 { get; set; }
    
        public virtual Artikl Artikl { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCProvozu> SCProvozu { get; set; }

        public int provoz { get; set; }
        public int umisteni { get; set; }
        public int zakaznik { get; set; }
    }
}
