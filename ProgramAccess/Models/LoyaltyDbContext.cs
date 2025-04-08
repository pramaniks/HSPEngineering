using Microsoft.EntityFrameworkCore;
using ProgramAccess.Models;
using System.Xml.Linq;

namespace ProgramAccess.Model
{
    public class LoyaltyDBContext : DbContext
    {
        public LoyaltyDBContext(DbContextOptions options)
        : base(options)
        {
        }

        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<ProgramCurrency> ProgramCurrencies { get; set; }
        public virtual DbSet<ProgramSecurityParameters> ProgramSecurityParameters { get; set; }
        public virtual DbSet<OTPConfigurations> OTPConfigurations { get; set; }
        public virtual DbSet<ProgramCurrencyCapping> ProgramCurrencyCappings { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            ProgramModelBuilder(modelBuilder);
        }

        private void ProgramModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Program>(p =>
            {
                p.HasIndex(pm => pm.ProgramId);
                p.HasIndex(pm => pm.ProgramName);
                //p.Property(pm => pm.CreatedOn).HasColumnType("timestamp");
                //p.Property(pm => pm.UpdatedOn).HasColumnType("timestamp");
            });

            modelBuilder.Entity<ProgramCurrency>(p =>
            {
                p.HasIndex(pm => pm.ProgramId);
                p.HasIndex(pm => pm.Name);
                //p.Property(pm => pm.CreatedOn).HasColumnType("timestamp");
                //p.Property(pm => pm.UpdatedOn).HasColumnType("timestamp");
                p.HasOne(pm => pm.Program).WithMany(pm => pm.Currencies).HasForeignKey(pm => pm.ProgramId);

            });

            modelBuilder.Entity<ProgramSecurityParameters>(p =>
            {
                //p.Property(pm => pm.CreatedOn).HasColumnType("timestamp");
                //p.Property(pm => pm.UpdatedOn).HasColumnType("timestamp");
                p.HasOne(pm => pm.Program).WithOne(pm => pm.SecurityParameter).HasForeignKey<ProgramSecurityParameters>(pm => pm.ProgramId).HasPrincipalKey<Program>(g => g.ProgramId);
            });

            modelBuilder.Entity<OTPConfigurations>(p =>
            {
                //p.Property(pm => pm.CreatedOn).HasColumnType("timestamp");
                //p.Property(pm => pm.UpdatedOn).HasColumnType("timestamp");
                p.HasOne(pm => pm.Program).WithOne(pm => pm.OtpConfigurations).HasForeignKey<OTPConfigurations>(pm => pm.ProgramId).HasPrincipalKey<Program>(g => g.ProgramId);
            });

            modelBuilder.Entity<ProgramCurrencyCapping>(p =>
            {
                p.HasKey(pm => pm.Id);
                p.HasIndex(pm => pm.ProgramCurrencyId);
                //p.Property(pm => pm.EffectiveDate).HasColumnType("timestamp");
                //p.Property(pm => pm.EndDate).HasColumnType("timestamp");
                //p.Property(pm => pm.CreatedOn).HasColumnType("timestamp");
                //p.Property(pm => pm.UpdatedOn).HasColumnType("timestamp");
                p.HasOne(pmc => pmc.Currency).WithMany(pmc => pmc.CurrencyCapping).HasForeignKey(e => e.ProgramCurrencyId);
            });
        }

    }
}
