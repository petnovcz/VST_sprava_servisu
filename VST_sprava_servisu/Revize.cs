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
    using System.Web.Mvc;

    public partial class Revize
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Revize()
        {
            this.RevizeSC = new HashSet<RevizeSC>();
            this.RevizeBaterie = new HashSet<RevizeBaterie>();
        }
    
        public int Id { get; set; }
        [Display(Name = "Provoz")]
        public int ProvozId { get; set; }
        [Required, Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum revize")]
        public System.DateTime DatumRevize { get; set; }
        [Display(Name = "Status revize")]
        public int StatusRevizeId { get; set; }
        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Datum vystaven�")]
        public Nullable<System.DateTime> DatumVystaveni { get; set; }
        [AllowHtml]
        [Display(Name = "Zji�t�n� stav")]
        public string ZjistenyStav { get; set; }
        [AllowHtml]
        [Display(Name = "Proveden� z�sahy")]
        public string ProvedeneZasahy { get; set; }
        [AllowHtml]
        [Display(Name = "Opat�en� k odstran�n�")]
        public string OpatreniKOdstraneni { get; set; }
        [Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}"), Display(Name = "Kontrola provedena dne")]
        public Nullable<System.DateTime> KontrolaProvedenaDne { get; set; }
        [Display(Name = "P��t� kontrola")]
        public string PristiKontrola { get; set; }
        public Nullable<int> Rok { get; set; }
        public Nullable<int> Pololeti { get; set; }
        public Nullable<int> UmisteniId { get; set; }
        [Display(Name = "Po�et bateri�")]
        public Nullable<int> Baterie { get; set; }
        [Display(Name = "Po�et pyro")]
        public Nullable<int> Pyro { get; set; }
        [Display(Name = "Po�et tlakov�ch zkou�ek")]
        public Nullable<int> TlkZk { get; set; }
        public Nullable<int> AP { get; set; }
        public Nullable<int> S { get; set; }
        public Nullable<int> RJ { get; set; }
        public Nullable<int> M { get; set; }
        public Nullable<int> V { get; set; }
        [Display(Name = "Projekt")]
        public string Projekt { get; set; }
        [Display(Name = "Nab�dka")]
        public string Nabidka { get; set; }
    
        public virtual Provoz Provoz { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RevizeSC> RevizeSC { get; set; }
        public virtual StatusRevize StatusRevize { get; set; }
        public virtual Umisteni Umisteni { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VymenyLahvi> VymenyLahvi { get; set; }
    }
}
