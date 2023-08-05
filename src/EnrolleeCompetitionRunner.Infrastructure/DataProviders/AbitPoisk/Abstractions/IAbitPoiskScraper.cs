using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.ScrapedResultModels;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Abstractions;
public interface IAbitPoiskScraper
{
    AbitPoiskResultOffer ScrapOfferPage(string rawHtml, string offerCode);
}
