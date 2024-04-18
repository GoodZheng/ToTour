using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToTour.Dtos;
using ToTour.Helpers;
using ToTour.Models;
using ToTour.Services;

namespace ToTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        public ShoppingCartController(IHttpContextAccessor httpContextAccessor, ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }


        #region 获取购物车信息
        [HttpGet(Name = "GetShoppingCart")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetShoppingCart()
        {
            //获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            //根据userid获取购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);

            var res = _mapper.Map<ShoppingCartDto>(shoppingCart);
            return Ok(res);
        }
        #endregion

        #region 给购物车添加商品
        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem([FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
        {
            // 1.获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            // 2.根据userid获取购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);
            
            // 3.创建lineitem
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound("旅游路线不存在");
            }
            var lineItem = new LineItem()
            {
                TouristRouteId = addShoppingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                Discount = touristRoute.DiscountPresent
            };
            // 4.添加lineitem 并保存数据库
            await _touristRouteRepository.AddShoppingCartItemAsync(lineItem);
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }
        #endregion

        #region 从购物车删除某件商品
        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem = await _touristRouteRepository.GetShoppingCartItemByItemIdAsync(itemId);
            if (lineItem == null)
            {
                return NotFound("购物车商品找不到");
            }

            _touristRouteRepository.DeleteShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
        #endregion

        #region 从购物车批量删除商品
        [HttpDelete("items/({itemIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItems([ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<int> itemIds)
        {
            var lineitems = await _touristRouteRepository.GetShoppingCartItemsByItemIdsAsync(itemIds);
            _touristRouteRepository.DeleteShoppingCartItems(lineitems);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
        #endregion

        #region 提交订单
        [HttpPost("checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CheckOut()
        {
            // 1.获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2.使用userid获取购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);
            if (shoppingCart.ShoppingCartItems.Count == 0 || shoppingCart.ShoppingCartItems == null)
            {
                return NotFound("购物车中没有商品");
            }
            // 3.创建订单
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderStateEnum.Pending,
                OrderItems = shoppingCart.ShoppingCartItems, //把购物车内的商品转移到订单中
                CreateDateUTC = DateTime.UtcNow,
                TransactionMetadata = "先假设第三方返回的是支付成功的信息"
            };
            // 4.结算后清空购物车
            shoppingCart.ShoppingCartItems = null;
            // 5.保存
            await _touristRouteRepository.AddOrderAsync(order);
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<OrderDto>(order));

        }
        #endregion


    }
}
