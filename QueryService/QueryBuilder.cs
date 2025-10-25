using AdvanceFiltringSytem.QueryService;
using System.Linq.Expressions;

public static class QueryBuilder
{
    public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> query, QueryRequest request)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        // Exact filters + operators
        if (request.Filter != null)
        {
            foreach (var (field, condition) in request.Filter)
            {
                var property = Expression.PropertyOrField(parameter, field); // x => x.Field
                var constant = Expression.Constant(Convert.ChangeType(condition.Value, property.Type));
                // Convert filter.value type to x.Field (property) type

                Expression? comparison = condition.Operator.ToLower() switch
                {
                    "eq" => Expression.Equal(property, constant),
                    "neq" => Expression.NotEqual(property, constant),
                    "gt" => Expression.GreaterThan(property, constant),
                    "gte" => Expression.GreaterThanOrEqual(property, constant),
                    "lt" => Expression.LessThan(property, constant),
                    "lte" => Expression.LessThanOrEqual(property, constant),
                    "in" => BuildInExpression(property, condition.Value),
                    _ => null
                };

                if (comparison != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                    query = query.Where(lambda);
                }
            }
        }

        // Search (contains)
        if (request.Search != null)
        {
            foreach (var (field, term) in request.Search)
            {
                var property = Expression.PropertyOrField(parameter, field);
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                var constant = Expression.Constant(term, typeof(string));
                var contains = Expression.Call(property, method, constant);

                var lambda = Expression.Lambda<Func<T, bool>>(contains, parameter);
                query = query.Where(lambda);
            }
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = ApplySorting(query, request.SortBy!, request.SortDirection);
        }

        // Paging
        query = query.Skip((request.PageNumber - 1) * request.PageSize)
                     .Take(request.PageSize);

        return query;
    }

    private static Expression BuildInExpression(MemberExpression property, object? value)
    {
        var list = ((IEnumerable<object>)(value)).Cast<object>().ToList();
        var constant = Expression.Constant(list);
        var contains = typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(property.Type);

        return Expression.Call(contains, constant, property);
    }

    private static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sortBy, string direction)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, sortBy);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = direction.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
    }
}

/* Example JSON Request Body:
 
 {
  "filter": {
    "centerId": { "operator": "eq", "value": 3 },
    "price": { "operator": "gte", "value": 100 },
    "status": { "operator": "in", "value": ["Active", "Pending"] }
  },
  "search": { "name": "service" },
  "sortBy": "price",
  "sortDirection": "desc",
  "pageNumber": 1,
  "pageSize": 20
}

 */
