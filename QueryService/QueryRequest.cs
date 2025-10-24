namespace AdvanceFiltringSytem.QueryService
{
    public class QueryRequest
    {
        public Dictionary<string, FilterCondition>? Filter { get; init; }  // Exact or operator filters
        public Dictionary<string, string>? Search { get; init; }  // Contains searches
        public string? SortBy { get; init; } = "Id";
        public string? SortDirection { get; init; } = "asc";
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }

    public class FilterCondition
    {
        public string Operator { get; set; } = "eq"; // eq, gt, lt, gte, lte, neq, in
        public object? Value { get; set; }
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
