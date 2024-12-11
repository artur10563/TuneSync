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
                            ? GlobalVariables.GetYoutubeVideo(src.SourceId)
                            : ""
                    )
                )
                .ForMember(dest => dest.AudioPath,
                    opt => opt.MapFrom(
                        src => GlobalVariables.GetFirebaseMP3Link(src.AudioPath))
                    )
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(
                    src => GlobalVariables.GetYoutubeThumbnail(src.SourceId!))
                );
        }
    }
}