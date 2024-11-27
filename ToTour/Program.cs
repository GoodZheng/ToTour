using ToTour.Database;
using ToTour.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ToTour.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]); //从配置文件获取私钥

        option.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = Configuration["Authentication:Issuer"],

            ValidateAudience = true,
            ValidAudience = Configuration["Authentication:Audience"],

            ValidateLifetime = true,

            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
    }); //验证服务的依赖注入

// Add services to the container.  添加服务到依赖注入容器

//注册MVC的Controllers组件 
builder.Services.AddControllers(setupAction =>
{
    //setuoAction.ReturnHttpNotAcceptable = false; //设置为false表示回复默认的数据结构json，会忽略所有请求的头部
    setupAction.ReturnHttpNotAcceptable = true;
    //setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()); //增加对xml的支持。新框架中直接调用AddXmlDataContractSerializerFormatters即可
}).AddNewtonsoftJson(setupAction => {
    setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//添加对JsonPatch的支持
}).AddXmlDataContractSerializerFormatters();  //添加对请求xml格式数据的支持

//builder.Services.AddTransient<ITouristRouteRepository,MockTouristRouteRepository>(); //依赖注入：注册旅游路线的数据仓库
builder.Services.AddTransient<ITouristRouteRepository, TouristRouteRepository>(); //依赖注入：注册旅游路线的数据仓库
//builder.Services.AddSingleton 都能进行依赖注入
//builder.Services.AddScoped 

//var connectionString = builder.Configuration["DbContext:MySQLConnectionString"];
builder.Services.AddDbContext<AppDbContext>(option =>
{
    string connStr = builder.Configuration.GetSection("MySQLConnectionString").Value;
    option.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
});

//自动扫描包含映射关系的profile文件，AddAutoMapper会将所有的profile文件加载到目前的AppDomain中
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>(); //用于排序字符串与属性的转换

builder.Services.Configure<MvcOptions>(config =>
{
    var outputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
    
    outputFormatter?.SupportedMediaTypes.Add("application/vnd.peano.hateoas+json");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); //将 Swagger 组件添加到服务容器中。Swagger 用于生成和显示 API 的文档和界面。
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 你在哪？
//app.UseRouting();
// 你是谁？
app.UseAuthentication();
// 你有什么权限？
app.UseAuthorization();

//app.MapGet("/test", () =>
//{
//    return "Hello My First APS Project!";
//}); //增加路由组件

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateTime.Now.AddDays(index),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

app.MapControllers(); //使用MVC的映射代替直接对路由进行映射

//app.UseAuthorization();

app.Run();

