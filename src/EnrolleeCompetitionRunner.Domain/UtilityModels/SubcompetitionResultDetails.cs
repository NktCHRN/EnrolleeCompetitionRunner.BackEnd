using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Domain.UtilityModels;
public record struct SubcompetitionResultDetails(
    IEnumerable<OfferEnrollee> PassedOfferEnrollees, 
    IEnumerable<OfferEnrollee> NotPassedOfferEnrollees, 
    int FreePlaces,
    int OccupiedPlaces);
