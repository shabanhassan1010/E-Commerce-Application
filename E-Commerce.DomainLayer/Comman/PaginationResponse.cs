

namespace E_Commerce.InfrastructureLayer
{
    public class PaginationResponse<T>
    {
        public int PageIndex { get; set; }  // Page Number
        public int PageSize { get; set; }  // Number of items in One Page
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; }
        public PaginationResponse(int pageIndex, int pageSize, int totalItems, IEnumerable<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            Data = data;
        }
    }
}
