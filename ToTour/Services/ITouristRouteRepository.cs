using ToTour.Helpers;
using ToTour.Models;

namespace ToTour.Services
{
    //数据仓库的接口文件
    public interface ITouristRouteRepository
    {
        //希望数据库能给我所有旅游路线的数据
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(string? keyword,string? ratingOperator, int? ratingValue, int pageSize, int pageNumber, string? orderBy);// (string keyword, string operatorType, int? raringVlaue);
        
        //希望通过特定的旅游路线的Id拿到某个特定的路线
        Task<TouristRoute?> GetTouristRouteAsync(Guid touristRouteId);

        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids);

        //检查指定Id的路线是否存在
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
        
        Task<IEnumerable<TouristRoutePicture>> GetPictureByTouristRouteIdAsync(Guid touristRouteId);
        
        Task<TouristRoutePicture?> GetPictureAsync(int pictureId);
        
        void AddTouristRoute(TouristRoute touristRoute);
        
        void AddTouristRoutePicture(Guid touristrRouteId, TouristRoutePicture touristRoutePicture);

        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutePicture(TouristRoutePicture picture);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
        Task<ShoppingCart?> GetShoppingCartByUserIdAsync(string userId);
        Task CreateShoppingCartAsync(ShoppingCart shoppingCart);
        Task AddShoppingCartItemAsync(LineItem lineItem);
        Task<LineItem?> GetShoppingCartItemByItemIdAsync(int lineItemId);
        void DeleteShoppingCartItem(LineItem lineItem);
        Task<IEnumerable<LineItem>> GetShoppingCartItemsByItemIdsAsync(IEnumerable<int> ids);
        void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);
        
        Task AddOrderAsync(Order order);

        Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userid, int pageSize,int pageNumber);

        Task<Order?> GetOrderByIdAsync(Guid orderId);

        Task<bool> SaveAsync(); //保存到数据库
    }
}
