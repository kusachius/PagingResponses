using System.Collections.Generic;

namespace Kusachius.PagingResponses
{
	public class ApiResult<T>
	{
		public IEnumerable<T> Data;
		public Meta Meta = new Meta();
	}
}
