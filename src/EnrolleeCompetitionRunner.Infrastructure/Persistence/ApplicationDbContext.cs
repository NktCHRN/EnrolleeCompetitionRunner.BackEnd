using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EnrolleeCompetitionRunner.Infrastructure.Persistence;
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Enrollee> Enrollees { get; set; }

    public DbSet<OfferEnrollee> OfferEnrollees { get; set; }

    public DbSet<Offer> Offers { get; set; }

    public DbSet<Speciality> Specialities { get; set; }

    public DbSet<Supercompetition> SuperCompetitions { get; set; }

    public DbSet<University> Universities { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IInfrastructureAssemblyMarker).Assembly);
    }

    public async Task DeleteAllDataAsync()
    {
        await Enrollees.ExecuteDeleteAsync();
        await OfferEnrollees.ExecuteDeleteAsync();
        await Offers.ExecuteDeleteAsync();
        await Specialities.ExecuteDeleteAsync();
        await SuperCompetitions.ExecuteDeleteAsync();
        await Universities.ExecuteDeleteAsync();
    }

    public async Task RecreateDatabaseAsync()
    {
        await Database.EnsureDeletedAsync();
        await Database.EnsureCreatedAsync();
    }

    public async Task SaveChangesAsync()
    {
        await ((DbContext)this).SaveChangesAsync();
    }

    public async Task BulkSaveChangesAsync()
    {
        await ((DbContext)this).BulkSaveChangesAsync();
    }

    public async Task BulkInsertAsync<T>(IEnumerable<T> entities) where T : class
    {
        await ((DbContext)this).BulkInsertAsync(entities);
    }

    public async Task BulkUpdateAsync<T>(IEnumerable<T> entities) where T : class
    {
        await ((DbContext)this).BulkUpdateAsync(entities);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await Database.BeginTransactionAsync();
    }
}
