using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAppFood.Migrations
{
    /// <inheritdoc />
    public partial class AddDifficultyToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Recipe ADD Difficulty INT NOT NULL DEFAULT 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Recipe DROP COLUMN Difficulty");
        }
    }
}
