﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Album", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ArtistGuid")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<int>("ExpectedSongs")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<string>("SourceId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ThumbnailId")
                        .HasColumnType("text");

                    b.Property<string>("ThumbnailSource")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Guid");

                    b.HasIndex("ArtistGuid");

                    b.HasIndex("CreatedBy");

                    b.ToTable("Album", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Artist", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ThumbnailUrl")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("YoutubeChannelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Guid");

                    b.HasIndex("YoutubeChannelId")
                        .IsUnique();

                    b.ToTable("Artist", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Playlist", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ThumbnailId")
                        .HasColumnType("text");

                    b.Property<string>("ThumbnailSource")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Guid");

                    b.HasIndex("CreatedBy");

                    b.ToTable("Playlist", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.PlaylistSong", b =>
                {
                    b.Property<Guid>("PlaylistGuid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SongGuid")
                        .HasColumnType("uuid");

                    b.HasKey("PlaylistGuid", "SongGuid");

                    b.HasIndex("SongGuid");

                    b.HasIndex("PlaylistGuid", "SongGuid")
                        .IsUnique();

                    b.ToTable("PlaylistSong", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Song", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AlbumGuid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ArtistGuid")
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("AudioLength")
                        .HasColumnType("interval");

                    b.Property<Guid>("AudioPath")
                        .HasColumnType("uuid");

                    b.Property<int>("AudioSize")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SourceId")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Guid");

                    b.HasIndex("AlbumGuid");

                    b.HasIndex("ArtistGuid");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("SourceId")
                        .IsUnique();

                    b.ToTable("Song", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Role")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Guid");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("IdentityId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.UserFavoriteAlbum", b =>
                {
                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AlbumGuid")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsFavorite")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.HasKey("UserGuid", "AlbumGuid");

                    b.HasIndex("AlbumGuid");

                    b.ToTable("UserFavoriteAlbum", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.UserFavoritePlaylist", b =>
                {
                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PlaylistGuid")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsFavorite")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.HasKey("UserGuid", "PlaylistGuid");

                    b.HasIndex("PlaylistGuid");

                    b.ToTable("UserFavoritePlaylist", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.UserSong", b =>
                {
                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SongGuid")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsFavorite")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsOffline")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.HasKey("UserGuid", "SongGuid");

                    b.HasIndex("SongGuid");

                    b.ToTable("UserSongs", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Album", b =>
                {
                    b.HasOne("Domain.Entities.Artist", "Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistGuid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Albums")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Playlist", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Playlists")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.PlaylistSong", b =>
                {
                    b.HasOne("Domain.Entities.Playlist", "Playlist")
                        .WithMany()
                        .HasForeignKey("PlaylistGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Playlist");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Domain.Entities.Song", b =>
                {
                    b.HasOne("Domain.Entities.Album", "Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumGuid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.Entities.Artist", "Artist")
                        .WithMany("Songs")
                        .HasForeignKey("ArtistGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Songs")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Album");

                    b.Navigation("Artist");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.UserFavoriteAlbum", b =>
                {
                    b.HasOne("Domain.Entities.Album", null)
                        .WithMany("FavoredBy")
                        .HasForeignKey("AlbumGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", null)
                        .WithMany("FavoriteAlbums")
                        .HasForeignKey("UserGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.UserFavoritePlaylist", b =>
                {
                    b.HasOne("Domain.Entities.Playlist", null)
                        .WithMany("FavoredBy")
                        .HasForeignKey("PlaylistGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", null)
                        .WithMany("FavoritePlaylists")
                        .HasForeignKey("UserGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.UserSong", b =>
                {
                    b.HasOne("Domain.Entities.Song", null)
                        .WithMany("FavoredBy")
                        .HasForeignKey("SongGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", null)
                        .WithMany("FavoriteSongs")
                        .HasForeignKey("UserGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.Album", b =>
                {
                    b.Navigation("FavoredBy");

                    b.Navigation("Songs");
                });

            modelBuilder.Entity("Domain.Entities.Artist", b =>
                {
                    b.Navigation("Albums");

                    b.Navigation("Songs");
                });

            modelBuilder.Entity("Domain.Entities.Playlist", b =>
                {
                    b.Navigation("FavoredBy");
                });

            modelBuilder.Entity("Domain.Entities.Song", b =>
                {
                    b.Navigation("FavoredBy");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("Albums");

                    b.Navigation("FavoriteAlbums");

                    b.Navigation("FavoritePlaylists");

                    b.Navigation("FavoriteSongs");

                    b.Navigation("Playlists");

                    b.Navigation("Songs");
                });
#pragma warning restore 612, 618
        }
    }
}
