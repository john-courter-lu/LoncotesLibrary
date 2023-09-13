using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoncotesLibrary.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CheckoutDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patrons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialName = table.Column<string>(type: "text", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false),
                    OutOfCirculationSince = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Materials_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    PatronId = table.Column<int>(type: "integer", nullable: false),
                    CheckoutDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkouts_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Checkouts_Patrons_PatronId",
                        column: x => x.PatronId,
                        principalTable: "Patrons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialPatron",
                columns: table => new
                {
                    MaterialsId = table.Column<int>(type: "integer", nullable: false),
                    PatronsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialPatron", x => new { x.MaterialsId, x.PatronsId });
                    table.ForeignKey(
                        name: "FK_MaterialPatron_Materials_MaterialsId",
                        column: x => x.MaterialsId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialPatron_Patrons_PatronsId",
                        column: x => x.PatronsId,
                        principalTable: "Patrons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Horror" },
                    { 2, "SciFi" },
                    { 3, "Action" },
                    { 4, "Metal" },
                    { 5, "Thriller" }
                });

            migrationBuilder.InsertData(
                table: "MaterialTypes",
                columns: new[] { "Id", "CheckoutDays", "Name" },
                values: new object[,]
                {
                    { 1, 7, "Book" },
                    { 2, 3, "Movie" },
                    { 3, 4, "CD" }
                });

            migrationBuilder.InsertData(
                table: "Patrons",
                columns: new[] { "Id", "Address", "Email", "FirstName", "IsActive", "LastName" },
                values: new object[,]
                {
                    { 1, "123 Main St", "johndoe@example.com", "John", true, "Doe" },
                    { 2, "456 Elm St", "alicesmith@example.com", "Alice", true, "Smith" },
                    { 3, "789 Oak St", "bobjohnson@example.com", "Bob", false, "Johnson" },
                    { 4, "101 Pine St", "emilywilson@example.com", "Emily", true, "Wilson" },
                    { 5, "222 Cedar St", "michaeldavis@example.com", "Michael", false, "Davis" }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "GenreId", "MaterialName", "MaterialTypeId", "OutOfCirculationSince" },
                values: new object[,]
                {
                    { 1, 1, "1984", 1, null },
                    { 2, 2, "The Lord of the Rings", 1, null },
                    { 3, 3, "The Matrix", 2, null },
                    { 4, 4, "The Shining", 1, new DateTime(1992, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 5, "Pulp Fiction", 2, null },
                    { 6, 1, "The Catcher in the Rye", 1, new DateTime(2002, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 2, "Blade Runner", 2, null },
                    { 8, 2, "War Of The Worlds", 3, null },
                    { 9, 4, "Brave New World", 1, null },
                    { 10, 5, "Star Wars: A New Hope", 2, new DateTime(2007, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, 1, "Master of Puppets", 3, null },
                    { 12, 2, "The Hobbit", 1, null },
                    { 13, 3, "The Terminator", 2, new DateTime(1998, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, 4, "Ride the Lightning", 3, null },
                    { 15, 5, "Jurassic Park", 1, new DateTime(2004, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Checkouts",
                columns: new[] { "Id", "CheckoutDate", "MaterialId", "PatronId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 9, 12, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5703), 1, 1, new DateTime(2023, 9, 19, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5738) },
                    { 2, new DateTime(2023, 9, 9, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5740), 5, 2, new DateTime(2023, 9, 16, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5742) },
                    { 3, new DateTime(2023, 9, 2, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5743), 10, 3, null },
                    { 4, new DateTime(2023, 9, 5, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5746), 8, 4, null },
                    { 5, new DateTime(2023, 9, 7, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5747), 15, 5, new DateTime(2023, 9, 15, 14, 37, 29, 25, DateTimeKind.Local).AddTicks(5748) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_MaterialId",
                table: "Checkouts",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_PatronId",
                table: "Checkouts",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialPatron_PatronsId",
                table: "MaterialPatron",
                column: "PatronsId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_GenreId",
                table: "Materials",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Checkouts");

            migrationBuilder.DropTable(
                name: "MaterialPatron");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Patrons");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "MaterialTypes");
        }
    }
}
