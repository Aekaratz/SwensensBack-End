using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwensenAPI.Models;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwensenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private swensenContext swensenContext = new swensenContext();

        [HttpGet("{keyword}")]
        public async Task<IActionResult> Get(string keyword)
        {
            try
            {
                var result = await swensenContext.Subdistricts.Join(swensenContext.Districts, sub => sub.DistrictId, dis => dis.Id, (sub, dis) => new
                {
                    SubdistrictID = sub.Id,
                    SubdistrictName = sub.NameInThai,
                    DistrictID = dis.Id,
                    DistrictName = dis.NameInThai,
                    ProvinceId = dis.ProvinceId,
                    ZipCode = sub.ZipCode
                }).Join(swensenContext.Provinces,dis => dis.ProvinceId, pro => pro.Id,(dis,pro) =>  new
                {
                    SubdistrictID = dis.SubdistrictID,
                    SubdistrictName = dis.SubdistrictName,
                    DistrictID = dis.DistrictID,
                    DistrictName = dis.DistrictName,
                    ProvinceId = dis.ProvinceId,
                    ProvinceName = pro.NameInThai,
                    ZipCode = dis.ZipCode
                }).Where(a=>a.SubdistrictName.Contains(keyword) || a.DistrictName.Contains(keyword) || a.ProvinceName.Contains(keyword) || a.ZipCode.Contains(keyword)).ToListAsync();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
