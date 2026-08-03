using System.Text.Json;
using LearnHubApi.RequestHelpers;

namespace LearnHubApi.Extensions;

public static class HttpExtension
{
    public static void AddPaginationHeader(this HttpResponse response,PaginationMetaData metaData)
    {
        var options=new JsonSerializerOptions
        {
            PropertyNamingPolicy=JsonNamingPolicy.CamelCase
        };
        response.Headers.Append("Pagination",JsonSerializer.Serialize(metaData,options));
        response.Headers.Append("Access-Control-Expose-Headers","Pagination");
    }
}