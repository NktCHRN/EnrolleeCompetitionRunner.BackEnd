using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EnrolleeCompetitionRunner.Core.Abstractions;
public interface IApplicationDbContext
{
    DbSet<Enrollee> Enrollees { get; }
    DbSet<OfferEnrollee> OfferEnrollees { get; set; }
    DbSet<Offer> Offers { get; }
    DbSet<Speciality> Specialities { get; }
    DbSet<Supercompetition> SuperCompetitions { get; }
    DbSet<University> Universities { get; }

    Task RecreateDatabaseAsync();

    Task SaveChangesAsync();

    Task BulkSaveChangesAsync();

    Task DeleteAllDataAsync();

    Task BulkInsertAsync<T>(IEnumerable<T> entities) where T : class;

    Task BulkUpdateAsync<T>(IEnumerable<T> entities) where T : class;

    Task<IDbContextTransaction> BeginTransactionAsync();
}
