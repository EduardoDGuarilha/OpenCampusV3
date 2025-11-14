using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenCampus.API.Auth.Roles;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Seed;

public sealed class DatabaseSeeder
{
    private const string SeedFileName = "SeedData.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly ApplicationDbContext _dbContext;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext dbContext, IHostEnvironment hostEnvironment, ILogger<DatabaseSeeder> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Applying database migrations...");
        await _dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Database migrations applied successfully.");

        await SeedRolesAsync(cancellationToken).ConfigureAwait(false);

        var seedData = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);
        await SeedInstitutionsAsync(seedData.Institutions, cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var desiredRoles = RoleSeedData.Roles;
        if (desiredRoles.Count == 0)
        {
            return;
        }

        var existingRoles = await _dbContext.Roles
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var rolesByName = existingRoles.ToDictionary(role => role.Name, StringComparer.OrdinalIgnoreCase);
        var hasChanges = false;

        foreach (var seed in desiredRoles)
        {
            if (rolesByName.TryGetValue(seed.Name, out var role))
            {
                hasChanges |= UpdateRoleIfNecessary(role, seed);
            }
            else
            {
                var newRole = new SystemRole
                {
                    Name = seed.Name,
                    NormalizedName = seed.NormalizedName,
                    DisplayName = seed.DisplayName,
                    Description = NormalizeOptional(seed.Description)
                };

                _dbContext.Roles.Add(newRole);
                rolesByName[newRole.Name] = newRole;
                hasChanges = true;
                _logger.LogInformation("Seeded role {Role}.", newRole.Name);
            }
        }

        if (hasChanges)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private bool UpdateRoleIfNecessary(SystemRole role, RoleSeed seed)
    {
        var changed = false;

        if (!string.Equals(role.NormalizedName, seed.NormalizedName, StringComparison.Ordinal))
        {
            role.NormalizedName = seed.NormalizedName;
            changed = true;
        }

        if (!string.Equals(role.DisplayName, seed.DisplayName, StringComparison.Ordinal))
        {
            role.DisplayName = seed.DisplayName;
            changed = true;
        }

        var description = NormalizeOptional(seed.Description);
        if (!string.Equals(role.Description, description, StringComparison.Ordinal))
        {
            role.Description = description;
            changed = true;
        }

        if (changed)
        {
            _logger.LogInformation("Updated role {Role} to match seed definition.", role.Name);
        }

        return changed;
    }

    private async Task SeedInstitutionsAsync(IReadOnlyCollection<InstitutionSeed> institutions, CancellationToken cancellationToken)
    {
        if (institutions.Count == 0)
        {
            _logger.LogInformation("No institution seed data found.");
            return;
        }

        var hasChanges = false;

        var existingInstitutions = await _dbContext.Institutions
            .Include(i => i.Courses)
                .ThenInclude(c => c.Subjects)
            .Include(i => i.Professors)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var institutionsByName = existingInstitutions.ToDictionary(i => i.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var institutionSeed in institutions)
        {
            var normalizedName = NormalizeName(institutionSeed.Name);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                continue;
            }

            if (!institutionsByName.TryGetValue(normalizedName, out var institution))
            {
                institution = new Institution
                {
                    Name = normalizedName,
                    Acronym = NormalizeOptional(institutionSeed.Acronym),
                    Description = NormalizeOptional(institutionSeed.Description),
                    WebsiteUrl = NormalizeOptional(institutionSeed.WebsiteUrl),
                    Courses = new List<Course>(),
                    Professors = new List<Professor>()
                };

                SeedCourses(institution, institutionSeed.Courses);
                SeedProfessors(institution, institutionSeed.Professors);

                _dbContext.Institutions.Add(institution);
                institutionsByName[normalizedName] = institution;
                hasChanges = true;
                _logger.LogInformation("Seeded institution {InstitutionName} with default structures.", institution.Name);
            }
            else
            {
                hasChanges |= UpdateInstitution(institution, institutionSeed);
                hasChanges |= SeedCourses(institution, institutionSeed.Courses);
                hasChanges |= SeedProfessors(institution, institutionSeed.Professors);
            }
        }

        if (hasChanges)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static bool UpdateInstitution(Institution institution, InstitutionSeed seed)
    {
        var changed = false;

        var acronym = NormalizeOptional(seed.Acronym);
        if (!string.Equals(institution.Acronym, acronym, StringComparison.Ordinal))
        {
            institution.Acronym = acronym;
            changed = true;
        }

        var description = NormalizeOptional(seed.Description);
        if (!string.Equals(institution.Description, description, StringComparison.Ordinal))
        {
            institution.Description = description;
            changed = true;
        }

        var website = NormalizeOptional(seed.WebsiteUrl);
        if (!string.Equals(institution.WebsiteUrl, website, StringComparison.Ordinal))
        {
            institution.WebsiteUrl = website;
            changed = true;
        }

        return changed;
    }

    private static bool SeedCourses(Institution institution, IReadOnlyCollection<CourseSeed>? courses)
    {
        if (courses is null || courses.Count == 0)
        {
            return false;
        }

        var changed = false;
        var coursesByName = institution.Courses.ToDictionary(course => course.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var courseSeed in courses)
        {
            var normalizedName = NormalizeName(courseSeed.Name);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                continue;
            }

            if (coursesByName.TryGetValue(normalizedName, out var course))
            {
                changed |= UpdateCourse(course, courseSeed);
                changed |= SeedSubjects(course, courseSeed.Subjects);
            }
            else
            {
                var newCourse = new Course
                {
                    Name = normalizedName,
                    Description = NormalizeOptional(courseSeed.Description)
                };

                SeedSubjects(newCourse, courseSeed.Subjects);

                institution.Courses.Add(newCourse);
                coursesByName[normalizedName] = newCourse;
                changed = true;
            }
        }

        return changed;
    }

    private static bool UpdateCourse(Course course, CourseSeed seed)
    {
        var description = NormalizeOptional(seed.Description);
        if (string.Equals(course.Description, description, StringComparison.Ordinal))
        {
            return false;
        }

        course.Description = description;
        return true;
    }

    private static bool SeedSubjects(Course course, IReadOnlyCollection<SubjectSeed>? subjects)
    {
        if (subjects is null || subjects.Count == 0)
        {
            return false;
        }

        var changed = false;
        var subjectsByName = course.Subjects.ToDictionary(subject => subject.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var subjectSeed in subjects)
        {
            var normalizedName = NormalizeName(subjectSeed.Name);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                continue;
            }

            if (subjectsByName.TryGetValue(normalizedName, out var subject))
            {
                var description = NormalizeOptional(subjectSeed.Description);
                if (!string.Equals(subject.Description, description, StringComparison.Ordinal))
                {
                    subject.Description = description;
                    changed = true;
                }
            }
            else
            {
                var newSubject = new Subject
                {
                    Name = normalizedName,
                    Description = NormalizeOptional(subjectSeed.Description)
                };

                course.Subjects.Add(newSubject);
                subjectsByName[normalizedName] = newSubject;
                changed = true;
            }
        }

        return changed;
    }

    private static bool SeedProfessors(Institution institution, IReadOnlyCollection<ProfessorSeed>? professors)
    {
        if (professors is null || professors.Count == 0)
        {
            return false;
        }

        var changed = false;
        var professorsByName = institution.Professors.ToDictionary(professor => professor.FullName, StringComparer.OrdinalIgnoreCase);

        foreach (var professorSeed in professors)
        {
            var normalizedName = NormalizeName(professorSeed.FullName);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                continue;
            }

            if (professorsByName.TryGetValue(normalizedName, out var professor))
            {
                var bio = NormalizeOptional(professorSeed.Bio);
                if (!string.Equals(professor.Bio, bio, StringComparison.Ordinal))
                {
                    professor.Bio = bio;
                    changed = true;
                }
            }
            else
            {
                var newProfessor = new Professor
                {
                    FullName = normalizedName,
                    Bio = NormalizeOptional(professorSeed.Bio)
                };

                institution.Professors.Add(newProfessor);
                professorsByName[normalizedName] = newProfessor;
                changed = true;
            }
        }

        return changed;
    }

    private async Task<SeedDataDocument> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var seedDirectory = Path.Combine(_hostEnvironment.ContentRootPath, "Data", "Seed");
        var seedFilePath = Path.Combine(seedDirectory, SeedFileName);

        if (!File.Exists(seedFilePath))
        {
            _logger.LogWarning("Seed data file not found at path {SeedFilePath}. Skipping structure seeding.", seedFilePath);
            return SeedDataDocument.Empty;
        }

        await using var stream = File.OpenRead(seedFilePath);
        var document = await JsonSerializer.DeserializeAsync<SeedDataDocument>(stream, SerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        return document ?? SeedDataDocument.Empty;
    }

    private static string NormalizeName(string value)
        => value?.Trim() ?? string.Empty;

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    private sealed class SeedDataDocument
    {
        public static SeedDataDocument Empty { get; } = new();

        public IReadOnlyCollection<InstitutionSeed> Institutions { get; init; } = Array.Empty<InstitutionSeed>();
    }

    private sealed class InstitutionSeed
    {
        public string Name { get; init; } = string.Empty;

        public string? Acronym { get; init; }

        public string? Description { get; init; }

        public string? WebsiteUrl { get; init; }

        public IReadOnlyCollection<CourseSeed> Courses { get; init; } = Array.Empty<CourseSeed>();

        public IReadOnlyCollection<ProfessorSeed> Professors { get; init; } = Array.Empty<ProfessorSeed>();
    }

    private sealed class CourseSeed
    {
        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public IReadOnlyCollection<SubjectSeed> Subjects { get; init; } = Array.Empty<SubjectSeed>();
    }

    private sealed class SubjectSeed
    {
        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }
    }

    private sealed class ProfessorSeed
    {
        public string FullName { get; init; } = string.Empty;

        public string? Bio { get; init; }
    }
}
