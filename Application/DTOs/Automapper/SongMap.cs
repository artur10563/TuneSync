using Application.DTOs.Songs;
using AutoMapper;
using Domain.Entities;
using Domain.Primitives;
using static Domain.Primitives.GlobalVariables;

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
                        src => src.Source == SongSource.YouTube && !string.IsNullOrEmpty(src.SourceId)
                            ? GetYoutubeVideo(src.SourceId)
                            : ""
                    )
                )
                .ForMember(dest => dest.AudioPath,
                    opt => opt.MapFrom(
                        src => GetFirebaseMP3Link(src.AudioPath))
                )
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(
                    src => GetYoutubeThumbnail(src.SourceId!))
                )
                //Album is mapped from Youtube playlist title
                .ForMember(dest => dest.Album, opt => opt.MapFrom(
                    src => src.Playlists.Where(pl=> pl.Source == PlaylistSource.YouTube )
                        .Select(pl=>pl.Title)
                        .FirstOrDefault() ?? string.Empty
                    )
                );
        }
    }
}