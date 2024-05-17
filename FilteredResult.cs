using System.Collections.Generic;

namespace Kusachius.PagingResponses
{
	public class FilteredResult<T>
	{
		public System.Collections.Generic.IEnumerable<T> Data;
		public int PerPage;
		public int CurrentPage;
		public int? TotalPages;
		public int? TotalItems;
	}
}
