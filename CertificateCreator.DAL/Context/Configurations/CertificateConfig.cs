using CertificateCreator.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CertificateCreator.DAL.Context.Configurations {
    public class CertificateConfig : IEntityTypeConfiguration<Certificate> {
        public void Configure(EntityTypeBuilder<Certificate> builder) {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");

            builder.Property(c => c.PDFCertificate)
                .HasColumnType("varbinary(max)");
        }
    }
}
