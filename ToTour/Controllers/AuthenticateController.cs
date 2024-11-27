using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToTour.Dtos;
using ToTour.Models;
using ToTour.Services;

namespace ToTour.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteRepository _touristRouteRepository;

        public AuthenticateController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITouristRouteRepository touristRouteRepository) //注入服务依赖
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepository;
        }

        [AllowAnonymous] // 允许任何人访问。不加也是同样效果，是默认状态
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // 1 验证用户名和密码
            var res = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
            if (!res.Succeeded)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(loginDto.Email); //取出数据库中的对象完整数据

            // 2 创建JWT Token
            // header
            var signingAlgrorithm = SecurityAlgorithms.HmacSha256;
            // payload
            var claims = new List<Claim> //claim是服务器给客户端发的能代表客户端信息的系列数据
            {
                // sub
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                //new Claim(ClaimTypes.Role,"Admin")
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }
            // signiture
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]); //从配置文件获取私钥
            var signingKey = new SymmetricSecurityKey(secretByte); //根据私钥字符串构造对称加密密钥
            var signingCredential = new SigningCredentials(signingKey, signingAlgrorithm); //使用hs256生成摘要/数字签名

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"], //发布者
                audience: _configuration["Authentication:Audience"], //授权方，即此Token将发布给谁。（本项目的授权方应该是项目前端，应是同一个域名）
                claims,
                notBefore: DateTime.UtcNow, //发布时间
                expires: DateTime.UtcNow.AddDays(1), //有效期1天
                signingCredential //数字签名
                );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            // 3 return 200 ok + jwt
            return Ok(tokenStr);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // 1 使用用户名创建用户对象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            // 2 hash 密码，并保存用户到数据库
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest("注册不成功！");
            }

            // 3 初始化用户购物车
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
            };
            await _touristRouteRepository.CreateShoppingCartAsync(shoppingCart);
            await _touristRouteRepository.SaveAsync();
             
            return Ok();
        }

    }
}
