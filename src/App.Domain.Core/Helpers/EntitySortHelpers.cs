using System.Linq;
using System.Linq.Expressions;
using App.Domain.Core.Models;
using App.Domain.Core.Models.Rule;

namespace App.Domain.Core.Helpers
{
    /// <summary>
    /// Extensions and utility methods to work with sort orders
    /// </summary>
    public static class EntitySortHelpers
    {
        /// <summary>
        /// Creates a query to sort type by Sort fields applying them sequentially,
        /// useful for (de)serialisation of Sort rules
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sort">Collection of rules - sortable field names</param>
        /// <param name="isOrdered">If query is already ordered, ThenBy methods will be used. Default false.</param>
        /// <returns>IOrderedQueryable</returns>
        public static IQueryable<T> OrderByRules<T>(this IQueryable<T> query, EntitySort<T> sort, bool isOrdered = false)
        {
            if (sort == null || sort.SortRules == null)
                return query;

            // query is IOrderedQueryable<T> for NHibernate always returns true but fails because there does not have orderby expression
            foreach (SortRule rule in sort.SortRules)
            {
                if (rule.IsAscending)
                {
                    if (isOrdered)
                        query = ((IOrderedQueryable<T>)query).ThenBy(rule.Name);
                    else
                        query = query.OrderBy(rule.Name);
                }
                else
                {
                    if (isOrdered)
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(rule.Name);
                    else
                        query = query.OrderByDescending(rule.Name);
                }

                // now it must be true
                isOrdered = true;
            }

            return query;
        }

        private static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, false);
        }

        private static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, false);
        }

        private static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, true);
        }

        private static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, true);
        }

        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression parameter = Expression.Parameter(source.ElementType, "p");

            MemberExpression memberAccess = null;
            foreach (var property in propertyName.Split('.'))
                memberAccess = MemberExpression.Property(memberAccess ?? (parameter as Expression), property);

            LambdaExpression sort = Expression.Lambda(memberAccess, parameter);

            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { source.ElementType, memberAccess.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }
    }
}
