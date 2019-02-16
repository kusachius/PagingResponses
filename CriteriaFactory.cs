using System;
using System.Collections;
using Microsoft.AspNetCore.Http;

namespace Kusachius.PagingResponses
{
    public class CriteriaFactory
    {
        ICriteriaMapperFactory mapperFactory;
        IStringCleaner cleaner;

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
        /// Creates from request.
        /// </summary>
        /// <returns>The from request.</returns>
        /// <param name="query">Query.</param>
        /// <param name="classType">Class name.</param>
        public Criteria CreateFromRequest(IQueryCollection query, Type classType)
        {
            var mapper = mapperFactory.GetMapper(classType);
            var criteria = new Criteria(mapper, cleaner);
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
                    default:
                        var parts = splitKey(q.Key);
                        if (parts.Length > 1)
                        {
                            if (parts[1] == "sort")
                            {
                                criteria.AddSort(new Sorting(parts[0], q.Value));
                            }
                            else
                            {
                                criteria.AddCondition(new Condition(parts[0], mapOperator(parts[1]), q.Value));
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
        private string[] splitKey(string key)
        {
            // format key[operation]
            var parts = key.Split("[");
            if (parts.Length > 1)
            {
                parts[1] = parts[1].Substring(0, parts[1].Length - 1);
            }

            return parts;
        }

        /// <summary>
        /// Maps the operator.
        /// </summary>
        /// <returns>The operator.</returns>
        /// <param name="opName">Op name.</param>
        private string mapOperator(string opName)
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
                case "gtq":
                    return ">=";
            }

            return opName;
        }
    }
}
