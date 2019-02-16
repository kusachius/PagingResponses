using System;
namespace Kusachius.PagingResponses
{
    public interface ICriteriaMapperFactory
    {
        ICriteriaMapper GetMapper(Type classType);
    }
}
