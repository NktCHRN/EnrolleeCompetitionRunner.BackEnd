using AutoMapper;
using EnrolleeCompetitionRunner.Contracts.Models;
using EnrolleeCompetitionRunner.Contracts.Requests;
using EnrolleeCompetitionRunner.Contracts.Responses;
using EnrolleeCompetitionRunner.Core.Dtos;
using EnrolleeCompetitionRunner.Core.Queries;

namespace EnrolleeCompetitionRunner.Api.Mappings;

public sealed class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // Dto -> Response|ResponseModel
        CreateMap<OfferDto, PassedEnrolleesByOfferCodeResponse>();
        CreateMap<OfferHeaderDto, OfferHeaderModel>();

        CreateMap<OfferEnrolleeDto, OfferEnrolleeModel>();

        CreateMap<EnrolleesSearchResultsDto, EnrolleesSearchResponse>();
        CreateMap<EnrolleeDto, EnrolleeModel>();
        CreateMap<EnrolleeDto, EnrolleeResponse>();

        CreateMap<OfferEnrolleeDto, EnrolleeOfferWithOfferInfoModel>();

        // Request -> Query
        CreateMap<SearchEnrolleesRequest, SearchEnrolleesQuery>();
    }
}
