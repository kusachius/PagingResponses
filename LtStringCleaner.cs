using System;
namespace Kusachius.PagingResponses
{
    public class LtStringCleaner : IStringCleaner
    {
        public string Clean(string text)
        {
            text = text.Replace('ą', 'a');
            text = text.Replace('č', 'c');
            text = text.Replace('ę', 'e');
            text = text.Replace('ė', 'e');
            text = text.Replace('į', 'i');
            text = text.Replace('š', 's');
            text = text.Replace('ų', 'u');
            text = text.Replace('ū', 'u');
            text = text.Replace('ž', 'z');

            text = text.Replace('Ą', 'a');
            text = text.Replace('Č', 'c');
            text = text.Replace('Ę', 'e');
            text = text.Replace('Ė', 'e');
            text = text.Replace('Į', 'i');
            text = text.Replace('Š', 's');
            text = text.Replace('Ų', 'u');
            text = text.Replace('Ū', 'u');
            text = text.Replace('Ž', 'z');
            return text.ToLower(); // TODO
        }
    }
}
