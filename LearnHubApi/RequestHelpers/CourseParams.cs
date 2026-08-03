namespace LearnHubApi.RequestHelpers;

public class CourseParams : PaginationParams
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public string? Language { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public double? MinDuration { get; set; }

    public double? MaxDuration { get; set; }

    public string? SortBy { get; set; }
}