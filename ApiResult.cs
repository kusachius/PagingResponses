using System.Collections.Generic;

namespace Kusachius.PagingResponses
{
	public class ApiResult<T>
	{
		public System.Collections.Generic.IEnumerable<T> Data;
		public Meta Meta = new Meta();
	}
}
