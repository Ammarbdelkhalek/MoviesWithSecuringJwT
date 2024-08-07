using AutoMapper;
using JwtWithIdentiyAuthenticatoin.Dto;
using JwtWithIdentiyAuthenticatoin.Dto.MoviesDto;
using JwtWithIdentiyAuthenticatoin.helper;
using JwtWithIdentiyAuthenticatoin.Models.MoviesModel;
using JwtWithIdentiyAuthenticatoin.services;
using Microsoft.AspNetCore.Authorization;
 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtWithIdentiyAuthenticatoin.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private IMovieServices movieServices;
        private IMapper mapper;
        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public MoviesController(IMovieServices movieServices, IMapper mapper)
        {
            this.movieServices = movieServices;
            this.mapper = mapper;
           
        }

        [HttpGet("get all")]
        public async Task<IActionResult> GetALLAsync()
        {
            var result  = await movieServices.GetALLAsync();
            var movieDto = mapper.Map< IEnumerable<MovieDto>>(result);
            return Ok(result);
        }
        [HttpGet(" getbyid {id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id) 
        {
            var result = await movieServices.GetByIdAsync(id);
            var movieDto = mapper.Map<MovieDto>(result);
            return Ok(movieDto);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateMovieDto movieDto)
        {

            if (movieDto.Image != null)
            {
                if (!_allowedExtenstions.Contains(Path.GetExtension(movieDto.Image.FileName).ToLower()))
                    return BadRequest(" Only .png and .jpg images are allowed! ");

                if (movieDto.Image.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB!");
            }

            using var DataStream = new MemoryStream();

            await movieDto.Image.CopyToAsync(DataStream);

            var movie = mapper.Map<Movies>(movieDto);
            movie.Image = DataStream.ToArray();
            var result = await movieServices.CreateAsync(movie);
           
            return Ok(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] CreateMovieDto movieDto)
        {
            var result = await movieServices.update(id, movieDto);
            var dto = mapper.Map<MovieDto>(result);
            return Ok(dto);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult>DeleteAsync(int id)
        {
            await movieServices.DeleteAsync(id);
            return Ok("deleted sucessfully"); 
        }
    }
}
