﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace qlvb.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class qlvbEntities : DbContext
    {
        public qlvbEntities()
            : base("name=qlvbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<cat1> cat1 { get; set; }
        public DbSet<cat2> cat2 { get; set; }
        public DbSet<cat3> cat3 { get; set; }
        public DbSet<cat4> cat4 { get; set; }
        public DbSet<dic_ignore> dic_ignore { get; set; }
        public DbSet<dic_normal> dic_normal { get; set; }
        public DbSet<dic_pro> dic_pro { get; set; }
        public DbSet<member> members { get; set; }
        public DbSet<log> logs { get; set; }
        public DbSet<document_items> document_items { get; set; }
        public DbSet<document> documents { get; set; }
    }
}
