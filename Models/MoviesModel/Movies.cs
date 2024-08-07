namespace JwtWithIdentiyAuthenticatoin.Models.MoviesModel
{
    public class Movies
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Rate { get; set; }
        public string MovieType { get; set; }
        public byte[] Image { get; set; }

    }
}
