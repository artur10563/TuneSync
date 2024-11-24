using Application.DTOs.Songs;
using AutoMapper;
using Domain.Entities;
using Domain.Primitives;

namespace Application.DTOs.Automapper
{
    internal sealed class SongMap : Profile
    {
        public SongMap()
        {
            CreateMap<Song, SongDTO>()
                .ForMember(
                    dest => dest.SourceUrl,
                    opt => opt.MapFrom(
                        src => src.Source == GlobalVariables.SongSource.YouTube && !string.IsNullOrEmpty(src.SourceId)
                            ? GlobalVariables.YoutubeWatch + src.SourceId
                            : ""
                    )
                )
                .ForMember(dest => dest.AudioPath,
                    opt => opt.MapFrom(
                        src => string.Format(GlobalVariables.GetFirebaseMP3Link(src.AudioPath))
                        )
                    );
        }
    }
}