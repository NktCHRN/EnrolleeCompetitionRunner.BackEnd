using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Domain.Utilities;
public sealed class SupercompetitionRunner : ISupercompetitionRunner
{
    public void Run(IEnumerable<Supercompetition> superCompetitions)
    {
        var enrollees = superCompetitions
            .AsParallel()
            .SelectMany(sc => sc.Specialities)
            .SelectMany(s => s.Offers)
            .SelectMany(o => o.Enrollees)
            .Select(e => e.Enrollee)
            .Distinct()
            .ToList();
        foreach (var enrollee in enrollees)
        {
            enrollee.NextPriorityOffer();
        }

        var waitingOfferEnrollees = new List<OfferEnrollee>();

        do
        {
            foreach (var enrollee in waitingOfferEnrollees)
            {
                enrollee.UnmarkAsWaiting();
            }
            waitingOfferEnrollees.Clear();

            foreach (var superCompetition in superCompetitions)
            {
                var superCompetitionResult = superCompetition.HandleCompetition();
                waitingOfferEnrollees.AddRange(superCompetitionResult.WaitingOfferEnrollees);
            }
        } while (waitingOfferEnrollees.Any());

        foreach (var offer in superCompetitions.SelectMany(sc => sc.Specialities).SelectMany(s => s.Offers))
        {
            offer.AwardBestEnrolleesWithScholarship();
        }
    }
}
