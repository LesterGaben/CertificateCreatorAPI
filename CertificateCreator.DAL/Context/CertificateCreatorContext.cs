using CertificateCreator.DAL.Context.Configurations;
using CertificateCreator.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CertificateCreator.DAL.Context {
    public class CertificateCreatorContext(DbContextOptions<CertificateCreatorContext> options) : DbContext(options) {

        public virtual DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(new CertificateConfig());
        }
    }
}
