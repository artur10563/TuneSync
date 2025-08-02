using Application.CQ.Songs.Query.GetUserFavoriteSongs;
using Application.DTOs.Albums;
using Application.DTOs.Artists;
using Domain.Entities;
using Domain.Helpers;
using Domain.Primitives;
using Infrastructure.Projections;

namespace Tests;

public class SongMappingTest : BaseTest
{
    [Fact]
    public async Task GetUserFavoriteSongsQuery_ShouldReturnUserFavoriteSongs()
    {
        // Arrange: Create test data
        var user = new User("user", RandomString(), RandomString());
        var artist = new Artist("Artist", RandomString(), RandomString());

        
        var song1 = new Song(RandomString(), RandomString(), RandomString(), Guid.NewGuid(), TimeSpan.FromMinutes(3), 1000, user.Guid, artist.Guid);
        var song2 = new Song(RandomString(), RandomString(), RandomString(), Guid.NewGuid(), TimeSpan.FromMinutes(4), 1200, user.Guid, artist.Guid);

        var userFavorite1 = new UserSong
        {
            UserGuid = user.Guid,
            SongGuid = song1.Guid,
            IsFavorite = true
        };

        var userFavorite2 = new UserSong
        {
            UserGuid = user.Guid,
            SongGuid = song2.Guid,
            IsFavorite = false
        };

        // Add songs and favorites to the context
        _uow.UserRepository.Insert(user);
        _uow.ArtistRepository.Insert(artist);
        _uow.SongRepository.Insert(song1);
        _uow.SongRepository.Insert(song2);
        _uow.UserSongRepository.Insert(userFavorite1);
        _uow.UserSongRepository.Insert(userFavorite2);
        await _uow.SaveChangesAsync();


        
        var request = new GetUserFavoriteSongsQuery(user.Guid, 1);
        var result = await _mediator.Send(request);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(x => x.Description)));

        var firstSong = result.Value.First();
        
        Assert.Single(result.Value);
        Assert.Equal(song1.Title, firstSong.Title);
        Assert.Equal(artist.Name, firstSong.Artist.Name);

    }

    [Fact]
    public async Task Album_ShouldReturnDTOs()
    {
        var user = new User(RandomString(), RandomString(), RandomString());
        var artist = new Artist(RandomString(), RandomString(), null);
        var album = new Album(RandomString(), user.Guid, RandomString(), artist.Guid,
            RandomString(), GlobalVariables.PlaylistSource.YouTubeMusic);


        _uow.UserRepository.Insert(user);
        _uow.ArtistRepository.Insert(artist);
        _uow.AlbumRepository.Insert(album);
        _uow.UserFavoriteAlbumRepository.Insert(new UserFavoriteAlbum(user.Guid, album.Guid, true));

        var expectedSongCount = 10;
        for (int i = 0; i < expectedSongCount; i++)
        {
            var song = new Song($"Song {i}", $"source{i}", $"sourceId{i}", Guid.NewGuid(),
                TimeSpan.FromMinutes(3 + i % 5), 1000 + i, createdBy: user.Guid, artistGuid: artist.Guid, albumGuid: album.Guid);
            _uow.SongRepository.Insert(song);
        }

        await _uow.SaveChangesAsync();

        var mockUserGuid = user.Guid;

        var albumSummaryDto = _uow.AlbumRepository.NoTrackingQueryable()
            .Select(_projectionProvider.GetAlbumSummaryProjection(mockUserGuid))
            .Select(x => AlbumSummaryDTO.FromProjection(x))
            .FirstOrDefault();


        Assert.NotNull(albumSummaryDto);
        Assert.Equal(album.Guid, albumSummaryDto.Guid);
        Assert.Equal(album.Title, albumSummaryDto.Title);
        Assert.Equal(
            YoutubeHelper.GetYoutubePlaylistThumbnail(album.ThumbnailId, album.SourceId),
            albumSummaryDto.ThumbnailUrl
        );
        Assert.True(albumSummaryDto.IsFavorite);
        Assert.Equal(artist.Guid, albumSummaryDto.Artist.Guid);
        Assert.Equal(artist.Name, albumSummaryDto.Artist.Name);
        Assert.Equal(artist.DisplayName, albumSummaryDto.Artist.DisplayName);
        Assert.Equal(YoutubeHelper.GetYoutubeChannel(artist.YoutubeChannelId), albumSummaryDto.Artist.ChannelUrl);
        Assert.Equal(artist.ThumbnailUrl, albumSummaryDto.Artist.ThumbnailUrl);
        Assert.Equal(expectedSongCount, albumSummaryDto.SongCount);
        Assert.Equal(album.ExpectedSongs, albumSummaryDto.ExpectedCount);
        Assert.Equal(YoutubeHelper.GetYoutubeAlbumUrl(album.SourceId), albumSummaryDto.SourceUrl);
    }
}