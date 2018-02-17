using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grid.Cache.Utils
{
    internal static class ReflectionUtils
    {
        public static PropertyInfo GetPropertyFromExpression<T, P>(Expression<Func<T, P>> propertyGetter)
        {
            MemberExpression expression = null;
            if (propertyGetter.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) propertyGetter.Body;
                if (unaryExpression.Operand is MemberExpression)
                {
                    expression = (MemberExpression) unaryExpression.Operand;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else if (propertyGetter.Body is MemberExpression)
            {
                expression = (MemberExpression) propertyGetter.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo) expression.Member;
        }
        
        public static string GetMethodName<T>(Expression<Action<T>> expression)
        {
            MethodCallExpression methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Name;
            }

            throw new ArgumentException("Unable to get method name from provided expression");
        }
    }
}