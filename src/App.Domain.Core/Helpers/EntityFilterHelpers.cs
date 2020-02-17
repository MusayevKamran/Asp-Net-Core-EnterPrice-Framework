using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using App.Domain.Core.Enums;
using App.Domain.Core.Interfaces;
using App.Domain.Core.Models;
using App.Domain.Core.Models.Rule;
using App.Domain.Core.Utilities;

namespace App.Domain.Core.Helpers
{
    /// <summary>
    /// Extensions and utility methods to work with filters
    /// </summary>
    public static class EntityFilterHelpers
    {
        /// <summary>
        /// Constructs a query to filter by field values
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filter">Collection of rules for field names</param>
        /// <returns>IQueryable</returns>
        public static IQueryable<T> FilterByRules<T>(this IQueryable<T> query, EntityFilter<T> filter)
        {
            if (filter == null)
                return query;

            var provider = query.Provider;

            var filterRules = filter.Filters;
            if (filterRules != null && filterRules.Count > 0)
            {
                var fullLambda = EntityFilterHelpers.CollectLambda<T>(filterRules,
                                                    Expression.Parameter(query.ElementType, "p"),
                                                    query.Provider is EnumerableQuery<T>);
                // is it simple linq-to-object query? TODO: do we need some other tests in case there are other linq-to-objects providers
                query = query.GetWhereWithLambda(fullLambda);
            }

            return query;
        }

        internal static FilterMatch ExpressionNodeTypeToFilterMatch(ExpressionType exType)
        {
            switch (exType)
            {
                case ExpressionType.Equal:
                    return FilterMatch.Equal;
                case ExpressionType.GreaterThan:
                    return FilterMatch.Greater;
                case ExpressionType.GreaterThanOrEqual:
                    return FilterMatch.GreaterOrEqual;
                case ExpressionType.LessThan:
                    return FilterMatch.Less;
                case ExpressionType.LessThanOrEqual:
                    return FilterMatch.LessOrEqual;
                case ExpressionType.NotEqual:
                    return FilterMatch.NotEqual;
                default:
                    throw new ArgumentException("The expression is not convertible to filter matching rule", "exType");
            }
        }

        private static Expression<Func<T, bool>> CollectLambda<T>(List<IFilterRule> propRules,
                                                                    ParameterExpression parameter,
                                                                    bool isLinqObjectsQuery)
        {
            Expression<Func<T, bool>> fullLambda = null;

            foreach (var rule in propRules)
            {
                if (rule is FilterRule)
                {
                    fullLambda =
                        EntityFilterHelpers.AppendLambdaForMember<T>(fullLambda, (FilterRule)rule, parameter, isLinqObjectsQuery);
                }
                else if (rule is CompositeFilterRule)
                {
                    // we have inner groups
                    var groupLambda =
                        EntityFilterHelpers.CollectLambda<T>(((CompositeFilterRule)rule).Filters, parameter, isLinqObjectsQuery);

                    // Linq wraps groups automatically
                    fullLambda = EntityFilterHelpers.SafeCombineWithLambda(groupLambda, fullLambda,
                        ((CompositeFilterRule)rule).FilterCombination);
                }
            }

            return fullLambda;
        }

        private static Expression<Func<T, bool>> AppendLambdaForMember<T>(Expression<Func<T, bool>> source, FilterRule propRule,
                                                                ParameterExpression parameter,
                                                                    bool isLinqObjectsQuery)
        {
            object val = propRule.Value;

            Expression buildExpression = null;

            var propHierarchy = propRule.Name.Split('.');

            foreach (var property in propHierarchy)
            {
                buildExpression =
                    MemberExpression.Property(buildExpression ?? (parameter as Expression), property);
            }

            // some guards

            // for non strings we need some checks
            if (buildExpression.Type != typeof(string))
            {
                // is correct CasIng ?
                if (propRule.IsCaseSensitive)
                    throw new InvalidOperationException("Cannot use case sensitive with non-string values");

                // is proper use of LIKEs ?
                if (propRule.FilterMatch == FilterMatch.Like || propRule.FilterMatch == FilterMatch.StartsWith || propRule.FilterMatch == FilterMatch.EndsWith)
                    throw new InvalidOperationException("Cannot use Like/StartsWith/EndsWith with non-string values");
            }

            if (val == null &&
                new FilterMatch[]
                {
                    FilterMatch.Like,
                    FilterMatch.StartsWith,
                    FilterMatch.EndsWith,
                    FilterMatch.Greater,
                    FilterMatch.Less,
                    FilterMatch.GreaterOrEqual,
                    FilterMatch.LessOrEqual}.Contains(propRule.FilterMatch))
            {
                throw new InvalidOperationException("Nulls can be compared only with Equals and NotEquals");
            }

            if (buildExpression.Type == typeof(string) &&
                !propRule.IsCaseSensitive && val != null) // no need to care about case when comparing to nulls
            {
                // safeguard against nulls
                // without coalesce, EnumerableQuery will throw NullRef when used on IEnumerable->AsQueryable datasources
                // otherwise if using some non-object providers (like NHib), it is the providers responsibility to deal with nulls correctly
                if (isLinqObjectsQuery)
                    buildExpression = Expression.Coalesce(buildExpression, Expression.Constant(string.Empty));

                buildExpression = Expression.Call(buildExpression, "ToLower", null, null);

                // ToLower generates a bit inefficient query: (lower(cast(table0_.Field as nchar)) like (''%''+@p1+''%'')) 
                // but IndexOf with OrdinalIgnoreCase throws NotSupported for NHibernate, so we have to stay with ToLower

                // will throw if val is not a string
                val = ((string)val).ToLower();
            }

            Expression constex = Expression.Constant(val);

            if (val != null)
            {
                // need conversion for floats-doubles and ints-longs, and also nullables
                Type tEx = buildExpression.Type;

                Type tVal = val.GetType();
                // cannot convert to underlying type - throws a Nullable comparison errors
                Type underNull = Nullable.GetUnderlyingType(buildExpression.Type);

                // deal with nullables with the same type in a special way because LINQ acts differently from C# - 
                // it does not lift non-nullables up

                if (underNull != null)
                {
                    // lift the value to nullable
                    if (underNull == tVal)
                    {
                        constex = Expression.Convert(Expression.Constant(val), tEx);
                    }
                    // see if we don't have the tricky enum<->int case
                    else if (underNull.IsEnum && tVal == typeof(int))
                    {
                        // convert the int back enum and then to the null-lifted enum, so underlying query provider deals with it
                        constex = Expression.Convert(Expression.Constant(
                                            Enum.ToObject(underNull, val)), tEx);
                    }
                    else
                        throw new InvalidOperationException(string.Format("The nullable field {0} of type {1} cannot be compared to a value of type {2}.",
                                                                                        propRule.Name, tEx.FullName, tVal.FullName));
                }
                else // no nullable lifting issues, try converting what we have here
                if (tEx != tVal)
                {
                    // convert to the most specific type in the hierarchy
                    if (tEx.IsAssignableFrom(tVal))
                    {
                        object obj = tVal.IsValueType ? Activator.CreateInstance(tVal) : null;

                        // tEx is greater, convert to more specific
                        // also coalesce if the member could be null
                        buildExpression = Expression.Convert(Expression.Coalesce(buildExpression,
                            Expression.Constant(obj)), tVal);
                    }
                    else if (tVal.IsAssignableFrom(tEx))
                    {
                        // tVal is greater, convert to more specific
                        constex = Expression.Constant(Convert.ChangeType(val, tEx));
                    }
                    // when passing binary expressions, linq converts enum values automatically to int, 
                    // so member access expression might be wrong and cause error if we don't take special care of it
                    else if (tEx.IsEnum && tVal == typeof(int))
                    {
                        // convert the int back to the enum, so underlying query provider deals with it
                        constex = Expression.Constant(Enum.ToObject(tEx, val));
                    }
                    else
                        throw new InvalidOperationException(string.Format("The field {0} of type {1} cannot be compared to a value of type {2}.",
                                                                                            propRule.Name, tEx.FullName, tVal.FullName));
                }
            }
            // else we'll have something freeform and it might throw later


            // else left case sensitive.
            // NOTICE: case sensitive depends on DB also
            if (propRule.FilterMatch == FilterMatch.Like)
            {
                // safeguard against nulls
                // without coalesce, EnumerableQuery will throw NullRef when used on IEnumerable->AsQueryable datasources
                // otherwise if using some non-object providers (like NHib), it is the providers responsibility to deal with nulls correctly
                if (isLinqObjectsQuery)
                    buildExpression = Expression.Coalesce(buildExpression, Expression.Constant(string.Empty));

                buildExpression = Expression.Call(buildExpression,
                                        typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                                        constex);
            }
            else if (propRule.FilterMatch == FilterMatch.StartsWith)
            {
                // safeguard against nulls
                // without coalesce, EnumerableQuery will throw NullRef when used on IEnumerable->AsQueryable datasources
                // otherwise if using some non-object providers (like NHib), it is the providers responsibility to deal with nulls correctly
                if (isLinqObjectsQuery)
                    buildExpression = Expression.Coalesce(buildExpression, Expression.Constant(string.Empty));

                buildExpression = Expression.Call(buildExpression,
                                        typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
                                        constex);
            }
            else if (propRule.FilterMatch == FilterMatch.EndsWith)
            {
                // safeguard against nulls
                // without coalesce, EnumerableQuery will throw NullRef when used on IEnumerable->AsQueryable datasources
                // otherwise if using some non-object providers (like NHib), it is the providers responsibility to deal with nulls correctly
                if (isLinqObjectsQuery)
                    buildExpression = Expression.Coalesce(buildExpression, Expression.Constant(string.Empty));

                buildExpression = Expression.Call(buildExpression,
                                        typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }),
                                        constex);
            }
            else if (propRule.FilterMatch == FilterMatch.Equal)
            {
                buildExpression = Expression.Equal(buildExpression,
                                        constex);
            }
            else if (propRule.FilterMatch == FilterMatch.Greater)
            {
                // TODO: if strings are needed, we could go CompareTo route
                // http://stackoverflow.com/questions/2061398/dynamic-linq-2-sql-using-expressions-trees-raising-exception-binary-operator-le
                // if only NHibernate supports that
                buildExpression = Expression.GreaterThan(buildExpression,
                                        constex);
            }
            else if (propRule.FilterMatch == FilterMatch.Less)
            {
                buildExpression = Expression.LessThan(buildExpression,
                                      constex);
            }
            else if (propRule.FilterMatch == FilterMatch.GreaterOrEqual)
            {
                buildExpression = Expression.GreaterThanOrEqual(buildExpression,
                                       constex);
            }
            else if (propRule.FilterMatch == FilterMatch.LessOrEqual)
            {
                buildExpression = Expression.LessThanOrEqual(buildExpression,
                                      constex);
            }
            else if (propRule.FilterMatch == FilterMatch.NotEqual)
            {
                buildExpression = Expression.NotEqual(buildExpression,
                                      constex);
            }

            Expression<Func<T, bool>> current =
                        (Expression<Func<T, bool>>)Expression.Lambda(buildExpression, parameter);

            source =
                EntityFilterHelpers.SafeCombineWithLambda(current, source, propRule.FilterCombination);

            return source;
        }

        private static Expression<Func<T, bool>> SafeCombineWithLambda<T>(Expression<Func<T, bool>> source,
            Expression<Func<T, bool>> target,
            FilterCombination filterCombination)
        {
            if (target == null)
            {
                // nothing to combine with yet
                target = source;
            }
            else
            {
                // cannot use predicate builder here - it groups the first part before adding the end
                if (filterCombination == FilterCombination.And)
                    target = target.And(source);
                else if (filterCombination == FilterCombination.Or)
                    target = target.Or(source);
                else
                    throw new ArgumentException("Cannot use the given operation with the existing lambda", "filterCombination");
            }

            return target;
        }

        private static IQueryable<T> GetWhereWithLambda<T>(this IQueryable<T> source,
            Expression<Func<T, bool>> fullLambda)
        {
            MethodCallExpression result = Expression.Call(
                              typeof(Queryable), "Where",
                              new[] { source.ElementType },
                              source.Expression,
                              fullLambda);

            return source.Provider.CreateQuery<T>(result);
        }
    }
}
