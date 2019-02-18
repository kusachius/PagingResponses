using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Kusachius.PagingResponses
{
    public class Criteria<T> : ICriteria<T>
    {
        List<Condition> conditions = new List<Condition>();
        List<Sorting> sorts = new List<Sorting>();
        ICriteriaMapper mapper;
        IStringCleaner cleaner;

        public PagingInfo<T> PagingInfo { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kusachius.PagingResponses.Criteria"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="cleaner">Cleaner.</param>
        public Criteria(ICriteriaMapper mapper, IStringCleaner cleaner)
        {
            this.mapper = mapper;
            this.cleaner = cleaner;
            PagingInfo = new PagingInfo<T>
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
        public IQueryable<T> Apply<K>(IQueryable<T> query)
        {
            query = ApplyWhere<K>(query);
            PagingInfo.TotalsQuery = query;

            query = ApplySort<K>(query);
            query = ApplyPaging<K>(query);

            // return query.paginate(this.getPageSize());
            return query;
        }

        /// <summary>
        /// Applies the sort.
        /// </summary>
        /// <returns>The sort.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private IQueryable<T> ApplySort<K>(IQueryable<T> query)
        {
            foreach (var sort in sorts)
            {
                string column = mapper.ToDbSortColumn(sort.Column);
                if (sort.Direction == "asc")
                {
                    query = query.OrderBy(e => EF.Property<string>(e, column));
                }
                else
                {
                    query = query.OrderByDescending(e => EF.Property<string>(e, column));
                }
            }

            return query;
        }

        /// <summary>
        /// Applies the where.
        /// </summary>
        /// <returns>The where.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private IQueryable<T> ApplyWhere<K>(IQueryable<T> query)
        {
            foreach (var cond in conditions)
            {
                string column = mapper.ToDbWhereColumn(cond.Column);
                string val = cleaner.Clean(cond.Value);
                switch (cond.Operation)
                {
                    case "=":
                        query = query.Where(e => EF.Property<string>(e, column) == val);
                        break;
                    case "like":
                        int start = 0;
                        int end = 0;
                        if (val.StartsWith('%'))
                        {
                            start = 1;
                            val = val.Substring(1);
                        }
                        if (val.EndsWith('%'))
                        {
                            end = 2;
                            val = val.Substring(0, val.Length - 1);
                        }
                        switch (start + end)
                        {
                            case 1: // start
                                query = query.Where(e => EF.Property<string>(e, column).StartsWith(val, StringComparison.InvariantCulture));
                                break;
                            case 2: // end
                                query = query.Where(e => EF.Property<string>(e, column).EndsWith(val, StringComparison.InvariantCulture));
                                break;
                            default: // both
                                query = query.Where(e => EF.Property<string>(e, column).Contains(val));
                                break;
                        }
                        break;
                }
            }

            return query;
        }

        /// <summary>
        /// Applies the paging.
        /// </summary>
        /// <returns>The paging.</returns>
        /// <param name="query">Query.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private IQueryable<T> ApplyPaging<K>(IQueryable<T> query)
        {
            return query.Skip(PagingInfo.RecordsToSkip()).Take(PagingInfo.PageSize);
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
            this.PagingInfo.PageSize = pageSize;
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
