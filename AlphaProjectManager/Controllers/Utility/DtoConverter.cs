using AlphaProjectManager.Controllers.ApplicationQuestions.Responses;
using AlphaProjectManager.Controllers.ProjectCases.Responses;
using Application.DataQuery;
using Domain.Entities;
using Domain.Entities.TelegramBot;
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
        queryParams.Paging = new PagingParams
        {
            Skip = skip ?? 0,
            Take = take ?? 10
        };
        
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
    
    public static void MapPropertiesValues<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        var sourceProps = typeof(TSource).GetProperties()
            .Where(p => p.CanRead).ToDictionary(p => p.Name);

        var destProps = typeof(TDestination).GetProperties()
            .Where(p => p.CanWrite);

        foreach (var destProp in destProps)
        {
            if (sourceProps.TryGetValue(destProp.Name, out var sourceProp))
            {
                if (destProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    var value = sourceProp.GetValue(source);
                    destProp.SetValue(destination, value);
                }
            }
        }
    }

}