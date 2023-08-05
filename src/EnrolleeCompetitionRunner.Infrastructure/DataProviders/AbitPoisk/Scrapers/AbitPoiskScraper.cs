using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Abstractions;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Constants;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.ScrapedResultModels;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Scrapers;
public sealed class AbitPoiskScraper : IAbitPoiskScraper
{
    private readonly ILogger<AbitPoiskScraper> _logger;

    public AbitPoiskScraper(ILogger<AbitPoiskScraper> logger)
    {
        _logger = logger;
    }

    public AbitPoiskResultOffer ScrapOfferPage(string? rawHtml, string offerCode)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(rawHtml);

        return new AbitPoiskResultOffer
        {
            Quote1Places = ParseQuotePlaces(htmlDocument, OfferConstants.Quote1NodeName, offerCode),
            Quote2Places = ParseQuotePlaces(htmlDocument, OfferConstants.Quote2NodeName, offerCode)
        };
    }

    private int? ParseQuotePlaces(HtmlDocument htmlDocument, string quoteNodeName, string offerCode)
    {

        var quoteNode = htmlDocument.DocumentNode.Descendants()
            .FirstOrDefault(n => n.InnerText == quoteNodeName);
        var parseResult = int.TryParse(quoteNode?.ParentNode?.InnerText?.Replace(quoteNodeName, string.Empty) ?? string.Empty, out int quotePlaces);

        if (!parseResult)
        {
            _logger.LogWarning("Quote places ({QuoteNodeName}) were not parsed. Offer code: {OfferCode}", quoteNodeName, offerCode);
        }
        else
        {
            _logger.LogInformation("Quote places ({QuoteNodeName}) parsed successfully. Offer code: {OfferCode}", quoteNodeName, offerCode);
        }

        return parseResult ? quotePlaces : null;
    }
}
