namespace EnrolleeCompetitionRunner.Core.Dtos;
public sealed record EnrolleesSearchResultsDto
{
    public IEnumerable<EnrolleeDto> Enrollees { get; set; } = Array.Empty<EnrolleeDto>();
}
