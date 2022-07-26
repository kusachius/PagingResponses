namespace Kusachius.PagingResponses
{
	public interface ICriteriaMapper
	{
		string ToDbWhereColumn(string column);
		string ToDbSortColumn(string column);
	}
}
