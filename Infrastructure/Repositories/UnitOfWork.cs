using Application.Interfaces.Repositories;
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

        public UnitOfWork(
            AppDbContext context,
            IRegistrationRequestRepository registrationRequests,
            IMemberRepository members,
            IDashboardAccountRepository dashboardAccounts,
            IEntryLogRepository entryLogs
            )
        {
            _context = context;
            RegistrationRequests = registrationRequests;
            Members = members;
            DashboardAccounts = dashboardAccounts;
            EntryLogs = entryLogs;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}