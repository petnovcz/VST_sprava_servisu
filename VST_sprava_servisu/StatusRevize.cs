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
    
    public partial class StatusRevize
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StatusRevize()
        {
            this.Revize = new HashSet<Revize>();
        }
    
        public int Id { get; set; }
        public string NazevStatusuRevize { get; set; }
        public bool Planovana { get; set; }
        public bool Potvrzena { get; set; }
        public bool Realizovana { get; set; }
        public bool Stornovana { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Revize> Revize { get; set; }
    }
}
