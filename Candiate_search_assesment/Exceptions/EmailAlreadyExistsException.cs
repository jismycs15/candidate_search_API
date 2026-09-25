namespace Candiate_search_assesment.Exceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public string Email { get; }

        public EmailAlreadyExistsException(string email, Exception? innerException = null)
            : base($"An account with email '{email}' already exists.", innerException)
        {
            Email = email;
        }
    }
}
