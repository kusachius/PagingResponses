using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Kusachius.PagingResponses
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; protected set; }
        public int PerPage { get; protected set; }
        public int CurrentPage { get; protected set; }
        public int? TotalPages { get; protected set; } = null;
        public int? TotalItems { get; protected set; } = null;

        /// <summary>
        /// Creates new result
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="criteria">Criteria.</param>
        public static async Task<PagedResult<T>> CreateResultAsync(IQueryable<T> query, ICriteria<T> criteria = null, bool includeTotals = true)
        {

            PagedResult<T> result = new PagedResult<T>();

            if (criteria != null)
            {
                query = criteria.Apply<T>(query);
            }

            result.CurrentPage = criteria.PagingInfo.CurrentPage;
            result.PerPage = criteria.PagingInfo.PageSize;
            if (includeTotals)
            {
                result.TotalItems = await criteria.PagingInfo.TotalsQuery.CountAsync();
                result.TotalPages = (int)Math.Ceiling((float)result.TotalItems / criteria.PagingInfo.PageSize);
            }

            result.Data = await query.ToListAsync();

            return result;
        }

        /// <summary>
        /// Creates the result.
        /// </summary>
        /// <returns>The result.</returns>
        /// <param name="query">Query.</param>
        /// <param name="criteria">Criteria.</param>
        /// <param name="includeTotals">If set to <c>true</c> include totals.</param>
        public static PagedResult<T> CreateResult(IQueryable<T> query, ICriteria<T> criteria = null, bool includeTotals = true)
        {

            PagedResult<T> result = new PagedResult<T>();

            if (criteria != null)
            {
                query = criteria.Apply<T>(query);
            }

            result.CurrentPage = criteria.PagingInfo.CurrentPage;
            result.PerPage = criteria.PagingInfo.PageSize;
            if (includeTotals)
            {
                result.TotalItems = criteria.PagingInfo.TotalsQuery.Count();
                result.TotalPages = (int)Math.Ceiling((float)result.TotalItems / criteria.PagingInfo.PageSize);
            }

            result.Data = query.ToList();

            return result;
        }


        /// <summary>
        /// Converts to mapped type
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="mapper">Mapper.</param>
        public PagedResult<K> ConvertTo<K>(IMapper mapper)
        {
            return new PagedResult<K>
            {
                PerPage = PerPage,
                CurrentPage = CurrentPage,
                TotalPages = TotalPages,
                TotalItems = TotalItems,

                Data = (IEnumerable<K>)mapper.Map<IEnumerable<K>>(Data)
            };
        }

    }
}
