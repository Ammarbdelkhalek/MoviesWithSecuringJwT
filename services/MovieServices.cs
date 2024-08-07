using AutoMapper;
using JwtWithIdentiyAuthenticatoin.Data;
using JwtWithIdentiyAuthenticatoin.Dto.MoviesDto;
using JwtWithIdentiyAuthenticatoin.Models.MoviesModel;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace JwtWithIdentiyAuthenticatoin.services
{
    public class MovieServices(AppDbContext context) : IMovieServices
    {
        public async  Task<IEnumerable<Movies>> GetALLAsync()
        {
             var movies = await context.Set<Movies>().ToListAsync();
            return movies;
        }

        public async  Task<Movies> GetByIdAsync(int id)
        {
             var movie = await context.Set<Movies>().FindAsync(id);
            if(movie == null)
            {
                return null;
            }
            return movie;
        }

        public  async Task<Movies> CreateAsync(Movies movie)
        {
            context.Set<Movies>().Add(movie);
            await context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movies> update(int id, CreateMovieDto movieDto)
        {
            using var DataStream = new MemoryStream();
            await movieDto.Image.CopyToAsync(DataStream);

            var movie = await GetByIdAsync(id);
            if(movie == null)
            {
                return null;
            }
            movie.Title = movieDto.Title;
            movie.Description = movieDto.Description;  
            movie.Rate = movieDto.Rate;
            movie.MovieType = movieDto.MovieType;
            movie.Image =DataStream.ToArray() ;

            await context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movies> DeleteAsync(int id)
        {
            var movie = await GetByIdAsync(id);
            if(movie == null)
            {
                return null;
            }
            context.Remove(movie);
            await context.SaveChangesAsync();
            return movie;
        }

        public Task<Movies> update(int id, MovieDto movieDto)
        {
            throw new NotImplementedException();
        }
    }
}
