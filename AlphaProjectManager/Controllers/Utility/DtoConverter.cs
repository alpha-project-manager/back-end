using Application.DataQuery;
using Domain.Interfaces;

namespace AlphaProjectManager.Controllers.Utility;

public static class DtoConverter
{
    public static DataQueryParams<T> ConvertBasicDataQuery<T>(int? skip, int? take, string? orderProperty, bool ascending) where T : class, IHasId
    {
        var queryParams = new DataQueryParams<T>
        {
            Paging = null,
            Sorting = null,
            Filters = null,
            IncludeParams = null
        };
        if (skip != null && take != null)
        {
            queryParams.Paging = new PagingParams
            {
                Skip = skip ?? 0,
                Take = take ?? 10
            };
        }

        if (!string.IsNullOrWhiteSpace(orderProperty))
        {
            queryParams.Sorting = new SortingParams<T>
            {
                PropertyName = orderProperty,
                Ascending = ascending
            };
        }
        
        return queryParams;
    }
}