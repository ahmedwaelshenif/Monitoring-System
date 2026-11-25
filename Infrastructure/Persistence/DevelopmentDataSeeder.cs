using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHasher<DashboardAccount> passwordHasher,
        string demoPassword,
        CancellationToken cancellationToken = default)
    {
        var seedLabs = new[]
        {
            new Lab { Name = "Innovation Lab", Capacity = 24 },
            new Lab { Name = "Robotics Studio", Capacity = 18 },
            new Lab { Name = "Computer Science Lab", Capacity = 32 },
            new Lab { Name = "Design Workshop", Capacity = 20 },
            new Lab { Name = "Research Suite", Capacity = 16 },
            new Lab { Name = "Artificial Intelligence Lab", Capacity = 28 },
            new Lab { Name = "Networking Lab", Capacity = 22 },
            new Lab { Name = "Electronics Lab", Capacity = 20 },
            new Lab { Name = "Cybersecurity Lab", Capacity = 18 },
            new Lab { Name = "Multimedia Studio", Capacity = 26 }
        };

        var existingLabNames = (await db.Labs.Select(lab => lab.Name).ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingLabs = seedLabs.Where(lab => !existingLabNames.Contains(lab.Name)).ToArray();
        if (missingLabs.Length > 0)
        {
            db.Labs.AddRange(missingLabs);
            await db.SaveChangesAsync(cancellationToken);
        }

        var labs = await db.Labs.OrderBy(lab => lab.Id).ToListAsync(cancellationToken);
        if (labs.Count == 0)
            return;

        if (!await db.Visitors.AnyAsync(cancellationToken))
        {
            // Empty image/embedding payloads deliberately identify these as demo-only
            // records; they are not usable for face recognition.
            var visitors = new[]
            {
                new Visitor { MemberId = 240101, FullName = "Maya Hassan", Email = "maya.hassan@example.test", AcademicYear = 2, FaceImage = [], IdImage = [], EmbeddingData = [] },
                new Visitor { MemberId = 240102, FullName = "Omar Khaled", Email = "omar.khaled@example.test", AcademicYear = 3, FaceImage = [], IdImage = [], EmbeddingData = [] },
                new Visitor { MemberId = 240103, FullName = "Lina Mostafa", Email = "lina.mostafa@example.test", AcademicYear = 1, FaceImage = [], IdImage = [], EmbeddingData = [] },
                new Visitor { MemberId = 240104, FullName = "Youssef Adel", Email = "youssef.adel@example.test", AcademicYear = 4, FaceImage = [], IdImage = [], EmbeddingData = [] },
                new Visitor { MemberId = 240105, FullName = "Nour Ibrahim", Email = "nour.ibrahim@example.test", AcademicYear = 2, FaceImage = [], IdImage = [], EmbeddingData = [] },
                new Visitor { MemberId = 240106, FullName = "Adam Farouk", Email = "adam.farouk@example.test", AcademicYear = 3, FaceImage = [], IdImage = [], EmbeddingData = [] },
                new Visitor { MemberId = 240107, FullName = "Salma Nabil", Email = "salma.nabil@example.test", AcademicYear = 1, FaceImage = [], IdImage = [], EmbeddingData = [] },
                new Visitor { MemberId = 240108, FullName = "Ziad Samir", Email = "ziad.samir@example.test", AcademicYear = 4, FaceImage = [], IdImage = [], EmbeddingData = [] }
            };

            void AssignLabs(Visitor visitor, params int[] indexes)
            {
                foreach (var lab in indexes.Select(index => labs[Math.Min(index, labs.Count - 1)]).DistinctBy(lab => lab.Id))
                    visitor.Labs.Add(lab);
            }

            AssignLabs(visitors[0], 0, 1);
            AssignLabs(visitors[1], 0, 2);
            AssignLabs(visitors[2], 1);
            AssignLabs(visitors[3], 2);
            AssignLabs(visitors[4], 3);
            AssignLabs(visitors[5], 3);
            AssignLabs(visitors[6], 4);
            AssignLabs(visitors[7], 0);

            db.Visitors.AddRange(visitors);
            await db.SaveChangesAsync(cancellationToken);
        }

        var savedVisitors = await db.Visitors.OrderBy(visitor => visitor.Id).ToListAsync(cancellationToken);

        if (!await db.DashboardAccounts.AnyAsync(cancellationToken))
        {
            var admin = new DashboardAccount
            {
                FullName = "Demo Administrator",
                Email = "admin@example.test",
                Role = Role.Admin,
                IsActive = true,
                Labs = labs.ToList()
            };
            admin.PasswordHash = passwordHasher.HashPassword(admin, demoPassword);

            var coordinator = new DashboardAccount
            {
                FullName = "Lab Coordinator",
                Email = "coordinator@example.test",
                Role = Role.User,
                IsActive = true,
                Labs = labs.Take(2).ToList()
            };
            coordinator.PasswordHash = passwordHasher.HashPassword(coordinator, demoPassword);

            db.DashboardAccounts.AddRange(admin, coordinator);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (!await db.Registration_Requests.AnyAsync(cancellationToken))
        {
            var now = DateTime.Now;
            db.Registration_Requests.AddRange(
                new Registration_Request { MemberId = 240201, FullName = "Farah Mahmoud", Email = "farah.mahmoud@example.test", AcademicYear = 1, FaceImage = [], IdImage = [], status = RequestStatus.Pending, SubmittedAt = now.AddHours(-3) },
                new Registration_Request { MemberId = 240202, FullName = "Karim Tarek", Email = "karim.tarek@example.test", AcademicYear = 2, FaceImage = [], IdImage = [], status = RequestStatus.Pending, SubmittedAt = now.AddDays(-1) },
                new Registration_Request { MemberId = 240203, FullName = "Dina Ashraf", Email = "dina.ashraf@example.test", AcademicYear = 3, FaceImage = [], IdImage = [], status = RequestStatus.Rejected, SubmittedAt = now.AddDays(-3) });
            await db.SaveChangesAsync(cancellationToken);
        }

        if (!await db.EntryLogs.AnyAsync(cancellationToken))
        {
            var now = DateTime.Now;
            var logs = Enumerable.Range(0, 28).Select(index => new EntryLog
            {
                LabId = labs[index % labs.Count].Id,
                VisitorId = index % 7 == 0 ? null : savedVisitors[index % savedVisitors.Count].Id,
                EventTime = now.Date.AddDays(-(index / 5)).AddHours(8 + (index * 3 % 10)).AddMinutes(index * 7 % 60),
                Status = index % 6 == 0 ? EntryStatus.Rejected : EntryStatus.Accepted
            });
            db.EntryLogs.AddRange(logs);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (!await db.Warnings.AnyAsync(cancellationToken))
        {
            db.Warnings.AddRange(
                new Warning { LabId = labs[0].Id, warning = Alert.Crowded },
                new Warning { LabId = labs[Math.Min(2, labs.Count - 1)].Id, warning = Alert.Food_Or_Drink_Detected },
                new Warning { LabId = labs[Math.Min(3, labs.Count - 1)].Id, warning = Alert.Crowded });
            await db.SaveChangesAsync(cancellationToken);
        }

        if (!await db.LogRotationConfigs.AnyAsync(cancellationToken))
        {
            db.LogRotationConfigs.Add(new LogRotationConfig
            {
                IsEnabled = false,
                RetentionDays = 90,
                RunAtHourUtc = 2
            });
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
