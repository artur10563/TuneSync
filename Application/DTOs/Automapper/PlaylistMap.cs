using Application.DTOs.Playlists;
using AutoMapper;
using Domain.Entities;

namespace Application.DTOs.Automapper
{
    internal sealed class PlaylistMap : Profile
    {
        public PlaylistMap()
        {
            CreateMap<Playlist, PlaylistDTO>()
                .ForMember(dest => dest.CreatedByName,
                opt => opt.MapFrom(
                    src => src.User.Name
                    )
                );

            CreateMap<Playlist, PlaylistSummaryDTO>();

        }
    }
}
