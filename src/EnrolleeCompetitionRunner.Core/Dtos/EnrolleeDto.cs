namespace EnrolleeCompetitionRunner.Core.Dtos;
public sealed record EnrolleeDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal UkrainianLanguageExamScore { get; set; }

    public IEnumerable<OfferEnrolleeDto> Offers { get; set; } = Array.Empty<OfferEnrolleeDto>();
}
