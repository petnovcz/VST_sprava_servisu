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
    
    public partial class ServisniZasahPrvekSarze
    {
        public int Id { get; set; }
        public int ServisniZasahPrvekId { get; set; }
        public string Sarze { get; set; }
        public string SAPKod { get; set; }
        public string Sklad { get; set; }
        public decimal Mnozstvi { get; set; }
    
        public virtual ServisniZasahPrvek ServisniZasahPrvek { get; set; }
    }
}
