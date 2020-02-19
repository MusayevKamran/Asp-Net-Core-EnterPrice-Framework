using App.Domain.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace App.Domain.Core.Interfaces
{
    public interface IRepositoryBase
    {
        IQueryable<T> GetFiltered<T>(
            IQueryable<T> query,
            EntityFilter<T> filter = null, EntitySort<T> sort = null,
            int limit = -1, int skip = -1)
            where T : class;

        IQueryable<IDictionary<string, object>> GetFilteredFields<T>(
            IQueryable<T> query, string[] entityFields,
            EntityFilter<T> filter = null, EntitySort<T> sort = null,
            int limit = -1, int skip = -1)
            where T : class;
    }
}
