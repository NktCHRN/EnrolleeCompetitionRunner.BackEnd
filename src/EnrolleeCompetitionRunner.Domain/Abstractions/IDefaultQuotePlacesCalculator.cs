namespace EnrolleeCompetitionRunner.Domain.Abstractions;
public interface IDefaultQuotePlacesCalculator
{
    public int CalculateQuote1Places(int budgetPlaces);

    public int CalculateQuote2Places(int budgetPlaces);
}
