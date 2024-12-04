using Application.DTOs.Artists;
using AutoMapper;
using Domain.Entities;
using Domain.Primitives;

namespace Application.DTOs.Automapper
{
    public sealed class ArtistMap : Profile
    {
        public ArtistMap()
        {
            CreateMap<Artist, ArtistInfoDTO>()
                .ForMember(dest => dest.ChannelUrl, opt => opt.MapFrom(src => GlobalVariables.GetYoutubeChannel(src.YoutubeChannelId)));
        }
    }
}
