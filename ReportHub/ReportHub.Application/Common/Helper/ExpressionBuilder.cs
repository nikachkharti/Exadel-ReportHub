using System.Linq.Expressions;

namespace ReportHub.Application.Common.Helper
{
    public class ExpressionBuilder
    {
        public static Expression<Func<T, object>> BuildSortExpression<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, propertyName);
            var converted = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(converted, parameter);
        }
    }
}
