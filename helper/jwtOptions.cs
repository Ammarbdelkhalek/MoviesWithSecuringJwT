namespace JwtWithIdentiyAuthenticatoin.helper
{
    public class jwtOptions
    {
        public string issuer { get; set; }
        public string Audienc { get; set; }
        public double LifeTime { get; set; }
        public string SigninKey { get; set; }
    }
}


