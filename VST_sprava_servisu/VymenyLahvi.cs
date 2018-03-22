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

    public partial class VymenyLahvi
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "S�riov� ��slo p�vodn�ho AP")]
        public int SCProvozuPuvodni { get; set; }
        [Display(Name = "S�riov� ��slo nov�ho AP")]
        public int SCProvozuNova { get; set; }
        [Display(Name = "S�riov� ��slo lahve nov�ho AP")]
        public string SCLahve { get; set; }
        [Display(Name = "Datum v�m�ny")]
        public System.DateTime DatumVymeny { get; set; }
        [Display(Name = "Revize")]
        public int Revize { get; set; }
        [Display(Name = "Popis")]
        public string Popis { get; set; }
        [Display(Name = "Um�st�n�")]
        public Nullable<int> Umisteni { get; set; }
        public bool Repase { get; set; }
        public virtual SCProvozu SCProvozu { get; set; }
        public virtual SCProvozu SCProvozu1 { get; set; }
        public virtual Revize Revize1 { get; set; }
    }
}
