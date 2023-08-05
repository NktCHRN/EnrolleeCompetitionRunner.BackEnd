using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedResultModels;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Abstractions;
public interface IEdboScraper
{
    IReadOnlyList<EdboResultSupercompetition> ScrapSupercompetitionPage(string rawHtml);
}
