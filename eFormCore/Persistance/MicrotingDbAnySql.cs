namespace eFormSqlController
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public partial class MicrotingDbAnySql : DbContext
    {
        public MicrotingDbAnySql() { }

        public MicrotingDbAnySql(DbContextOptions options)
          : base(options)
        {
           
        }
       
        public virtual DbSet<case_versions> case_versions { get; set; }
        public virtual DbSet<cases> cases { get; set; }
        public virtual DbSet<check_list_site_versions> check_list_site_versions { get; set; }
        public virtual DbSet<check_list_sites> check_list_sites { get; set; }
        public virtual DbSet<check_list_value_versions> check_list_value_versions { get; set; }
        public virtual DbSet<check_list_values> check_list_values { get; set; }
        public virtual DbSet<check_list_versions> check_list_versions { get; set; }
        public virtual DbSet<check_lists> check_lists { get; set; }
        public virtual DbSet<entity_group_versions> entity_group_versions { get; set; }
        public virtual DbSet<entity_groups> entity_groups { get; set; }
        public virtual DbSet<entity_item_versions> entity_item_versions { get; set; }
        public virtual DbSet<entity_items> entity_items { get; set; }
        public virtual DbSet<field_types> field_types { get; set; }
        public virtual DbSet<field_value_versions> field_value_versions { get; set; }
        public virtual DbSet<field_values> field_values { get; set; }
        public virtual DbSet<field_versions> field_versions { get; set; }
        public virtual DbSet<fields> fields { get; set; }
        public virtual DbSet<log_exceptions> log_exceptions { get; set; }
        public virtual DbSet<logs> logs { get; set; }
        public virtual DbSet<notifications> notifications { get; set; }
        public virtual DbSet<settings> settings { get; set; }
        public virtual DbSet<site_versions> site_versions { get; set; }
        public virtual DbSet<site_worker_versions> site_worker_versions { get; set; }
        public virtual DbSet<site_workers> site_workers { get; set; }
        public virtual DbSet<sites> sites { get; set; }
        public virtual DbSet<unit_versions> unit_versions { get; set; }
        public virtual DbSet<units> units { get; set; }
        public virtual DbSet<uploaded_data> uploaded_data { get; set; }
        public virtual DbSet<uploaded_data_versions> uploaded_data_versions { get; set; }
        public virtual DbSet<worker_versions> worker_versions { get; set; }
        public virtual DbSet<workers> workers { get; set; }
        public virtual DbSet<tags> tags { get; set; }
        public virtual DbSet<tag_versions> tag_versions { get; set; }
        public virtual DbSet<taggings> taggings { get; set; }
        public virtual DbSet<tagging_versions> tagging_versions { get; set; }

        public virtual Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade ContextDatabase
        {
            get => base.Database;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618

            modelBuilder.Entity<check_lists>(entity =>
            {
                entity.HasOne(d => d.parent).WithMany(p => p.children).HasForeignKey(d => d.parent_id);
            });

            modelBuilder.Entity<fields>(entity =>
            {
                entity.HasOne(d => d.parent).WithMany(p => p.children).HasForeignKey(d => d.parent_field_id);
            });

#pragma warning restore 612, 618
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(@"data source=(LocalDb)\SharedInstance;Initial catalog=eformsdk-tests;Integrated Security=True");
        //    }
        //}
    }
}