using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Domain.UtilityModels;
public record struct SwitchToNextPriorityResult(OfferEnrollee? NextPriorityOffer);
