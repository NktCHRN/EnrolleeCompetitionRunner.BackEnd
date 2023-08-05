using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Domain.Extensions;
public static class OfferEnrolleesLinqExtensions
{
    public static IOrderedEnumerable<OfferEnrollee> OrderByScoreAndPriority(this IEnumerable<OfferEnrollee> offerEnrollees)
        => offerEnrollees
            .OrderByDescending(e => e.Score)
            .ThenBy(e => e.Priority);
}
