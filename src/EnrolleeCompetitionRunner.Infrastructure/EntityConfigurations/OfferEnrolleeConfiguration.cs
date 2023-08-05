using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrolleeCompetitionRunner.Infrastructure.EntityConfigurations;
public sealed class OfferEnrolleeConfiguration : IEntityTypeConfiguration<OfferEnrollee>
{
    public void Configure(EntityTypeBuilder<OfferEnrollee> builder)
    {
        builder.Property(b => b.Id)
            .HasDefaultValueSql("newsequentialid()");

        builder.Ignore(b => b.IsWaiting);

        builder.Property(b => b.SubcompetitionType)
            .HasConversion<string>();

        builder.Property(b => b.Score)
            .HasPrecision(18, 4);
    }
}
