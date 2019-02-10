using System;
namespace Kusachius.PagingResponses
{
    public interface ICriteriaMapper
    {
        string ToDbCriteria(string column);
    }
}
