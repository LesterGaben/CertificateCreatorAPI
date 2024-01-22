using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CertificateCreator.DAL.Context {
    public class CertificateCreatorContextFactory : IDesignTimeDbContextFactory<CertificateCreatorContext> {
        public CertificateCreatorContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<CertificateCreatorContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=CertificateCreatorDB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new CertificateCreatorContext(optionsBuilder.Options);
        }
    }
}
