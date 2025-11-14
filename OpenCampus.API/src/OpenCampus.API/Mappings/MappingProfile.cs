using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OpenCampus.API.DTOs.ChangeRequests;
using OpenCampus.API.DTOs.Comments;
using OpenCampus.API.DTOs.Courses;
using OpenCampus.API.DTOs.Institutions;
using OpenCampus.API.DTOs.Professors;
using OpenCampus.API.DTOs.Reviews;
using OpenCampus.API.DTOs.Subjects;
using OpenCampus.API.DTOs.Users;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateChangeRequestMappings();
        CreateCommentMappings();
        CreateCourseMappings();
        CreateInstitutionMappings();
        CreateProfessorMappings();
        CreateReviewMappings();
        CreateSubjectMappings();
        CreateUserMappings();
    }

    private void CreateCommentMappings()
    {
        CreateMap<Comment, CommentListDto>();

        CreateMap<CommentCreateDto, Comment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.IsOfficial, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.Review, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<OfficialCommentCreateDto, Comment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.IsOfficial, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Review, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }

    private void CreateReviewMappings()
    {
        CreateMap<Review, ReviewListDto>()
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Approved ? src.Comments : Enumerable.Empty<Comment>()));

        CreateMap<Review, ReviewModerationDto>()
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

        CreateMap<ReviewCreateDto, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Approved, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.Institution, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Professor, opt => opt.Ignore())
            .ForMember(dest => dest.Subject, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }

    private void CreateChangeRequestMappings()
    {
        CreateMap<ChangeRequest, ChangeRequestListDto>();

        CreateMap<ChangeRequest, ChangeRequestDetailDto>();

        CreateMap<ChangeRequest, ChangeRequestModerationDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : null))
            .ForMember(dest => dest.ResolvedByName, opt => opt.MapFrom(src => src.ResolvedBy != null ? src.ResolvedBy.FullName : null));

        CreateMap<CreateChangeRequestDto, ChangeRequest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ChangeRequestStatus.Pending))
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedById, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ResolvedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ResolutionNotes, opt => opt.Ignore())
            .ForMember(dest => dest.Institution, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Professor, opt => opt.Ignore())
            .ForMember(dest => dest.Subject, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }

    private void CreateInstitutionMappings()
    {
        CreateMap<Institution, InstitutionSummaryDto>();

        CreateMap<Institution, InstitutionListDto>()
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)));

        CreateMap<Institution, InstitutionDetailDto>()
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)))
            .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses))
            .ForMember(dest => dest.Professors, opt => opt.MapFrom(src => src.Professors));
    }

    private void CreateCourseMappings()
    {
        CreateMap<Course, CourseSummaryDto>()
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Institution != null ? src.Institution.Name : string.Empty));

        CreateMap<Course, CourseListDto>()
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Institution != null ? src.Institution.Name : string.Empty))
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)));

        CreateMap<Course, CourseDetailDto>()
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Institution != null ? src.Institution.Name : string.Empty))
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)))
            .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.Subjects));
    }

    private void CreateProfessorMappings()
    {
        CreateMap<Professor, ProfessorSummaryDto>()
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Institution != null ? src.Institution.Name : string.Empty));

        CreateMap<Professor, ProfessorListDto>()
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Institution != null ? src.Institution.Name : string.Empty))
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)));

        CreateMap<Professor, ProfessorDetailDto>()
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Institution != null ? src.Institution.Name : string.Empty))
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)));
    }

    private void CreateSubjectMappings()
    {
        CreateMap<Subject, SubjectSummaryDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : string.Empty));

        CreateMap<Subject, SubjectListDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : string.Empty))
            .ForMember(dest => dest.InstitutionId, opt => opt.MapFrom(src => src.Course != null ? src.Course.InstitutionId : Guid.Empty))
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Course != null && src.Course.Institution != null ? src.Course.Institution.Name : string.Empty))
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)));

        CreateMap<Subject, SubjectDetailDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : string.Empty))
            .ForMember(dest => dest.InstitutionId, opt => opt.MapFrom(src => src.Course != null ? src.Course.InstitutionId : Guid.Empty))
            .ForMember(dest => dest.InstitutionName, opt => opt.MapFrom(src => src.Course != null && src.Course.Institution != null ? src.Course.Institution.Name : string.Empty))
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => CalculateAverageScore(src.Reviews)))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => CountApprovedReviews(src.Reviews)));
    }

    private static double CalculateAverageScore(IEnumerable<Review> reviews)
    {
        var approved = ApprovedReviews(reviews).ToList();
        if (approved.Count == 0)
        {
            return 0d;
        }

        return approved.Average(r => CalculateOverallScore(r));
    }

    private static int CountApprovedReviews(IEnumerable<Review> reviews)
    {
        return ApprovedReviews(reviews).Count();
    }

    private static IEnumerable<Review> ApprovedReviews(IEnumerable<Review> reviews)
    {
        return reviews?.Where(r => r.Approved) ?? Enumerable.Empty<Review>();
    }

    private static double CalculateOverallScore(Review review)
    {
        var total = review.ScoreClarity + review.ScoreRelevance + review.ScoreSupport + review.ScoreInfrastructure;
        return total / 4d;
    }

    private void CreateUserMappings()
    {
        CreateMap<User, UserDetailDto>();
    }
}
