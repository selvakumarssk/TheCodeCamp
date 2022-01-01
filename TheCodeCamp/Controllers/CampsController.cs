using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps")]
    public class CampsController : ApiController
    {
        public ICampRepository Repository { get; }
        public IMapper Mapper { get; }

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        [Route()]
        public async Task<IHttpActionResult> GET(bool includeTalks = false)
        {
            try
            {
                var result = await Repository.GetAllCampsAsync(includeTalks);

                //Mapping
                var mappedResult = Mapper.Map<IEnumerable<CampModel>>(result);
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                //TODO Add Logging
                return InternalServerError(ex);
            }

        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> GET(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await Repository.GetCampAsync(moniker, includeTalks);

                if (result == null)
                    return NotFound();
                //Mapping
                var mappedResult = Mapper.Map<CampModel>(result);
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                //TODO Add Logging
                return InternalServerError(ex);
            }

        }


        [HttpGet]
        [Route("searchByDate/{eventDate:datetime}")]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks = false)
        {
            try
            {
                var result = await Repository.GetAllCampsByEventDate(eventDate, includeTalks);

                var mappedResilt = Mapper.Map<IEnumerable<CampModel>>(result);
                return Ok(mappedResilt);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}
