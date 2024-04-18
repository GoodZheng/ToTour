using Microsoft.EntityFrameworkCore;

namespace ToTour.Helpers
{
    public class PaginationList<T>:List<T>
    {
        public int TotalPages { get; set; } //页面总量
        public int TotalCount { get; set; } //数据库的总数据量
        public bool HasPrevious => CurrentPage > 1; //判断是否有上一页
        public bool HasNext => CurrentPage < TotalPages; //判断是否有下一页
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
        {
            CurrentPage = currentPage;

            PageSize = pageSize;

            AddRange(items);

            TotalCount = totalCount;

            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize); //总页数由总数据量与每页页数计算得出
        }

        // 工厂模式
        public static async Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync(); // 访问数据库获取数据总量

            // 分页
            // 1.跳过数据
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            // 2.以pageSize为标准显示一定量的数据
            result = result.Take(pageSize);

            // Eager Load 立即加载 （include vs join ）
            var items = await result.ToListAsync(); // 访问数据库获取数据list

            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }
    }
}
