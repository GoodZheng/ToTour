using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToTour.Dtos;
using ToTour.Models;
using ToTour.Services;

namespace ToTour.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController:ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;

        public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository,IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository ??
                throw new ArgumentNullException(nameof(touristRouteRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        //获取指定路线Id的所有图片
        [HttpGet(Name = "GetPictureListForTouristRoute")]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在");
            }
            //存在则需要再进入_touristRouteRepository获取图片的数据
            var picturesFromRepo = await _touristRouteRepository.GetPictureByTouristRouteIdAsync(touristRouteId);
            if (picturesFromRepo == null || picturesFromRepo.Count() < 0)
            {
                return NotFound("图片不存在");
            }
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo));
        }

        //获取指定路线的指定图片
        [HttpGet("{pictureId}",Name = "GetPicture")]
        public async Task<IActionResult> GetPicture(Guid touristRouteId, int pictureId)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线不存在");
            }

            var pictureFromRep = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRep == null)
            {
                return NotFound("图片不存在");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRep));
        }

        // 根据现有路线ID，创建该路线的图片。
        [HttpPost(Name = "CreateTouristRoutePicture")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoutePicture([FromRoute]Guid touristRouteId, [FromBody]TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线不存在");
            }

            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);

            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            await _touristRouteRepository.SaveAsync();

            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtAction("GetPicture",
                                    new{touristRouteId = pictureModel.TouristRouteId,
                                        pictureId = pictureModel.Id},
                                    pictureToReturn);
        }

        [HttpDelete("{pictureId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePicture([FromRoute] Guid touristRouteId, [FromRoute] int pictureId)
        {
            if (!( await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线不存在");
            }

            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRepo is null)
            {
                return NotFound("图片不存在");
            }

            _touristRouteRepository.DeleteTouristRoutePicture(pictureFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}
