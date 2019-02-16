using System;
using System.Collections.Generic;

namespace Kusachius.PagingResponses
{
    public class FilteredResult<T>
    {
        public IEnumerable<T> Data;
        public int PerPage;
        public int CurrentPage;
        public int? TotalPages;
        public int? TotalItems;
    }
}
