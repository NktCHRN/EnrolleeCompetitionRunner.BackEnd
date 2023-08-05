using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace EnrolleeCompetitionRunner.Core.Commands;
public sealed class RunCompetitionCommandHandler : IRequestHandler<RunCompetitionCommand>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ISupercompetitionRunner _supercompetitionRunner;
    private readonly ILogger<RunCompetitionCommandHandler> _logger;

    public RunCompetitionCommandHandler(IApplicationDbContext applicationDbContext, ISupercompetitionRunner supercompetitionRunner, ILogger<RunCompetitionCommandHandler> logger)
    {
        _applicationDbContext = applicationDbContext;
        _supercompetitionRunner = supercompetitionRunner;
        _logger = logger;
    }

    public async Task Handle(RunCompetitionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving the data from DB");
        var supercompetitions = await _applicationDbContext.SuperCompetitions
            .Include(sc => sc.Specialities)
                .ThenInclude(s => s.Offers)
                .ThenInclude(o => o.Enrollees)
                .ThenInclude(e => e.Enrollee)
                .ThenInclude(e => e.Offers)
            .Include(sc => sc.Specialities)
                .ThenInclude(s => s.Offers)
                .ThenInclude(o => o.University)
            .ToListAsync();

        var offerEnrollees = supercompetitions
            .AsParallel()
            .SelectMany(sc => sc.Specialities)
            .SelectMany(s => s.Offers)
            .SelectMany(o => o.Enrollees)
            .ToList();
        foreach(var offerEnrollee in offerEnrollees)
        {
            offerEnrollee.ResetInitialRecommendation();
        }
        _logger.LogInformation("Running the competition");

        _supercompetitionRunner.Run(supercompetitions);

        _logger.LogInformation("Saving data to the DB");
        await _applicationDbContext.BulkUpdateAsync(offerEnrollees);
    }
}
