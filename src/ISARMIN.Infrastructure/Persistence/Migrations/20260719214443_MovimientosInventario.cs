using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovimientosInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movimientos_inventario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_movimiento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    origen_tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    origen_id = table.Column<Guid>(type: "uuid", nullable: true),
                    motivo_ajuste = table.Column<string>(type: "text", nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimientos_inventario", x => x.id);
                    table.CheckConstraint("ck_movimientos_inventario_origen_tipo", "origen_tipo IN ('Compra','Venta','OrdenTrabajo','ServicioCampo') OR origen_tipo IS NULL");
                    table.CheckConstraint("ck_movimientos_inventario_tipo_movimiento", "tipo_movimiento IN ('Compra','Venta','ConsumoTaller','ConsumoCampo','Ajuste','Devolucion')");
                    table.ForeignKey(
                        name: "fk_movimientos_inventario_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_inventario_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_inventario_producto_id_fecha",
                table: "movimientos_inventario",
                columns: new[] { "producto_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_inventario_usuario_id",
                table: "movimientos_inventario",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimientos_inventario");
        }
    }
}
