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

                    b.Property<string>("SourceId")
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

                    b.HasIndex("ArtistGuid");

                    b.HasIndex("CreatedBy");

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

                    b.HasKey("Guid");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("IdentityId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.UserFavoriteSong", b =>
                {
                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SongGuid")
                        .HasColumnType("uuid");

                    b.HasKey("UserGuid", "SongGuid");

                    b.HasIndex("SongGuid");

                    b.HasIndex("UserGuid", "SongGuid")
                        .IsUnique();

                    b.ToTable("UserFavoriteSongs", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Playlist", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Playlists")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.PlaylistSong", b =>
                {
                    b.HasOne("Domain.Entities.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Song", null)
                        .WithMany()
                        .HasForeignKey("SongGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.Song", b =>
                {
                    b.HasOne("Domain.Entities.Artist", "Artist")
                        .WithMany("Songs")
                        .HasForeignKey("ArtistGuid")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Songs")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Artist");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.UserFavoriteSong", b =>
                {
                    b.HasOne("Domain.Entities.Song", null)
                        .WithMany()
                        .HasForeignKey("SongGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.Artist", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("Playlists");

                    b.Navigation("Songs");
                });
#pragma warning restore 612, 618
        }
    }
}
