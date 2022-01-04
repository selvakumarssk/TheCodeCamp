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
    [RoutePrefix("api/camps/{moniker}/talks")]
    public class TalksController : ApiController
    {
        public TalksController(ICampRepository repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public ICampRepository Repository { get; }
        public IMapper Mapper { get; }

        [Route()]
        public async Task<IHttpActionResult> Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                var results = await Repository.GetTalksByMonikerAsync(moniker, includeSpeakers);
                var mappedResult = Mapper.Map<IEnumerable<TalkModel>>(results);
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [Route("{id:int}", Name = "GetTalk")]
        public async Task<IHttpActionResult> Get(string moniker, int id, bool includeSpeakers = false)
        {
            try
            {
                var result = await Repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers);
                if (result == null) return NotFound();

                var mappedResult = Mapper.Map<TalkModel>(result);
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route()]
        public async Task<IHttpActionResult> Post(string moniker, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var camp = await Repository.GetCampAsync(moniker);
                    if (camp != null)
                    {
                        var talk = Mapper.Map<Talk>(model);
                        talk.Camp = camp;

                        if (model.Speaker != null)
                        {
                            var speaker = await Repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                            if (speaker != null) talk.Speaker = speaker;
                        }

                        Repository.AddTalk(talk);
                        if (await Repository.SaveChangesAsync())
                        {
                            return CreatedAtRoute("GetTalk", new { moniker = moniker, id = talk.TalkId }, Mapper.Map<TalkModel>(talk));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
            return BadRequest(ModelState);
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Put(string moniker, int talkId, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var talk = await Repository.GetTalkByMonikerAsync(moniker, talkId);
                    if (talk == null) return NotFound();

                    Mapper.Map(model, talk);

                    if (model.Speaker.SpeakerId != talk.Speaker.SpeakerId)
                    {
                        var speaker = await Repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                        if (speaker != null) talk.Speaker = speaker;
                    }

                    if (await Repository.SaveChangesAsync())
                    {
                        return Ok(Mapper.Map<TalkModel>(talk));
                    }
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
            return BadRequest(ModelState);
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Ddelete(string moniker, int talkId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var talk = await Repository.GetTalkByMonikerAsync(moniker, talkId);
                    if (talk == null) return NotFound();

                    Repository.DeleteTalk(talk);
                    if (await Repository.SaveChangesAsync())
                    {
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
            return BadRequest(ModelState);
        }
    }
}
