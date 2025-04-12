using ReportHub.Application.Validators.Exceptions;
using System.Linq.Expressions;
using System.Reflection;

namespace ReportHub.Application.Common.Helper
{
    public class ExpressionBuilder
    {
        public static Expression<Func<T, object>> BuildSortExpression<T>(string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (propertyInfo == null)
            {
                throw new BadRequestException($"Property '{propertyName}' does not exist on type '{typeof(T).Name}'");
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, propertyName);
            var converted = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(converted, parameter);
        }
    }
}
