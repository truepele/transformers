using AutoMapper;
using Transformers.Model.Entities;
using Transformers.WebApi.Dto;

namespace Transformers.WebApi.Infrastructure.Mappings
{
    public class TransformerProfile : Profile
    {
        public TransformerProfile()
        {
            CreateMap<Transformer, TransformerDto>();
            CreateMap<NewTransformerDto, Transformer>()
                .ForMember(dst => dst.Id, x => x.Ignore())
                .ForMember(dst => dst.RowVersion, x => x.Ignore())
                .ForMember(dst => dst.OverallRating, x => x.Ignore());
        }
    }
}
