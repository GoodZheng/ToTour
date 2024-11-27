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
        var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]); //�������ļ���ȡ˽Կ

        option.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = Configuration["Authentication:Issuer"],

            ValidateAudience = true,
            ValidAudience = Configuration["Authentication:Audience"],

            ValidateLifetime = true,

            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
    }); //��֤���������ע��

// Add services to the container.  ��ӷ�������ע������

//ע��MVC��Controllers��� 
builder.Services.AddControllers(setupAction =>
{
    //setuoAction.ReturnHttpNotAcceptable = false; //����Ϊfalse��ʾ�ظ�Ĭ�ϵ����ݽṹjson����������������ͷ��
    setupAction.ReturnHttpNotAcceptable = true;
    //setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()); //���Ӷ�xml��֧�֡��¿����ֱ�ӵ���AddXmlDataContractSerializerFormatters����
}).AddNewtonsoftJson(setupAction => {
    setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//��Ӷ�JsonPatch��֧��
}).AddXmlDataContractSerializerFormatters();  //��Ӷ�����xml��ʽ���ݵ�֧��

//builder.Services.AddTransient<ITouristRouteRepository,MockTouristRouteRepository>(); //����ע�룺ע������·�ߵ����ݲֿ�
builder.Services.AddTransient<ITouristRouteRepository, TouristRouteRepository>(); //����ע�룺ע������·�ߵ����ݲֿ�
//builder.Services.AddSingleton ���ܽ�������ע��
//builder.Services.AddScoped 

//var connectionString = builder.Configuration["DbContext:MySQLConnectionString"];
builder.Services.AddDbContext<AppDbContext>(option =>
{
    string connStr = builder.Configuration.GetSection("MySQLConnectionString").Value;
    option.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
});

//�Զ�ɨ�����ӳ���ϵ��profile�ļ���AddAutoMapper�Ὣ���е�profile�ļ����ص�Ŀǰ��AppDomain��
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>(); //���������ַ��������Ե�ת��

builder.Services.Configure<MvcOptions>(config =>
{
    var outputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
    
    outputFormatter?.SupportedMediaTypes.Add("application/vnd.peano.hateoas+json");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); //�� Swagger �����ӵ����������С�Swagger �������ɺ���ʾ API ���ĵ��ͽ��档
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// �����ģ�
//app.UseRouting();
// ����˭��
app.UseAuthentication();
// ����ʲôȨ�ޣ�
app.UseAuthorization();

//app.MapGet("/test", () =>
//{
//    return "Hello My First APS Project!";
//}); //����·�����

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

app.MapControllers(); //ʹ��MVC��ӳ�����ֱ�Ӷ�·�ɽ���ӳ��

//app.UseAuthorization();

app.Run();

