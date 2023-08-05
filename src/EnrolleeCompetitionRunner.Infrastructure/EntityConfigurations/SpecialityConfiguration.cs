using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrolleeCompetitionRunner.Infrastructure.EntityConfigurations;
public sealed class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder.Property(b => b.Id)
            .HasDefaultValueSql("newsequentialid()");

        builder.HasIndex(b => new { b.Code, b.SpecializationCode })
            .IsUnique();
    }
}
