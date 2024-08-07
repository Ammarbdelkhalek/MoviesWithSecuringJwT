using AutoMapper;
using JwtWithIdentiyAuthenticatoin.Dto.MoviesDto;
using JwtWithIdentiyAuthenticatoin.Models.MoviesModel;

namespace JwtWithIdentiyAuthenticatoin.Mapper
{
    public class MappingProfiling : Profile
    {
        public MappingProfiling()
        {
            CreateMap<Movies, MovieDto>();
            CreateMap<CreateMovieDto, Movies>().ForMember(x => x.Image, opt => opt.Ignore());
        }

    }
}
