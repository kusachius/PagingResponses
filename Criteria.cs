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
		public IEnumerable<string> sumFields;
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

			if (sorts.Count == 0)
			{
				return query;
			}

			IOrderedQueryable<T> qsorted;
			var sort = sorts.First();
			string column = mapper.ToDbSortColumn(sort.Column);
			if (sort.Direction == "asc")
			{
				qsorted = query.OrderBy(e => EF.Property<object>(e, column));
			}
			else
			{
				qsorted = query.OrderByDescending(e => EF.Property<object>(e, column));
			}


			for (int i = 0; i < sorts.Count; i++)
			{
				column = mapper.ToDbSortColumn(sort.Column);
				if (sort.Direction == "asc")
				{
					qsorted = qsorted.ThenBy(e => EF.Property<object>(e, column));
				}
				else
				{
					qsorted = qsorted.ThenByDescending(e => EF.Property<object>(e, column));
				}
			}

			return qsorted;
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

					case "+=":
						query = query.Where(e => EF.Property<int>(e, column) == Convert.ToInt32(val));
						break;

					case ">":
						query = query.Where(e => EF.Property<int>(e, column) > Convert.ToInt32(val));
						break;

					case "<":
						query = query.Where(e => EF.Property<int>(e, column) < Convert.ToInt32(val));
						break;

					case ">=":
						query = query.Where(e => EF.Property<int>(e, column) >= Convert.ToInt32(val));
						break;

					case "<=":
						query = query.Where(e => EF.Property<int>(e, column) <= Convert.ToInt32(val));
						break;

					case "in": // string values
						string[] searchValues = val.Split(',');
						if (searchValues.Length == 1)
						{
							query = query.Where(e => EF.Property<string>(e, column) == searchValues[0]);
						}
						else
						{
							List<string> searchIds = new List<string>();
							foreach (var x in searchValues)
							{
								searchIds.Add(x);
							}
							query = query.Where(e => searchIds.Contains(EF.Property<string>(e, column)));
						}
						break;

					case "+in": // integer only values
						string[] searchValuesInt = val.Split(',');
						if (searchValuesInt.Length == 1)
						{
							int value = Convert.ToInt32(searchValuesInt[0]);
							query = query.Where(e => EF.Property<int>(e, column) == value);
						}
						else
						{
							List<int> searchIds = new List<int>();
							foreach (var x in searchValuesInt)
							{
								searchIds.Add(Convert.ToInt32(x));
							}
							query = query.Where(e => searchIds.Contains(EF.Property<int>(e, column)));
						}
						break;

					case "wlike":
						val = val.Replace("  ", " "); // TODO repeat
						string[] words = val.Split(' ');
						foreach (string word in words)
						{
							query = query.Where(e => EF.Property<string>(e, column).Contains(word));
						}
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

		public IEnumerable<string> GetSumFields()
		{
			return sumFields;
		}
	}
}
