using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Domain.UtilityModels;
public record struct CompetitionResultDetails(IEnumerable<OfferEnrollee> WaitingOfferEnrollees);
