using System;
namespace Kusachius.PagingResponses
{
    public class PagingInfo
    {
        public int PageSize = 20;
        public int CurrentPage = 1;

        public int RecordsToSkip()
        {
            return (CurrentPage - 1) * PageSize;
        }

        public PagingInfo()
        {
        }
    }
}
