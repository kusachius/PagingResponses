using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kusachius.PagingResponses
{
    class Result<T>
    {
        public IEnumerable<T> Data;
        public Meta Meta;
    }

    public class PagedResult<T> : IActionResult
    {
        public IEnumerable<T> Data;
        public Meta Meta;
        private Result<T> result = new Result<T>();
        private IQueryable<T> query;
        private Criteria criteria;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kusachius.PagingResponses.PagedResult`1"/> class.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="criteria">Criteria.</param>
        public PagedResult(IQueryable<T> query, Criteria criteria = null)
        {
            this.query = query;
            this.criteria = criteria;
        }

        /// <summary>
        /// Executes the result async.
        /// </summary>
        /// <returns>The result async.</returns>
        /// <param name="context">Context.</param>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (criteria != null)
            {
                query = criteria.Apply(query);
            }
            result.Meta = new Meta();
            result.Meta.Pagination.CurrentPage = criteria. pagingInfo.CurrentPage;
            result.Meta.Pagination.PerPage = criteria.pagingInfo.PageSize;
            result.Data = await query.Skip(criteria.pagingInfo.RecordsToSkip()).Take(criteria.pagingInfo.PageSize).ToListAsync();
            result.Meta.Pagination.Total = await query.CountAsync();
            result.Meta.Pagination.TotalPages = (int)Math.Ceiling((float)result.Meta.Pagination.Total / criteria.pagingInfo.PageSize);

            var objectResult = new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status200OK
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
