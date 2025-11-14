OpenCampus.API/
├── OpenCampus.API.sln
├── README.md
├── src/
│   └── OpenCampus.API/
│       ├── OpenCampus.API.csproj
│       ├── Program.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Configuration/
│       │   ├── DependencyInjection/
│       │   │   ├── AuthenticationSetup.cs
│       │   │   ├── AuthorizationSetup.cs
│       │   │   ├── DatabaseSetup.cs
│       │   │   └── ServiceRegistration.cs
│       │   ├── Options/
│       │   │   ├── JwtOptions.cs
│       │   │   ├── PasswordHashingOptions.cs
│       │   │   └── RateLimitingOptions.cs
│       │   └── SettingsValidation/
│       │       └── ConfigurationValidationService.cs
│       ├── Auth/
│       │   ├── Jwt/
│       │   │   ├── IJwtTokenService.cs
│       │   │   ├── JwtTokenService.cs
│       │   │   └── JwtTokenFactory.cs
│       │   ├── Password/
│       │   │   ├── IPasswordHasher.cs
│       │   │   └── Pbkdf2PasswordHasher.cs
│       │   ├── Roles/
│       │   │   ├── RoleNames.cs
│       │   │   └── RoleSeedData.cs
│       │   └── Policies/
│       │       ├── AuthorizationPolicyNames.cs
│       │       └── ClaimsTransformation.cs
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/
│       │   │   ├── ChangeRequestConfiguration.cs
│       │   │   ├── CommentConfiguration.cs
│       │   │   ├── CourseConfiguration.cs
│       │   │   ├── InstitutionConfiguration.cs
│       │   │   ├── ProfessorConfiguration.cs
│       │   │   ├── ReviewConfiguration.cs
│       │   │   ├── SubjectConfiguration.cs
│       │   │   └── UserConfiguration.cs
│       │   ├── Extensions/
│       │   │   └── ModelBuilderExtensions.cs
│       │   ├── Migrations/
│       │   │   ├── <timestamp>_InitialCreate.cs
│       │   │   ├── <timestamp>_InitialCreate.Designer.cs
│       │   │   └── ApplicationDbContextModelSnapshot.cs
│       │   ├── Seed/
│       │   │   ├── DatabaseSeeder.cs
│       │   │   └── SeedData.json
│       │   └── UnitOfWork/
│       │       ├── IUnitOfWork.cs
│       │       └── UnitOfWork.cs
│       ├── Entities/
│       │   ├── Base/
│       │   │   ├── AuditableEntity.cs
│       │   │   └── IdentifiableEntity.cs
│       │   ├── ChangeRequest.cs
│       │   ├── Comment.cs
│       │   ├── Course.cs
│       │   ├── Institution.cs
│       │   ├── Professor.cs
│       │   ├── Review.cs
│       │   ├── Subject.cs
│       │   └── User.cs
│       ├── DTOs/
│       │   ├── Auth/
│       │   │   ├── LoginRequestDto.cs
│       │   │   ├── LoginResponseDto.cs
│       │   │   ├── RegisterRequestDto.cs
│       │   │   └── TokenRefreshRequestDto.cs
│       │   ├── ChangeRequests/
│       │   │   ├── ChangeRequestDetailDto.cs
│       │   │   ├── ChangeRequestListDto.cs
│       │   │   ├── ChangeRequestModerationDto.cs
│       │   │   ├── CreateChangeRequestDto.cs
│       │   │   └── UpdateChangeRequestStatusDto.cs
│       │   ├── Comments/
│       │   │   ├── CommentCreateDto.cs
│       │   │   ├── CommentListDto.cs
│       │   │   └── OfficialCommentCreateDto.cs
│       │   ├── Institutions/
│       │   │   ├── InstitutionDetailDto.cs
│       │   │   ├── InstitutionListDto.cs
│       │   │   └── InstitutionSummaryDto.cs
│       │   ├── Reviews/
│       │   │   ├── ReviewCreateDto.cs
│       │   │   ├── ReviewListDto.cs
│       │   │   ├── ReviewModerationDto.cs
│       │   │   └── ReviewSummaryDto.cs
│       │   └── Shared/
│       │       ├── PagedResultDto.cs
│       │       └── SearchFilterDto.cs
│       ├── Services/
│       │   ├── Interfaces/
│       │   │   ├── IChangeRequestService.cs
│       │   │   ├── ICommentService.cs
│       │   │   ├── IInstitutionService.cs
│       │   │   ├── IModerationService.cs
│       │   │   ├── IReviewService.cs
│       │   │   ├── ISubjectService.cs
│       │   │   └── IUserService.cs
│       │   ├── Implementations/
│       │   │   ├── ChangeRequestService.cs
│       │   │   ├── CommentService.cs
│       │   │   ├── InstitutionService.cs
│       │   │   ├── ModerationService.cs
│       │   │   ├── ReviewService.cs
│       │   │   ├── SubjectService.cs
│       │   │   └── UserService.cs
│       │   ├── Repositories/
│       │   │   ├── IAsyncRepository.cs
│       │   │   ├── ReviewRepository.cs
│       │   │   └── SpecificationEvaluator.cs
│       │   └── Validators/
│       │       ├── ReviewBusinessValidator.cs
│       │       └── CommentEligibilityValidator.cs
│       ├── Controllers/
│       │   ├── Admin/
│       │   │   ├── ChangeRequestsController.cs
│       │   │   └── ReviewsModerationController.cs
│       │   ├── AuthController.cs
│       │   ├── ChangeRequestsController.cs
│       │   ├── CommentsController.cs
│       │   ├── InstitutionsController.cs
│       │   ├── ReviewsController.cs
│       │   ├── SubjectsController.cs
│       │   └── UsersController.cs
│       ├── Common/
│       │   ├── Constants/
│       │   │   ├── ErrorCodes.cs
│       │   │   └── ReviewLimits.cs
│       │   ├── Exceptions/
│       │   │   ├── ConflictException.cs
│       │   │   ├── ForbiddenException.cs
│       │   │   └── NotFoundException.cs
│       │   ├── Extensions/
│       │   │   ├── ClaimsPrincipalExtensions.cs
│       │   │   └── ServiceCollectionExtensions.cs
│       │   └── Responses/
│       │       └── ApiResponseFactory.cs
│       ├── Filters/
│       │   ├── ValidateModelFilter.cs
│       │   └── ModeratorOnlyFilter.cs
│       ├── Mappings/
│       │   └── MappingProfile.cs
│       ├── Middleware/
│       │   ├── ErrorHandlingMiddleware.cs
│       │   └── RequestLoggingMiddleware.cs
│       └── Observability/
│           ├── Logging/
│           │   └── SerilogConfiguration.cs
│           └── Metrics/
│               └── PrometheusSetup.cs
├── tests/
│   ├── OpenCampus.API.Tests/
│   │   ├── OpenCampus.API.Tests.csproj
│   │   ├── Integration/
│   │   │   ├── AuthEndpointsTests.cs
│   │   │   ├── ModerationEndpointsTests.cs
│   │   │   └── ReviewsEndpointsTests.cs
│   │   ├── Shared/
│   │   │   ├── ApiFactory.cs
│   │   │   └── SeedDataBuilder.cs
│   │   └── Unit/
│   │       ├── ReviewServiceTests.cs
│   │       └── CommentServiceTests.cs
│   └── OpenCampus.API.UnitTests/
│       ├── OpenCampus.API.UnitTests.csproj
│       └── Services/
│           ├── ChangeRequestServiceTests.cs
│           └── InstitutionServiceTests.cs
└── docs/
    ├── architecture.md
    ├── business-rules.md
    └── api-reference.md
