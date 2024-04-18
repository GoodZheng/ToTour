using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ToTour.Dtos;
using ToTour.Helpers;
using ToTour.Models;
using ToTour.ResourceParameters;
using ToTour.Services;

namespace ToTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;

        private readonly IMapper _mapper;
        
        private readonly IUrlHelper _urlHelper;

        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper,IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IPropertyMappingService propertyMappingService)
        {
            _touristRouteRepository = touristRouteRepository; //注入数据仓库服务
            _mapper = mapper;  //给控制器注入AutoMapper的服务依赖
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }

        // Url生成器：生成上/下一页 路径信息
        private string? GenerateTouristRouteResourceUrl(TouristRouteResourceParameters parameters,PaginationResourceParameters parameters2,ResourceUriType type)
        {
            return type switch
            {
                ResourceUriType.PreviousPage => _urlHelper.Link("GetTouristRoutes", new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    keyword = parameters.Keyword,
                    rating = parameters.Rating,
                    pageNumber = parameters2.PageNumber - 1,
                    pageSize = parameters2.PageSize
                }),
                ResourceUriType.NextPage => _urlHelper.Link("GetTouristRoutes", new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    keyword = parameters.Keyword,
                    rating = parameters.Rating,
                    pageNumber = parameters2.PageNumber + 1,
                    pageSize = parameters2.PageSize
                }),
                // 默认页
                _ => _urlHelper.Link("GetTouristRoutes", new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    keyword = parameters.Keyword,
                    rating = parameters.Rating,
                    pageNumber = parameters2.PageNumber,
                    pageSize = parameters2.PageSize
                })
            };
        }

        // 1. 请求头部中请求类型：application/json -> 旅游路线资源
        // 2. 请求头部中请求类型：application/vnd.peano.hateoas + json 
        // 3. application/vnd.peano.touristRoute.simplify+json -> 输出简化版资源数据
        // 4. application/vnd.peano.touristRoute.simplify.hateoas+json -> 输出简化版hateoas超媒体资源数据

        /// <summary>
        /// 获取旅游路线
        /// rating 小于lessThan，大于largerThan,等于equalTo
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead] //直接加上Head请求即可，避免重复代码
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters parameters2,
            [FromHeader(Name ="Accept")] string mediaType)//[FromQuery]string? keyword,string rating)//[FromQuery] TouristRouteResourceParameters parameters //[FromQuery] string keyword, //string rating
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? parsedMediaType))
                return BadRequest();

            if (!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(parameters.OrderBy))
                return BadRequest("请输入正确的排序参数");

            if (!_propertyMappingService.IsPropertiesExists<TouristRoute>(parameters.Fields))
                return BadRequest("请输入正确的塑形参数");

            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(parameters.Keyword,
                                        parameters.OperatorType,
                                        parameters.RatingValue,
                                        parameters2.PageSize,
                                        parameters2.PageNumber,
                                        parameters.OrderBy);// parameters.Keyword, parameters.operatorType, parameters.raringVlaue);
            
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() < 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            //var previousPageLink = touristRouteFromRep.HasPrevious;

            var previousPageLink = touristRoutesFromRepo.HasPrevious
                ? GenerateTouristRouteResourceUrl(parameters, parameters2, ResourceUriType.PreviousPage)
                : null;
            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResourceUrl(parameters, parameters2, ResourceUriType.NextPage)
                : null;
            
            // 给相应的头部加上自定义的分页信息  x-pagination
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };
            Response.Headers.Add("x-pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var shapedDtoList = touristRoutesDto.ShapeData(parameters.Fields);

            if(parsedMediaType.MediaType == "application/vnd.peano.hateoas+json")
            {
                var linkDto = CreateLinksForTouristRouteList(parameters, parameters2);

                var shapedDtoWithLinkList = shapedDtoList.Select(t =>
                {
                    var touristRouteDictionary = t as IDictionary<string, object>;
                    var links = CreateLinkForTouristRoute((Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("links", links);
                    return touristRouteDictionary;
                });

                var result = new
                {
                    value = shapedDtoWithLinkList,
                    links = linkDto
                };

                return Ok(result);
            }

            return Ok(shapedDtoList);
        }

        //api/touristRoutes/{touristRouteId}
        [HttpGet("{touristRouteId}",Name = "GetTouristRouteById")]
        //[HttpHead]
        //[Authorize]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId,string? fields)
        {
            var touristRouteFromRep = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRep == null)
            {
                return NotFound($"旅游路线{touristRouteId}未找到");
            }

            #region DTO手动映射
            //
            //var touristRouteDto = new TouristRouteDto()
            //{
            //    Id = touristRouteFromRep.Id,
            //    Title = touristRouteFromRep.Title,
            //    Description = touristRouteFromRep.Description,
            //    Price = touristRouteFromRep.OriginalPrice * (decimal)(touristRouteFromRep.DiscountPresent ?? 1),
            //    CreateTime = touristRouteFromRep.CreateTime,
            //    UpDateTime = touristRouteFromRep.UpdateTime,
            //    Features = touristRouteFromRep.Features,
            //    Fees = touristRouteFromRep.Fees,
            //    Notes = touristRouteFromRep.Notes,
            //    Rating = touristRouteFromRep.Rating,
            //    TravelDays = touristRouteFromRep.TravelDays.ToString(),
            //    TripType = touristRouteFromRep.TripType.ToString(),
            //    DepartureCity = touristRouteFromRep.DepartureCity.ToString(),
            //};
            //return Ok(touristRouteDto); 
            #endregion

            //DTO自动映射
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRep);
            
            //return Ok(touristRouteDto.ShapeData(fields));
            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);
            var result = touristRouteDto.ShapeData(fields) as IDictionary<string,object>;
            result.Add("links", linkDtos);
            return Ok(result);
        }

        #region HATEOAS
        // 列表资源使用 HATEOAS 
        private IEnumerable<LinkDto> CreateLinksForTouristRouteList(TouristRouteResourceParameters parameters,
            PaginationResourceParameters parameters2)
        {
            var links = new List<LinkDto>();

            // 添加 self 自我链接
            links.Add(new LinkDto(GenerateTouristRouteResourceUrl(parameters, parameters2, ResourceUriType.CurrentPage),"self","GET"));

            // api/touristRoutes
            // 添加和创建旅游路线
            links.Add(new LinkDto(Url.Link("CreateTouristRoute", null),"create_tourist_route","POST"));
            

            return links;
        }

        // 创建 HATEOAS link 链接
        private IEnumerable<LinkDto> CreateLinkForTouristRoute(Guid touristRouteId, string? fields)
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(Url.Link("GetTouristRouteById", new { touristRouteId, fields }), "self", "GET"));

            // 更新
            links.Add(new LinkDto(Url.Link("UpdateTouristRoute", new { touristRouteId }), "update", "PUT"));

            // 局部更新
            links.Add(new LinkDto(Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }), "partially_update", "PATCH"));

            // 删除
            links.Add(new LinkDto(Url.Link("DeleteTouristRoute", new { touristRouteId }), "delete", "DELETE"));

            // 获取旅游路线图片
            links.Add(new LinkDto(Url.Link("GetPictureListForTouristRoute", new { touristRouteId }), "get_pictures", "GET"));

            // 添加新图片
            links.Add(new LinkDto(Url.Link("CreateTouristRoutePicture", new { touristRouteId }), "create_picture", "POST"));

            return links;
        } 
        #endregion

        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Authorize] //便于演示，这里先设为普通用户就能创建路线
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto); //将DTO映射为Model
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);
            var result = touristRouteToReturn.ShapeData(null) as IDictionary<string, object>;
            result.Add("links", links);

            //定义header location，实现RESTFul简单的自我发现功能
            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = result["Id"] },
                result); //第一个参数：Post请求需要返回的API地址  第二个参数：该API路径所需要的参数   第三个参数：本post请求成功后需要输出的数据
        }

        [HttpPut("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute([FromRoute]Guid touristRouteId, [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteFromRepo =   await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            
            // 接下来更新数据的步骤
            // 1. 将touristRouteFromRepo中的数据映射为Dto
            // 2. 更新Dto的数据
            // 3. 把更新后的Dto数据映射回model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo); //使用AutoMapper能很快实现上面的三步骤

             await _touristRouteRepository.SaveAsync();

            return NoContent(); //204。可以根据前端的需求判断是否需要返回数据
        }

        [HttpPatch("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute([FromRoute] Guid touristRouteId, [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }

            var touristRouteFromRepo = await  _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);//ModelState与Dto绑定, 数据的验证规则由Dto的Data Annotation定义

            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState); //如果验证失败则返回错误信息
            }

            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }


        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteByIDs([ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> touristIDs)
        {
            if (touristIDs == null)
            {
                return BadRequest();
            }
            var touristToutesRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristToutesRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}
