using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrolleeCompetitionRunner.Infrastructure.EntityConfigurations;
public sealed class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.Property(b => b.Id)
            .HasDefaultValueSql("newsequentialid()");
    }
}
