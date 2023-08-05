using EnrolleeCompetitionRunner.Domain.Enums;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedResultModels;
public sealed record EdboResultSupercompetition
{
    public int TotalPlaces { get; init; }

    public string Code { get; init; } = string.Empty;

    public EducationalStage EnrollmentBasis { get; init; }
    public EducationalStage EducationalStage { get; init; }

    public IList<EdboResultSpeciality> Specialities { get; init; } = new List<EdboResultSpeciality>();
}
