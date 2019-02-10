using System;
namespace Kusachius.PagingResponses
{
    public class LtStringCleaner : IStringCleaner
    {
        public string Clean(string text)
        {
            return text.ToLower(); // TODO
        }
    }
}
