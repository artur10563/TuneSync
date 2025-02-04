using Application.DTOs.Albums;
using Application.DTOs.Playlists;
using Application.DTOs.Songs;
using Domain.Entities;

namespace Application.DTOs.Artists;

/// <summary>
/// Artist Info and list of albums + separate songs(later)
/// </summary>
public record ArtistSummaryDTO(
    ArtistInfoDTO ArtistInfo, 
    ICollection<AlbumSummaryDTO> Albums, 
    ICollection<SongDTO> Songs)
{
    public static ArtistSummaryDTO Create(Artist artist, IEnumerable<Song> abandonedSongs, Guid? currentUserGuid)
    {
        currentUserGuid = currentUserGuid ?? Guid.Empty;
        
        return new ArtistSummaryDTO(
            ArtistInfo: ArtistInfoDTO.Create(artist),
            Albums: AlbumSummaryDTO.Create(artist.Albums, currentUserGuid.Value),
            Songs: SongDTO.Create(abandonedSongs, currentUserGuid.Value)
        );
    }
    public static ArtistSummaryDTO Create(Artist artist, IEnumerable<SongDTO> abandonedSongsDTO, Guid? currentUserGuid)
    {
        currentUserGuid = currentUserGuid ?? Guid.Empty;
        
        return new ArtistSummaryDTO(
            ArtistInfo: ArtistInfoDTO.Create(artist),
            Albums: ArtistSummaryDTO.Create(artist.Albums, currentUserGuid.Value),
            Songs: abandonedSongsDTO.ToList()
        );
    }
}