using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Domain.Abstractions;
public interface ISupercompetitionRunner
{
    void Run(IEnumerable<Supercompetition> superCompetitions);
}
