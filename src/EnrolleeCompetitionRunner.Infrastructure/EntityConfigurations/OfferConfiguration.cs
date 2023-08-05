using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrolleeCompetitionRunner.Infrastructure.EntityConfigurations;
public sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.Property(b => b.Id)
            .HasDefaultValueSql("newsequentialid()");

        builder.Property(b => b.EnrollmentBasis)
            .HasConversion<string>();
        builder.Property(b => b.EducationalStage)
            .HasConversion<string>();

        builder.HasIndex(b => b.Code)
            .IsUnique();
    }
}
