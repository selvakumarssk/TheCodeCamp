using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCodeCamp.Models;

namespace TheCodeCamp.Data
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(des => des.Venue, opt => opt.MapFrom(src => src.Location.VenueName));
            CreateMap<Talk, TalkModel>();
            CreateMap<Speaker, SpeakerModel>();
        }

    }
}
