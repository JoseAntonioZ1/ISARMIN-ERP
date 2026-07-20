using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Taller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class DiagnosticoConfiguration : IEntityTypeConfiguration<Diagnostico>
{
    public void Configure(EntityTypeBuilder<Diagnostico> builder)
    {
        builder.ToTable("diagnosticos");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Descripcion).HasColumnType("text").IsRequired();
        builder.Property(d => d.Fecha).IsRequired();

        builder.HasIndex(d => d.OrdenTrabajoId).IsUnique();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
