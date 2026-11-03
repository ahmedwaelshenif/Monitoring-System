using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IRegistrationRequestRepository RegistrationRequests { get; }
        IMemberRepository Members { get; }
        IDashboardAccountRepository DashboardAccounts { get; }

        IEntryLogRepository EntryLogs { get; }
        Task<int> SaveChangesAsync();
    }
}
