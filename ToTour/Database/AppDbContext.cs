using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
using ToTour.Models;

namespace ToTour.Database
{
    public class AppDbContext:IdentityDbContext<ApplicationUser> //DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        //模型映射
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 添加数据库种子数据
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            //{
            //    Id = Guid.NewGuid(),
            //    Title = "testTitle",
            //    Description = "This is a seed data!",
            //    OriginalPrice = 0,
            //    CreateTime = DateTime.UtcNow
            //});

            //json批量添加 添加数据库种子数据
            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures); 
            #endregion

            // 初始化用户与角色的种子数据
            // 1. 更新用户与角色的外键关系
            modelBuilder.Entity<ApplicationUser>(u =>
            {
                u.HasMany(x => x.UserRoles) //一个User对于多个UserRoles
                .WithOne().HasForeignKey(ur => ur.UserId) //一个UserRoles对于一个外键（使用UserId作为外键）
                .IsRequired(); //外键是必须的
            });

            // 2. 添加角色种子数据
            var adminRoleId = "408660dc-ae51-480f-824d-7dca6714c3e2"; // guid 
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            );

            // 3. 添加用户种子数据
            var adminUserId = "10184155-dee0-40c9-bb1e-b5ed07afc04e";
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@totour.com",
                NormalizedUserName = "admin@totour.com".ToUpper(),
                Email = "admin@totour.com",
                NormalizedEmail = "admin@totour.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = false
            };
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();//为了给密码进行hash而创建的对象
            adminUser.PasswordHash = ph.HashPassword(adminUser, "1622990174aA@");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // 4. 给用户加入管理员权限
            // 通过使用 linking table：IdentityUserRole
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>()
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
