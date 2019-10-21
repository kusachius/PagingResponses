using System;
using System.Collections;
using Microsoft.AspNetCore.Http;

namespace Kusachius.PagingResponses
{
    public class CriteriaFactory
    {
        readonly ICriteriaMapperFactory mapperFactory;
        readonly IStringCleaner cleaner;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Kusachius.PagingResponses.CriteriaFactory"/> class.
        /// </summary>
        /// <param name="mapperFactory">Mapper factory.</param>
        /// <param name="cleaner">Cleaner.</param>
        public CriteriaFactory(ICriteriaMapperFactory mapperFactory, IStringCleaner cleaner)
        {
            this.mapperFactory = mapperFactory;
            this.cleaner = cleaner;
        }

        /// <summary>
        /// Creates criteria from request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Criteria<T> CreateFromRequest<T>(IQueryCollection query)
        {
            var mapper = mapperFactory.GetMapper<T>();
            var criteria = new Criteria<T>(mapper, cleaner);
            foreach (var q in query)
            {
                switch (q.Key.ToLower())
                {
                    case "pagesize":
                        criteria.SetPageSize(int.Parse(q.Value));
                        break;
                    case "page":
                        int page;
                        if (int.TryParse(q.Value, out page))
                        {
                            criteria.PagingInfo.CurrentPage = page;
                        }
                        else
                        {
                            criteria.PagingInfo.CurrentPage = 1;
                        }
                        break;
                    case "sumfields":
                        criteria.sumFields = ((string)q.Value).Split(',');
                        break;
                    default:
                        var parts = SplitKey(q.Key);
                        if (parts.Length > 1)
                        {
                            if (parts[1] == "sort")
                            {
                                criteria.AddSort(new Sorting(parts[0], q.Value));
                            }
                            else
                            {
                                criteria.AddCondition(new Condition(parts[0], MapOperator(parts[1]), q.Value));
                            }
                        }
                        break;
                }

            }
            return criteria;
        }

        /// <summary>
        /// Splits the key.
        /// </summary>
        /// <returns>The key.</returns>
        /// <param name="key">Key.</param>
        private string[] SplitKey(string key)
        {
            // format key[operation]
            var parts = key.Split("[");
            if (parts.Length > 1)
            {
                if (!parts[1].EndsWith(']'))
                {
                    throw new PagingResponseException($"Invalid criteria operation: {key}");
                }
                parts[1] = parts[1].Substring(0, parts[1].Length - 1);
            }

            return parts;
        }

        /// <summary>
        /// Maps the operator.
        /// </summary>
        /// <returns>The operator.</returns>
        /// <param name="opName">Op name.</param>
        private string MapOperator(string opName)
        {
            switch (opName)
            {
                case "eq":
                    return "=";
                case "neq":
                    return "<>";
                case "gt":
                    return ">";
                case "lt":
                    return "<";
                case "lte":
                    return "<=";
                case "gte":
                    return ">=";
                case "-eq":
                    return "+=";
                case "-neq":
                    return "+<>";
                case "-in":
                    return "+in";
            }

            return opName;
        }
    }
}
