using EnrolleeCompetitionRunner.Domain.Abstractions;

namespace EnrolleeCompetitionRunner.Domain.Utilities;
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
