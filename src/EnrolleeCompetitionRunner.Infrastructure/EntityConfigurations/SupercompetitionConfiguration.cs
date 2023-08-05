using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrolleeCompetitionRunner.Infrastructure.EntityConfigurations;
public sealed class SupercompetitionConfiguration : IEntityTypeConfiguration<Supercompetition>
{
    public void Configure(EntityTypeBuilder<Supercompetition> builder)
    {
        builder.Property(b => b.Id)
            .HasDefaultValueSql("newsequentialid()");

        builder.Property(b => b.EnrollmentBasis)
            .HasConversion<string>();
        builder.Property(b => b.EducationalStage)
            .HasConversion<string>();
    }
}
