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
    
    public partial class Revize
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Revize()
        {
            this.RevizeSC = new HashSet<RevizeSC>();
        }
    
        public int Id { get; set; }
        public int ProvozId { get; set; }
        public System.DateTime DatumRevize { get; set; }
        public int StatusRevizeId { get; set; }
        public System.DateTime DatumVystaveni { get; set; }
        public string ZjistenyStav { get; set; }
        public string ProvedeneZasahy { get; set; }
        public string OpatreniKOdstraneni { get; set; }
        public System.DateTime KontrolaProvedenaDne { get; set; }
        public string PristiKontrola { get; set; }
    
        public virtual Provoz Provoz { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RevizeSC> RevizeSC { get; set; }
        public virtual StatusRevize StatusRevize { get; set; }
    }
}