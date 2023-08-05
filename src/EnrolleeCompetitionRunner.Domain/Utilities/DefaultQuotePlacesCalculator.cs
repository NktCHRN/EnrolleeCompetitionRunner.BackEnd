using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Constants;

namespace EnrolleeCompetitionRunner.Domain.Utilities;
public sealed class DefaultQuotePlacesCalculator : IDefaultQuotePlacesCalculator
{
    public int CalculateQuote1Places(int budgetPlaces)
    {
        var places = (int)Math.Ceiling(budgetPlaces * OfferConstants.DefaultQuote1PlacesRate);
        if (places < 1) 
        {
            places = 1;
        }

        return places;
    }

    public int CalculateQuote2Places(int budgetPlaces)
    {
        var places = (int)Math.Ceiling(budgetPlaces * OfferConstants.DefaultQuote2PlacesRate);
        if (places < 1)
        {
            places = 1;
        }

        return places;
    }
}
