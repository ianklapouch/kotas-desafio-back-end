using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kotas_desafio_back_end.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Idade = table.Column<sbyte>(type: "INTEGER", nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapturedPokemons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    PokemonMasterId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapturedPokemons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapturedPokemons_PokemonMasters_PokemonMasterId",
                        column: x => x.PokemonMasterId,
                        principalTable: "PokemonMasters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapturedPokemons_PokemonMasterId",
                table: "CapturedPokemons",
                column: "PokemonMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonMasters_Cpf",
                table: "PokemonMasters",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapturedPokemons");

            migrationBuilder.DropTable(
                name: "PokemonMasters");
        }
    }
}
