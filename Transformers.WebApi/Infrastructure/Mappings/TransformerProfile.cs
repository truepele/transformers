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
        }
    }
}
