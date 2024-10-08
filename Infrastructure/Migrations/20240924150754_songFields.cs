using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class songFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoId",
                table: "Song",
                newName: "SourceId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AudioLength",
                table: "Song",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "AudioSize",
                table: "Song",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioLength",
                table: "Song");

            migrationBuilder.DropColumn(
                name: "AudioSize",
                table: "Song");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "Song",
                newName: "VideoId");
        }
    }
}
