namespace ToTour.ResourceParameters
{
    public class PaginationResourceParameters
    {
        private int _pageNumber = 1;
        public int PageNumber //用户要访问的页数
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if (value >= 1)
                    _pageNumber = value;
            }
        }

        private int _pageSize = 3;
        const int maxPageSize = 50;
        public int PageSize //设置每页的大小
        {
            get { return _pageSize; }
            set
            {
                if (value >= 1)
                {
                    _pageSize = value > maxPageSize ? maxPageSize : value;
                }
            }
        }
    }
}
