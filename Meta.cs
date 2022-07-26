using System.Collections.Generic;

namespace Kusachius.PagingResponses
{
	public class Paigination
	{
		public int? Total;
		public int? Count { get { return Total; } }
		public int PerPage;
		public int CurrentPage;
		public int? TotalPages;
		public object Totals;
		public Dictionary<string, string> Links;
	}

	public class Meta
	{
		public Paigination Pagination = new Paigination();
	}
}
