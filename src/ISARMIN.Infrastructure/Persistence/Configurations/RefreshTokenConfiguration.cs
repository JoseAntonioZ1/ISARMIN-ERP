using ISARMIN.Domain.Entities.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.TokenHash)
            .HasMaxLength(255)
            .IsRequired();
        builder.HasIndex(rt => rt.TokenHash).IsUnique();

        builder.Property(rt => rt.CreadoEnUtc).IsRequired();
        builder.Property(rt => rt.ExpiraEnUtc).IsRequired();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(rt => rt.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
