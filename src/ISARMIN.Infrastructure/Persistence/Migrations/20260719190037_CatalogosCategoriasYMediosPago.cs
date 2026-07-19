using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CatalogosCategoriasYMediosPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    categoria_padre_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categorias", x => x.id);
                    table.ForeignKey(
                        name: "fk_categorias_categorias_categoria_padre_id",
                        column: x => x.categoria_padre_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "medios_pago",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_medios_pago", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "medios_pago",
                columns: new[] { "id", "activo", "nombre" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-0000000000c1"), true, "Efectivo" },
                    { new Guid("00000000-0000-0000-0000-0000000000c2"), true, "Yape" },
                    { new Guid("00000000-0000-0000-0000-0000000000c3"), true, "Plin" },
                    { new Guid("00000000-0000-0000-0000-0000000000c4"), true, "Transferencia bancaria" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_categorias_categoria_padre_id",
                table: "categorias",
                column: "categoria_padre_id");

            migrationBuilder.CreateIndex(
                name: "ix_medios_pago_nombre",
                table: "medios_pago",
                column: "nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "medios_pago");
        }
    }
}
