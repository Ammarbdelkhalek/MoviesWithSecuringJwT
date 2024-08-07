 
using JwtWithIdentiyAuthenticatoin.Dto.MoviesDto;
using JwtWithIdentiyAuthenticatoin.Models.MoviesModel;

namespace JwtWithIdentiyAuthenticatoin.services
{
    public interface IMovieServices
    {
        Task<IEnumerable<Movies>> GetALLAsync();
        Task<Movies>GetByIdAsync( int id );
        Task<Movies> CreateAsync(Movies movie);
        Task<Movies> update( int id, CreateMovieDto movieDto);
        Task<Movies> DeleteAsync(int id);
    }
}
