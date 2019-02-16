using System;
namespace Kusachius.PagingResponses
{
    public class Paigination
    {
        public int? Total;
        public int? Count { get { return Total; } }
        public int PerPage;
        public int CurrentPage;
        public int? TotalPages;
        public Tuple<string, string>[] Links;
    }

    public class Meta
    {
        public Paigination Pagination = new Paigination();
    }
}
