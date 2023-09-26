namespace kotas_desafio_back_end.Services
{
    public class PokemonServiceException : Exception
    {
        public int StatusCode { get; }

        public PokemonServiceException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
