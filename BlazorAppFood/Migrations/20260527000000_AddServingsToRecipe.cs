using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAppFood.Migrations
{
    /// <inheritdoc />
    public partial class AddServingsToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Recipe ADD Servings INT NOT NULL DEFAULT 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Recipe DROP COLUMN Servings");
        }
    }
}
