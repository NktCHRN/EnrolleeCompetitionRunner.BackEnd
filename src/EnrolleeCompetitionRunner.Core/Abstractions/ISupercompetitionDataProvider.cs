using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Core.Abstractions;
public interface ISupercompetitionDataProvider
{
    Task<IReadOnlyList<Supercompetition>> GetSupercompetitionsAsync();
}
