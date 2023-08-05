using AutoMapper;
using EnrolleeCompetitionRunner.Core.Dtos;
using EnrolleeCompetitionRunner.Domain.Constants;
using EnrolleeCompetitionRunner.Domain.Entities;

namespace EnrolleeCompetitionRunner.Core.Mappings;
public sealed class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        // Entity -> Dto
        CreateMap<Offer, OfferDto>()
            .ForMember(d => d.SpecialityCode, opt => opt.MapFrom(src => src.Speciality.Code))
            .ForMember(d => d.SpecialityName, opt => opt.MapFrom(src => src.Speciality.Name))
            .ForMember(d => d.SpecializationCode, opt => opt.MapFrom(src => src.Speciality.SpecializationCode))
            .ForMember(d => d.SpecializationName, opt => opt.MapFrom(src => src.Speciality.SpecializationName))
            .ForMember(d => d.UniversityName, opt => opt.MapFrom(src => src.University.Name))
            .ForMember(d => d.PassedEnrollees, opt => opt.MapFrom(src => src.Enrollees))
            .ForMember(d => d.TotalPassedEnrollees, opt => opt.Ignore())
            .ForMember(d => d.Quote1PassingScore, opt => opt.Ignore())
            .ForMember(d => d.Quote2PassingScore, opt => opt.Ignore())
            .ForMember(d => d.GeneralPassingScore, opt => opt.Ignore())
            .ForMember(d => d.MinScholarshipScore, opt => opt.Ignore());
        CreateMap<Offer, OfferHeaderDto>()
            .ForMember(d => d.SpecialityCode, opt => opt.MapFrom(src => src.Speciality.Code))
            .ForMember(d => d.SpecialityName, opt => opt.MapFrom(src => src.Speciality.Name))
            .ForMember(d => d.SpecializationCode, opt => opt.MapFrom(src => src.Speciality.SpecializationCode))
            .ForMember(d => d.SpecializationName, opt => opt.MapFrom(src => src.Speciality.SpecializationName))
            .ForMember(d => d.UniversityName, opt => opt.MapFrom(src => src.University.Name));

        CreateMap<OfferEnrollee, OfferEnrolleeDto>()
            .ForMember(d => d.EnrolleeName, opt => opt.MapFrom(src => src.Enrollee.Name))
            .ForMember(d => d.Passed, opt => opt.MapFrom(src => src.FinalStatus == OfferEnrolleeConstants.Statuses.RecommendedBudget));

        CreateMap<Enrollee, EnrolleeDto>();
    }
}
