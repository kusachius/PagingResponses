using System;
using System.Collections.Generic;
using System.Linq;

namespace Kusachius.PagingResponses
{
    public class Criteria
    {
        List<Condition> conditions = new List<Condition>();
        List<Sorting> sorts = new List<Sorting>();
        ICriteriaMapper mapper;
        IStringCleaner cleaner;
        public PagingInfo pagingInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kusachius.PagingResponses.Criteria"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="cleaner">Cleaner.</param>
        public Criteria(ICriteriaMapper mapper, IStringCleaner cleaner)
        {
            this.mapper = mapper;
            this.cleaner = cleaner;
            pagingInfo = new PagingInfo
            {
                PageSize = 20
            };
        }

        /// <summary>
        /// Apply the specified query.
        /// </summary>
        /// <returns>The apply.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public IQueryable<T> Apply<T>(IQueryable<T> query)
        {
            query = ApplySort(query);
            query = ApplyWhere(query);

            // return query.paginate(this.getPageSize());
            return query;
        }

        /// <summary>
        /// Applies the sort.
        /// </summary>
        /// <returns>The sort.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private IQueryable<T> ApplySort<T>(IQueryable<T> query)
        {
            foreach (var sort in sorts)
            {
                string column = mapper.ToDbCriteria(sort.Column);
                var propertyInfo = query.First().GetType().GetProperty(column);
                query = query.OrderBy(e => propertyInfo.GetValue(e, null));
            }

            return query;
        }

        /// <summary>
        /// Applies the where.
        /// </summary>
        /// <returns>The where.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private IQueryable<T> ApplyWhere<T>(IQueryable<T> query)
        {

            foreach (var cond in conditions)
            {
                string column = mapper.ToDbCriteria(cond.Column);
                var first = query.First();
                var propertyInfo = first.GetType().GetProperty(column);
                string val = cleaner.Clean(cond.Value);
                switch(cond.Operation)
                {
                    case "=":
                        query = query.Where(e => (string)propertyInfo.GetValue(e, null) == val);
                        break;
                    case "like":
                        query = query.Where(e => ((string)propertyInfo.GetValue(e, null)).Contains(val));
                        break;
                }
            }

            return query;
        }

        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void AddCondition(Condition condition)
        {
            conditions.Add(condition);
        }

        /// <summary>
        /// Sets the size of the page.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public void SetPageSize(int pageSize)
        {
            this.pagingInfo.PageSize = pageSize;
        }

        /// <summary>
        /// Adds the sort.
        /// </summary>
        /// <param name="sort">Sort.</param>
        public void AddSort(Sorting sort)
        {
            sorts.Add(sort);
        }
    }
}
