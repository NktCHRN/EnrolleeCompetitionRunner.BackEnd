using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnrolleeCompetitionRunner.Infrastructure.EntityConfigurations;
public sealed class EnrolleeConfiguration : IEntityTypeConfiguration<Enrollee>
{
    public void Configure(EntityTypeBuilder<Enrollee> builder)
    {
        builder.Property(b => b.Id)
            .HasDefaultValueSql("newsequentialid()");

        builder.Ignore(b => b.CurrentPriorityOffer);

        builder.HasIndex(b => new { b.Name, b.UkrainianLanguageExamScore })
            .IsUnique();

        builder.Property(b => b.UkrainianLanguageExamScore)
            .HasPrecision(18, 4);
    }
}
