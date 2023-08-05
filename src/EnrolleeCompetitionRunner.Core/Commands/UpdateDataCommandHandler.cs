using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EnrolleeCompetitionRunner.Core.Commands;
public sealed class UpdateDataCommandHandler : IRequestHandler<UpdateDataCommand>
{
    private readonly ISupercompetitionDataProvider _supercompetitionDataProvider;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ISupercompetitionRunner _supercompetitionRunner;
    private readonly ILogger<UpdateDataCommandHandler> _logger;

    public UpdateDataCommandHandler(ISupercompetitionDataProvider supercompetitionDataProvider, IApplicationDbContext applicationDbContext, ISupercompetitionRunner supercompetitionRunner, ILogger<UpdateDataCommandHandler> logger)
    {
        _supercompetitionDataProvider = supercompetitionDataProvider;
        _applicationDbContext = applicationDbContext;
        _supercompetitionRunner = supercompetitionRunner;
        _logger = logger;
    }

    public async Task Handle(UpdateDataCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started retrieving the data from providers");
        var supercompetitions = await _supercompetitionDataProvider.GetSupercompetitionsAsync();

        var offerEnrollees = supercompetitions
            .AsParallel()
            .SelectMany(sc => sc.Specialities)
            .SelectMany(s => s.Offers)
            .SelectMany(o => o.Enrollees)
            .ToList();
        foreach (var offerEnrollee in offerEnrollees)
        {
            offerEnrollee.ResetInitialRecommendation();
        }
        _logger.LogInformation("Completed retrieving the data from providers");

        _logger.LogInformation("Started running the competition");
        _supercompetitionRunner.Run(supercompetitions);

        _logger.LogInformation("Completed running the competition");

        _logger.LogInformation("Started saving data to the DB");

        using (var transaction = await _applicationDbContext.BeginTransactionAsync())
        {
            await _applicationDbContext.DeleteAllDataAsync();

            await _applicationDbContext.BulkInsertAsync(supercompetitions);

            var specialities = supercompetitions.AsParallel().SelectMany(sc => sc.Specialities).ToList();
            foreach (var speciality in specialities)
            {
                speciality.SupercompetitionId = speciality.Supercompetition.Id;
            }
            await _applicationDbContext.BulkInsertAsync(specialities);

            var universities = supercompetitions.AsParallel().SelectMany(sc => sc.Specialities).SelectMany(s => s.Offers).Select(o => o.University).Distinct().ToList();
            await _applicationDbContext.BulkInsertAsync(universities);

            var offers = supercompetitions.AsParallel().SelectMany(sc => sc.Specialities).SelectMany(s => s.Offers).ToList();
            foreach(var offer in offers)
            {
                offer.UniversityId = offer.University.Id;
                offer.SpecialityId = offer.Speciality.Id;
            }
            await _applicationDbContext.BulkInsertAsync(offers);

            var enrollees = supercompetitions.AsParallel().SelectMany(sc => sc.Specialities).SelectMany(s => s.Offers).SelectMany(o => o.Enrollees).Select(oe => oe.Enrollee).Distinct().ToList();
            await _applicationDbContext.BulkInsertAsync(enrollees);

            offerEnrollees = supercompetitions.AsParallel().SelectMany(sc => sc.Specialities).SelectMany(s => s.Offers).SelectMany(o => o.Enrollees).ToList();
            foreach (var offerEnrollee in offerEnrollees)
            {
                offerEnrollee.EnrolleeId = offerEnrollee.Enrollee.Id;
                offerEnrollee.OfferId = offerEnrollee.Offer.Id;
            }
            await _applicationDbContext.BulkInsertAsync(offerEnrollees);

            transaction.Commit();
        }
        _logger.LogInformation("Completed saving data to the DB");
    }
}
