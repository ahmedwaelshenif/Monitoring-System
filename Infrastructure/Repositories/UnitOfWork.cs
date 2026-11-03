using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IRegistrationRequestRepository RegistrationRequests { get; }
        public IMemberRepository Members { get; }
        public IDashboardAccountRepository DashboardAccounts { get; }
        public IEntryLogRepository EntryLogs { get; }
        public ILabRepository Labs { get; }

        public IWarningRepository Warnings { get; }

        public UnitOfWork(
            AppDbContext context,
            IRegistrationRequestRepository registrationRequests,
            IMemberRepository members,
            IDashboardAccountRepository dashboardAccounts,
            IEntryLogRepository entryLogs,
            ILabRepository labs,
            IWarningRepository warnings
            )
        {
            _context = context;
            RegistrationRequests = registrationRequests;
            Members = members;
            DashboardAccounts = dashboardAccounts;
            EntryLogs = entryLogs;
            Labs = labs;
            Warnings = warnings;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
