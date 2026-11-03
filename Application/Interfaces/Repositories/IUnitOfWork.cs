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
        ILabRepository Labs { get; }

        IWarningRepository Warnings { get; }
        Task<int> SaveChangesAsync();
    }
}
