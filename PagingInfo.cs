using System;
using System.Linq;

namespace Kusachius.PagingResponses
{
    public class PagingInfo<T>
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

        public IQueryable<T> TotalsQuery;
    }
}
