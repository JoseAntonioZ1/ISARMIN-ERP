using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductosYUnidadesMedida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "unidades_medida",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_unidades_medida", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_interno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    codigo_barras = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    marca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    unidad_medida_id = table.Column<Guid>(type: "uuid", nullable: false),
                    costo_referencia = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    precio_venta = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    stock_actual = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    stock_minimo = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_productos", x => x.id);
                    table.CheckConstraint("ck_productos_estado", "estado IN ('Activo','Inactivo')");
                    table.ForeignKey(
                        name: "fk_productos_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_productos_unidades_medida_unidad_medida_id",
                        column: x => x.unidad_medida_id,
                        principalTable: "unidades_medida",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "unidades_medida",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-0000000000d1"), "Unidad" },
                    { new Guid("00000000-0000-0000-0000-0000000000d2"), "Metro" },
                    { new Guid("00000000-0000-0000-0000-0000000000d3"), "Kilogramo" },
                    { new Guid("00000000-0000-0000-0000-0000000000d4"), "Litro" },
                    { new Guid("00000000-0000-0000-0000-0000000000d5"), "Rollo" },
                    { new Guid("00000000-0000-0000-0000-0000000000d6"), "Par" },
                    { new Guid("00000000-0000-0000-0000-0000000000d7"), "Juego" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_productos_categoria_id",
                table: "productos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_productos_codigo_barras",
                table: "productos",
                column: "codigo_barras",
                unique: true,
                filter: "codigo_barras IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_productos_codigo_interno",
                table: "productos",
                column: "codigo_interno",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_productos_unidad_medida_id",
                table: "productos",
                column: "unidad_medida_id");

            migrationBuilder.CreateIndex(
                name: "ix_unidades_medida_nombre",
                table: "unidades_medida",
                column: "nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "unidades_medida");
        }
    }
}
