namespace EnrolleeCompetitionRunner.Domain.Abstractions;
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
