using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kusachius.PagingResponses
{
    public class PagedActionResult<T, K> : IActionResult
    {
        readonly PagedResult<K> data;
        readonly ApiResult<T> result= new ApiResult<T>();
        readonly IMapper mapper;

        public PagedActionResult(PagedResult<K> data, IMapper mapper = null)
        {
            this.data = data;
            this.mapper = mapper;
        }


        /// <summary>
        /// Executes the result async.
        /// </summary>
        /// <returns>The result async.</returns>
        /// <param name="context">Context.</param>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            result.Meta.Pagination.CurrentPage = data.CurrentPage;
            result.Meta.Pagination.PerPage = data.PerPage;
            if (mapper != null)
            {
                result.Data = mapper.Map<IEnumerable<T>>(data.Data);
            }
            else
            {
                result.Data = (IEnumerable<T>)data.Data;
            }
            result.Meta.Pagination.Total = data.TotalItems;
            result.Meta.Pagination.TotalPages = (int)Math.Ceiling((float)result.Meta.Pagination.Total / data.PerPage);
            result.Meta.Pagination.Totals = data.Totals;

            var objectResult = new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status200OK
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
