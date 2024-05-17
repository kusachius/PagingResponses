using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Kusachius.PagingResponses
{
	public class KEnumerable<T>
	{
		public System.Collections.Generic.IEnumerable<T> Data { get; set; }
		public int PerPage { get; set; }
		public int CurrentPage { get; set; }
		public int? TotalPages { get; set; } = null;
		public int? TotalItems { get; set; } = null;
		public object Totals { get; set; } = null;

		/// <summary>
		/// Creates new result
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="criteria">Criteria.</param>
		public static async Task<KEnumerable<T>> CreateResultAsync(IQueryable<T> query, ICriteria<T> criteria = null, bool includeTotals = true)
		{

			KEnumerable<T> result = new KEnumerable<T>();

			if (criteria != null)
			{
				query = criteria.Apply<T>(query);
			}

			result.CurrentPage = criteria.PagingInfo.CurrentPage;
			result.PerPage = criteria.PagingInfo.PageSize;
			if (includeTotals)
			{
				if (criteria.GetSumFields() != null && criteria.GetSumFields().Any())
				{
					var fields = criteria.GetSumFields().ToList();
					for (int i = fields.Count; i < 4; i++)
					{
						fields.Add(fields[0]);
					}

					var q = await criteria.PagingInfo.TotalsQuery.GroupBy(x => 1)
						.Select(x => new
						{
							Totals1 = x.Sum(z => EF.Property<decimal>(z, fields[0])),
							Totals2 = x.Sum(z => EF.Property<decimal>(z, fields[1])),
							Totals3 = x.Sum(z => EF.Property<decimal>(z, fields[2])),
							Totals4 = x.Sum(z => EF.Property<decimal>(z, fields[3])),
							Count = x.Count()
						}).FirstOrDefaultAsync();
					result.TotalItems = q.Count;
					result.Totals = new
					{
						q.Totals1,
						q.Totals2,
						q.Totals3,
						q.Totals4
					};
				}
				else
				{
					result.TotalItems = criteria.PagingInfo.TotalsQuery.Count();
				}
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
		public static KEnumerable<T> CreateResult(IQueryable<T> query, ICriteria<T> criteria = null, bool includeTotals = true)
		{

			KEnumerable<T> result = new KEnumerable<T>();

			if (criteria != null)
			{
				query = criteria.Apply<T>(query);
			}

			result.CurrentPage = criteria.PagingInfo.CurrentPage;
			result.PerPage = criteria.PagingInfo.PageSize;
			if (includeTotals)
			{
				if (criteria.GetSumFields() != null && criteria.GetSumFields().Any())
				{
					var fields = criteria.GetSumFields().ToList();
					for (int i = fields.Count; i < 4; i++)
					{
						fields.Add(fields[0]);
					}

					var q = criteria.PagingInfo.TotalsQuery.GroupBy(x => 1)
						.Select(x => new
						{
							Totals1 = x.Sum(z => EF.Property<decimal>(z, fields[0])),
							Totals2 = x.Sum(z => EF.Property<decimal>(z, fields[1])),
							Totals3 = x.Sum(z => EF.Property<decimal>(z, fields[2])),
							Totals4 = x.Sum(z => EF.Property<decimal>(z, fields[3])),
							Count = x.Count()
						}).FirstOrDefault();
					result.TotalItems = q.Count;
					result.Totals = new
					{
						q.Totals1,
						q.Totals2,
						q.Totals3,
						q.Totals4
					};
				}
				else
				{
					result.TotalItems = criteria.PagingInfo.TotalsQuery.Count();
				}
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
		public KEnumerable<K> ConvertTo<K>(IMapper mapper)
		{
			return new KEnumerable<K>
			{
				PerPage = PerPage,
				CurrentPage = CurrentPage,
				TotalPages = TotalPages,
				TotalItems = TotalItems,

				Data = (System.Collections.Generic.IEnumerable<K>)mapper.Map<System.Collections.Generic.IEnumerable<K>>(Data)
			};
		}

	}
}
