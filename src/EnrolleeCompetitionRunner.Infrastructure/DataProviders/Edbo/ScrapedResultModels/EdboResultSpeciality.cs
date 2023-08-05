namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedResultModels;
public sealed record EdboResultSpeciality
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public string? SpecializationInternalCode { get; init; }
    public string? SpecializationCode { get; init; }
    public string? SpecializationName { get; init; }

    public EdboResultSupercompetition Supercompetition { get; init; } = new();
}
