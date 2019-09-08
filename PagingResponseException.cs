using System;
namespace Kusachius.PagingResponses
{
    public class PagingResponseException: Exception
    {
        public PagingResponseException(string message) : base(message)
        {
        }
    }
}
