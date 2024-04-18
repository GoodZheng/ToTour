using Microsoft.EntityFrameworkCore;
using ToTour.Database;
using ToTour.Dtos;
using ToTour.Helpers;
using ToTour.Models;

namespace ToTour.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        //数据库连接器
        private readonly AppDbContext _context;

        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRouteRepository(AppDbContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<TouristRoute?> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);//找不到返回null
        }

        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(string? keyword, string? ratingOperator, int? ratingValue, int pageSize, int pageNumber, string? orderBy)
        {
            IQueryable<TouristRoute> result = _context
                .TouristRoutes.
                Include(t => t.TouristRoutePictures); //相当于是生成SQL语句


            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));
            }
            if (ratingValue >= 0)
            {
                result = ratingOperator switch
                {
                    "largerThan" => result.Where(t => t.Rating >= ratingValue),
                    "lessThan" => result.Where(t => t.Rating <= ratingValue),
                    _ => result.Where(t => t.Rating == ratingValue),
                };
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                // 属性映射字典
                var touristRouteMappingDictionary = _propertyMappingService.GetPropertyMapping<TouristRouteDto, TouristRoute>();

                result = result.ApplySort(orderBy, touristRouteMappingDictionary);
            }

            return await PaginationList<TouristRoute>.CreateAsync(pageNumber, pageSize, result);

            //return await result.ToListAsync(); //ToList是IQueryable内建函数，调用后IQueryable马上执行对数据库的访问，
            //并且把结果以列表返回。类似的还有FirstOrDefault函数（处理单条数据）
        }

        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId);
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPictureByTouristRouteIdAsync(Guid touristRouteId)
        {
            //进入数据库拿数据
            return await _context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId).ToListAsync();
        }

        public async Task<TouristRoutePicture?> GetPictureAsync(int pictureId)
        {
            return await _context.TouristRoutePictures.Where(p => p.Id == pictureId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
            {
                throw new ArgumentException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);

            //_context.SaveChanges();
        }



        public void AddTouristRoutePicture(Guid touristrRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristrRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristrRouteId));
            }
            if (touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristrRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }

        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        public void DeleteTouristRoutePicture(TouristRoutePicture picture)
        {
            _context.TouristRoutePictures.Remove(picture);
        }



        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<ShoppingCart?> GetShoppingCartByUserIdAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User).
                Include(s => s.ShoppingCartItems).ThenInclude(li => li.TouristRoute)
                .Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateShoppingCartAsync(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        public async Task AddShoppingCartItemAsync(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }

        public async Task<LineItem?> GetShoppingCartItemByItemIdAsync(int lineItemId)
        {
            return await _context.LineItems.Where(li => li.Id == lineItemId).FirstOrDefaultAsync();
        }

        public void DeleteShoppingCartItem(LineItem lineItem)
        {
            _context.LineItems.Remove(lineItem);
        }

        public async Task<IEnumerable<LineItem>> GetShoppingCartItemsByItemIdsAsync(IEnumerable<int> ids)
        {
            return await _context.LineItems.Where(li => ids.Contains(li.Id)).ToListAsync();
        }

        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userid, int pageSize, int pageNumber)
        {
            //return await _context.Orders.Where(o=>o.UserId == userid).ToListAsync();
            IQueryable<Order> result = _context.Orders.Where(o => o.UserId == userid);
            return await PaginationList<Order>.CreateAsync(pageNumber, pageSize, result);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.TouristRoute)
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }
    }
}
