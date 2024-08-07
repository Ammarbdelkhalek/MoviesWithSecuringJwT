namespace JwtWithIdentiyAuthenticatoin.Dto.MoviesDto
{
    public class CreateMovieDto
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public double Rate { get; set; }
        public string MovieType { get; set; }
        public IFormFile Image { get; set; }

    }
}
