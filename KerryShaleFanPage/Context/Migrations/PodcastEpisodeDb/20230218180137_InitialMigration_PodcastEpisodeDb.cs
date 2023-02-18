using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace KerryShaleFanPage.Context.Migrations.PodcastEpisodeDb
{
    public partial class InitialMigration_PodcastEpisodeDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PodcastEpisodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    ImageData = table.Column<byte[]>(type: "longblob", nullable: true),
                    ImageDataBase64 = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Duration = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    Checksum = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    FetchedExpectedNextDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CalculatedExpectedNextDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PodcastEpisodes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PodcastEpisodes");
        }
    }
}
