﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Model1Container : DbContext
    {
        public Model1Container()
            : base("name=Model1Container")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Zakaznik> Zakaznik { get; set; }
        public virtual DbSet<Jazyk> Jazyk { get; set; }
        public virtual DbSet<Provoz> Provoz { get; set; }
        public virtual DbSet<Umisteni> Umisteni { get; set; }
        public virtual DbSet<Artikl> Artikl { get; set; }
        public virtual DbSet<Revize> Revize { get; set; }
        public virtual DbSet<RevizeSC> RevizeSC { get; set; }
        public virtual DbSet<SCProvozu> SCProvozu { get; set; }
        public virtual DbSet<SerioveCislo> SerioveCislo { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<StatusRevize> StatusRevize { get; set; }
        public virtual DbSet<KontakniOsoba> KontakniOsoba { get; set; }
        public virtual DbSet<SkupinaArtiklu> SkupinaArtiklu { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<VymenyLahvi> VymenyLahvi { get; set; }
        public virtual DbSet<Vozidlo> Vozidlo { get; set; }
        public virtual DbSet<KategoriePoruchy> KategoriePoruchy { get; set; }
        public virtual DbSet<Porucha> Porucha { get; set; }
        public virtual DbSet<ServisniZasahPrvek> ServisniZasahPrvek { get; set; }
        public virtual DbSet<ServisniZasah> ServisniZasah { get; set; }
        public virtual DbSet<ServisniZasahPrvekSarze> ServisniZasahPrvekSarze { get; set; }
        public virtual DbSet<ServisniZasahPrvekSerioveCislo> ServisniZasahPrvekSerioveCislo { get; set; }
        public virtual DbSet<Technici> Technici { get; set; }
        public virtual DbSet<TypPBZ> TypPBZ { get; set; }
        public virtual DbSet<UmisteniTypPBZ> UmisteniTypPBZ { get; set; }
    }
}
