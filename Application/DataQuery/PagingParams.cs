namespace Application.DataQuery;

public class PagingParams
{
    public int Skip { get; set; }

    public int Take { get; set; }

    public PagingParams(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    public PagingParams()
    {
        
    }
}