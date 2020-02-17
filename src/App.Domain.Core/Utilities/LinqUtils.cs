using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace App.Domain.Core.Utilities
{
    public static class LinqUtils
    {
        /// <summary>
        /// Helps to retrieve member name safely from any kind of expressions
        /// </summary>
        /// <param name="expression">Linq Expression</param>
        /// <returns>Member name</returns>
        public static string GetExpressionMemberName(this Expression expression)
        {
            // maybe wrapped around with lambda?
            if (expression.NodeType == ExpressionType.Lambda)
                expression = ((LambdaExpression)expression).Body;

            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var superName = (memberExpression.Expression).GetExpressionMemberName();
                    return string.IsNullOrEmpty(superName) ? memberExpression.Member.Name : $"{superName}{'.'}{memberExpression.Member.Name}";
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;
                case ExpressionType.Convert: // comes from automatic boxing of long/int to object for lambda expressions
                    var unaryExpression = (UnaryExpression)expression;
                    return GetExpressionMemberName(unaryExpression.Operand);
                case ExpressionType.Parameter:
                case ExpressionType.Constant:
                    return string.Empty;
                default:
                    throw new ArgumentException("The expression is not a member access or method call expression", nameof(expression));
            }
        }

        /// <summary>
        /// Helps to retrieve member expression from one side of a logical expression
        /// </summary>
        /// <param name="logicalRule">Linq logical expression</param>
        /// <returns>Member expression in lambda</returns>
        public static Expression<Func<T, object>> GetExpressionMember<T>(this Expression<Func<T, bool>> logicalRule)
        {
            // get type
            var lb = logicalRule.Body as BinaryExpression;
            if (lb == null)
                throw new ArgumentException("Logical rule must be a binary expression", nameof(logicalRule));

            Expression memeXl = TryExtractMemberExpression(lb.Left);

            // is it valid?
            if (memeXl == null)
                throw new InvalidOperationException("Logical rule must have a member access on the left");

            if (memeXl.Type.IsValueType)
                memeXl = Expression.Convert(memeXl, typeof(object));// need conversion for the following lambda

            return Expression.Lambda<Func<T, object>>(memeXl, logicalRule.Parameters);
        }

        /// <summary>
        /// Helps to retrieve the constant value from one side of a logical expression
        /// </summary>
        /// <param name="logicalRule">Linq logical expression</param>
        /// <returns>Constant value</returns>
        public static object GetExpressionConstant<T>(this Expression<Func<T, bool>> logicalRule)
        {
            // get type
            var lb = logicalRule.Body as BinaryExpression;
            if (lb == null)
                throw new ArgumentException("Logical rule must be a binary expression", nameof(logicalRule));

            var isConstant = CanReduceToConstantExpression(lb.Right);

            // is it valid?
            if (!isConstant)
                throw new InvalidOperationException("Logical rule must have a constant on the right side");

            // as constant might come from various variables or member access paths, we need to compile the full expression
            // to get the value out of it
            // more efficient would be to parse it but we keep it cached anyway
            var value = Expression.Lambda(lb.Right).Compile().DynamicInvoke();

            return value;
        }

        /// <summary>
        /// Helps to retrieve the operator from one side of a logical expression
        /// </summary>
        /// <param name="logicalRule">Linq logical expression</param>
        /// <returns>Operation type</returns>
        public static ExpressionType GetExpressionOperation<T>(this Expression<Func<T, bool>> logicalRule)
        {
            // get type
            var lb = logicalRule.Body as BinaryExpression;
            if (lb == null)
                throw new ArgumentException("Logical rule must be a binary expression", nameof(logicalRule));

            return lb.NodeType;
        }

        /// <summary>
        /// Composes two lambdas with a rule
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="first">First lambda to compose</param>
        /// <param name="second">Second lambda to compose</param>
        /// <param name="merge">Composition rule</param>
        /// <returns>Composed lambda</returns>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Combines two expressions with And
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Expression created</returns>    
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)

        {
            return first.Compose(second, Expression.AndAlso);
        }
        /// Combines two expressions with Or
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Expression created</returns>   
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Creates a select expression which projects only the requested fields into a dictionary.
        /// The other fields are ignored.
        /// </summary>
        /// <param name="fieldNames">Names of fields or properties to select from the type T</param>
        /// <returns>Expression which returns a dictionary with named fields corresponding to fieldNames</returns>
        public static Expression<Func<T, IDictionary<string, object>>> CreateFieldSelectDictionary<T>(params string[] fieldNames)
        {
            if (fieldNames.Length == 0)
                throw new InvalidOperationException("Cannot select zero fields");

            var type = typeof(T);
            var param = Expression.Parameter(type, "item");

            var addMethod = typeof(Dictionary<string, object>).GetMethod(
                        "Add", new[] { typeof(string), typeof(object) });

            // LINQ does not box/unbox, need TypeAs to treat as objects
            var fields = fieldNames.Select(x =>
            {
                Expression buildExpression = null;
                var propHierarchy = x.Split('.');
                foreach (var property in propHierarchy)
                {
                    buildExpression =
                        MemberExpression.PropertyOrField(buildExpression ??
                        (param as Expression), property);
                }

                return Expression.ElementInit(addMethod,
                            Expression.Constant(x),
                            Expression.TypeAs(
                                buildExpression,
                                typeof(object)
                            ));
            });

            return Expression.Lambda<Func<T, IDictionary<string, object>>>(
                Expression.ListInit(
                        Expression.New(typeof(Dictionary<string, object>)),
                        fields), param);
        }

        private static bool CanReduceToConstantExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return true;
                // we may have a conversion for the constant, unbox it
                case ExpressionType.Convert:
                    return CanReduceToConstantExpression(((UnaryExpression)expression).Operand);
                // if we pass variables or complex structure fields, they also are member access
                case ExpressionType.MemberAccess:
                    // traverse deeper
                    return CanReduceToConstantExpression(((MemberExpression)expression).Expression);
                // what about array index?
                case ExpressionType.ArrayIndex:
                    // traverse deeper
                    return CanReduceToConstantExpression(((BinaryExpression)expression).Right);
                default:
                    return false;
            }
        }

        private static MemberExpression TryExtractMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                {
                    // we have a problem - if we pass variables, they also are member access
                    // but with type Constant and not with type Parameter, as expected
                    var memeX = (MemberExpression)expression;
                    if (memeX.Expression.NodeType != ExpressionType.Parameter &&
                        memeX.Expression.NodeType != ExpressionType.MemberAccess)
                        return null;

                    // won't go deeper, return the first available upper level
                    return (MemberExpression)expression;
                }
                // we may have a conversion for the constant, unbox it
                case ExpressionType.Convert:
                    return TryExtractMemberExpression(((UnaryExpression)expression).Operand);
                default:
                    return null;
            }
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }
        }
    }
}


