using System;
using System.Collections.Generic;
using System.Linq;
using App.Domain.Core.Helpers;
using App.Domain.Core.Interfaces;
using App.Domain.Core.Utilities;


namespace App.Domain.Core.Models
{
    public class RepositoryBase : IRepositoryBase
    {
        public IQueryable<T> GetFiltered<T>(
          IQueryable<T> query,
          EntityFilter<T> filter = null, EntitySort<T> sort = null,
          int limit = -1, int skip = -1)
          where T : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            query = query.FilterByRules<T>(filter);
            query = query.OrderByRules<T>(sort, false);

            if (skip > -1)
                query = query.Skip(skip);

            if (limit > -1)
                query = query.Take(limit);

            return query;
        }

        public IQueryable<IDictionary<string, object>> GetFilteredFields<T>(
          IQueryable<T> query,
          string[] entityFields,
          EntityFilter<T> filter = null, EntitySort<T> sort = null,
          int limit = -1, int skip = -1)
          where T : class
        {
            query = this.GetFiltered<T>(query, filter, sort, limit, skip);
            return query.Select<T, IDictionary<string, object>>(LinqUtils.CreateFieldSelectDictionary<T>(entityFields));
        }
    }
}


