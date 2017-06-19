using System.Data.Entity;
using BachelorThesis.Database.Models;

namespace BachelorThesis.Database
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public virtual DbSet<Feedback> Feedback { get; set; }

        public virtual DbSet<KnowledgeBase> KnowledgeBase { get; set; }

        public virtual DbSet<Logging> Logging { get; set; }

        public virtual DbSet<Users> Users { get; set; }

        public DbContext()
            : base("DefaultDbContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback>()
                .Property(e => e.RawText)
                .IsUnicode(false);

            modelBuilder.Entity<KnowledgeBase>()
                .Property(e => e.Question)
                .IsUnicode(false);

            modelBuilder.Entity<KnowledgeBase>()
                .Property(e => e.Answer)
                .IsUnicode(false);

            modelBuilder.Entity<KnowledgeBase>()
                .Property(e => e.Analysis)
                .IsUnicode(false);

            modelBuilder.Entity<KnowledgeBase>()
                .Property(e => e.PairChecksum)
                .IsUnicode(false);

            modelBuilder.Entity<KnowledgeBase>()
                .Property(e => e.Intent)
                .IsUnicode(false);

            modelBuilder.Entity<Logging>()
                .Property(e => e.RawText)
                .IsUnicode(false);

            modelBuilder.Entity<Logging>()
                .Property(e => e.TranslateJson)
                .IsUnicode(false);

            modelBuilder.Entity<Logging>()
                .Property(e => e.QnAMakerJson)
                .IsUnicode(false);

            modelBuilder.Entity<Logging>()
                .Property(e => e.LuisJson)
                .IsUnicode(false);

            modelBuilder.Entity<Logging>()
                .Property(e => e.AnalysisJson)
                .IsUnicode(false);

            modelBuilder.Entity<Logging>()
                .Property(e => e.CustomJson)
                .IsUnicode(false);

            modelBuilder.Entity<Logging>()
                .HasMany(e => e.Feedback)
                .WithRequired(e => e.Logging)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Email)
                .IsUnicode(false);
        }
    }
}
