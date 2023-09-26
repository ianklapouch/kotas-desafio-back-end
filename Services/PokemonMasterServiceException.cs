namespace kotas_desafio_back_end.Services
{
    public class PokemonMasterServiceException : Exception
    {
        public int StatusCode { get; }

        public PokemonMasterServiceException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
