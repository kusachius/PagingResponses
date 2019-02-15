using System;
using System.Collections.Generic;
using System.Linq;

namespace Kusachius.PagingResponses
{
    public interface ICriteria
    {
        /// <summary>
        /// Apply the specified query.
        /// </summary>
        /// <returns>The apply.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        IQueryable<T> Apply<T>(IQueryable<T> query);
        /// <summary>
        /// Applies the where.
        /// </summary>
        /// <returns>The where.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>

        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void AddCondition(Condition condition);

        /// <summary>
        /// Sets the size of the page.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        void SetPageSize(int pageSize);

        /// <summary>
        /// Adds the sort.
        /// </summary>
        /// <param name="sort">Sort.</param>
        void AddSort(Sorting sort);
    }
}
