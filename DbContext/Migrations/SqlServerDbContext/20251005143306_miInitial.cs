using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.SqlServerDbContext
{
    /// <inheritdoc />
    public partial class miInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "supusr");

            migrationBuilder.CreateTable(
                name: "AttractionAddressesDb",
                schema: "supusr",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    CityPlace = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionAddressesDb", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesDb",
                schema: "supusr",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesDb", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "UsersDb",
                schema: "supusr",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersDb", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AttractionsDb",
                schema: "supusr",
                columns: table => new
                {
                    AttractionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttractionName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    AttractionDescription = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionsDb", x => x.AttractionId);
                    table.ForeignKey(
                        name: "FK_AttractionsDb_AttractionAddressesDb_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "supusr",
                        principalTable: "AttractionAddressesDb",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttractionCategories",
                schema: "supusr",
                columns: table => new
                {
                    AttractionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionCategories", x => new { x.AttractionId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_AttractionCategories_AttractionsDb_AttractionId",
                        column: x => x.AttractionId,
                        principalSchema: "supusr",
                        principalTable: "AttractionsDb",
                        principalColumn: "AttractionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttractionCategories_CategoriesDb_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "supusr",
                        principalTable: "CategoriesDb",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewsDb",
                schema: "supusr",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttractionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewScore = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(200)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewsDb", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_ReviewsDb_AttractionsDb_AttractionId",
                        column: x => x.AttractionId,
                        principalSchema: "supusr",
                        principalTable: "AttractionsDb",
                        principalColumn: "AttractionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewsDb_UsersDb_UserId",
                        column: x => x.UserId,
                        principalSchema: "supusr",
                        principalTable: "UsersDb",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttractionAddressesDb_StreetAddress_ZipCode_CityPlace_Country",
                schema: "supusr",
                table: "AttractionAddressesDb",
                columns: new[] { "StreetAddress", "ZipCode", "CityPlace", "Country" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttractionCategories_CategoryId",
                schema: "supusr",
                table: "AttractionCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionsDb_AddressId",
                schema: "supusr",
                table: "AttractionsDb",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionsDb_AttractionName_AttractionDescription_AddressId",
                schema: "supusr",
                table: "AttractionsDb",
                columns: new[] { "AttractionName", "AttractionDescription", "AddressId" },
                unique: true,
                filter: "[AttractionDescription] IS NOT NULL AND [AddressId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesDb_CategoryName",
                schema: "supusr",
                table: "CategoriesDb",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewsDb_AttractionId_UserId",
                schema: "supusr",
                table: "ReviewsDb",
                columns: new[] { "AttractionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewsDb_ReviewScore_ReviewText",
                schema: "supusr",
                table: "ReviewsDb",
                columns: new[] { "ReviewScore", "ReviewText" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewsDb_UserId",
                schema: "supusr",
                table: "ReviewsDb",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersDb_Email",
                schema: "supusr",
                table: "UsersDb",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersDb_FirstName_LastName",
                schema: "supusr",
                table: "UsersDb",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_UsersDb_LastName_FirstName",
                schema: "supusr",
                table: "UsersDb",
                columns: new[] { "LastName", "FirstName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttractionCategories",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "ReviewsDb",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "CategoriesDb",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "AttractionsDb",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "UsersDb",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "AttractionAddressesDb",
                schema: "supusr");
        }
    }
}
