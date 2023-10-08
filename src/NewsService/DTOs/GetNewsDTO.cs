using NewsService.Models;

namespace NewsService.DTOs
{
    public class GetNewsDTO
    {
        public GetNewsDTO(List<News> results, int pageCount, int totalCount)
        {
            Results = results;          
            PageCount = pageCount;
            TotalCount = totalCount;
        }
        public List<News> Results{get; set;}
        public int PageCount { get; set;}
        public int TotalCount { get; set;}
    }
}
