using ContactNumberWebAPI.Data;
using ContactNumberWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactNumberWebAPI.Seeders;

public class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await dbContext.Database.MigrateAsync(cancellationToken);
        
        await SeedContactCategoryAsync(dbContext, cancellationToken);

        await SeedContactAsync(dbContext, cancellationToken);
        
        await SeedAdminAsync(
            dbContext,
            configuration,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedContactCategoryAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        string[] categoryNames =
        [
            "FAMILY",
            "FRIEND",
            "OFFICE",
            "CLIENT"
        ];

        List<string> existingCategoryNames = await dbContext.ContactCategories.Select(category => category.Name)
            .ToListAsync(cancellationToken);

        HashSet<string> existingCategoryNameSet = existingCategoryNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string categoryName in categoryNames)
        {
            if (existingCategoryNameSet.Contains(categoryName))
            {
                continue;
            }

            ContactCategory category = new ContactCategory
            {
                Id = Guid.NewGuid(),
                Name = categoryName
            };

            await dbContext.ContactCategories.AddAsync(
                category,
                cancellationToken);
        }
    }
    
    private static async Task SeedAdminAsync(
        AppDbContext dbContext,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        string? adminEmail = configuration["SeedAdmin:Email"];
        string? adminPassword = configuration["SeedAdmin:Password"];
        string adminFullName = configuration["SeedAdmin:FullName"] ?? "Administrator";

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        string normalizedEmail = adminEmail.Trim().ToLowerInvariant();

        bool adminExists = await dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (adminExists)
        {
            return;
        }

        User admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = adminFullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = "Admin"
        };

        await dbContext.Users.AddAsync(admin, cancellationToken);
    }
    
    public static async Task SeedContactAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        ContactSeedData[] contactSeedData =
        [
            new(
                Name: "Budi Santoso",
                PhoneNumber: "081234567801",
                Email: "budi@mail.com",
                Address: "Jakarta",
                CategoryName: "Family"),

            new(
                Name: "Andi Wijaya",
                PhoneNumber: "081234567802",
                Email: "andi@mail.com",
                Address: "Bandung",
                CategoryName: "Friend"),

            new(
                Name: "Siti Rahma",
                PhoneNumber: "081234567803",
                Email: "siti@mail.com",
                Address: "Surabaya",
                CategoryName: "Office"),

            new(
                Name: "Rina Amelia",
                PhoneNumber: "081234567804",
                Email: "rina@mail.com",
                Address: "Malang",
                CategoryName: "Client"),
            
            new(
                Name: "Abams",
                PhoneNumber: "081234561111",
                Email: "abams@mail.com",
                Address: "Semarang",
                CategoryName: "Office"),
            
            new(
                Name: "Rayhan",
                PhoneNumber: "081234562222",
                Email: "rayhan@mail.com",
                Address: "Bandung",
                CategoryName: "Office"),
            
            new(
                Name: "Malvin",
                PhoneNumber: "081234563333",
                Email: "malvin@mail.com",
                Address: "Semarang",
                CategoryName: "Friend"),
            
            new(
                Name: "Putra",
                PhoneNumber: "081234568888",
                Email: "putra@mail.com",
                Address: "Salatiga",
                CategoryName: "Friend"),
            
            new(
                Name: "Alle",
                PhoneNumber: "081234564444",
                Email: "alle@mail.com",
                Address: "Salatiga",
                CategoryName: "Family"),
            
            new(
                Name: "Jeje",
                PhoneNumber: "081234565555",
                Email: "jeje@mail.com",
                Address: "Sidoarjo",
                CategoryName: "Family"),
            
            new(
                Name: "Dinda",
                PhoneNumber: "081234566666",
                Email: "dinda@mail.com",
                Address: "Salatiga",
                CategoryName: "Client"),
            
            new(
                Name: "Muhammad Iqbal",
                PhoneNumber: "081234567777",
                Email: "ibal@mail.com",
                Address: "Pekanbaru",
                CategoryName: "Client")
        ];

        Dictionary<string, Guid> categories =
            await dbContext.ContactCategories
                .AsNoTracking()
                .ToDictionaryAsync(
                    category => category.Name.ToLower(),
                    category => category.Id,
                    cancellationToken);

        List<string> existingPhoneNumbers = await dbContext.Contacts
                .AsNoTracking()
                .Select(contact => contact.PhoneNumber)
                .ToListAsync(cancellationToken);

        HashSet<string> existingPhoneNumberSet = existingPhoneNumbers.ToHashSet();

        List<Contact> contactsToAdd = [];

        foreach (ContactSeedData seedData in contactSeedData)
        {
            if (existingPhoneNumberSet.Contains(seedData.PhoneNumber))
            {
                continue;
            }

            string categoryKey = seedData.CategoryName.ToLower();

            if (!categories.TryGetValue(categoryKey, out Guid contactCategoryId))
            {
                throw new InvalidOperationException(
                    $"Kategori '{seedData.CategoryName}' " +
                    "belum tersedia. Jalankan category seeder terlebih dahulu."
                    );
            }

            Contact contact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = seedData.Name,
                PhoneNumber = seedData.PhoneNumber,
                Email = seedData.Email,
                Address = seedData.Address,
                ContactCategoryId = contactCategoryId
            };

            contactsToAdd.Add(contact);
            existingPhoneNumberSet.Add(seedData.PhoneNumber);
        }

        if (contactsToAdd.Count == 0)
        {
            return;
        }

        await dbContext.Contacts.AddRangeAsync(contactsToAdd, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    private sealed record ContactSeedData(
        string Name,
        string PhoneNumber,
        string? Email,
        string? Address,
        string CategoryName);
}