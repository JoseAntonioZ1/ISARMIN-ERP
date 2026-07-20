using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Ventas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_comprobante = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    origen = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Directa"),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Registrada"),
                    saldo_pendiente = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    usuario_autorizo_saldo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    motivo_anulacion = table.Column<string>(type: "text", nullable: true),
                    usuario_anulo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_anulacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ventas", x => x.id);
                    table.CheckConstraint("ck_ventas_estado", "estado IN ('Registrada','Emitida','Pagada','Anulada')");
                    table.CheckConstraint("ck_ventas_origen", "origen IN ('Directa','OrdenTrabajo','ServicioCampo')");
                    table.CheckConstraint("ck_ventas_tipo_comprobante", "tipo_comprobante IN ('Cotizacion','Boleta','Factura','NotaVenta','Ticket')");
                    table.ForeignKey(
                        name: "fk_ventas_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ventas_usuarios_usuario_anulo_id",
                        column: x => x.usuario_anulo_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ventas_usuarios_usuario_autorizo_saldo_id",
                        column: x => x.usuario_autorizo_saldo_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ventas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pagos_venta",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    venta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    medio_pago_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pagos_venta", x => x.id);
                    table.CheckConstraint("ck_pagos_venta_monto", "monto > 0");
                    table.ForeignKey(
                        name: "fk_pagos_venta_medios_pago_medio_pago_id",
                        column: x => x.medio_pago_id,
                        principalTable: "medios_pago",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pagos_venta_ventas_venta_id",
                        column: x => x.venta_id,
                        principalTable: "ventas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "venta_detalle",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    venta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    precio_unitario = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_venta_detalle", x => x.id);
                    table.CheckConstraint("ck_venta_detalle_cantidad", "cantidad > 0");
                    table.ForeignKey(
                        name: "fk_venta_detalle_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_venta_detalle_ventas_venta_id",
                        column: x => x.venta_id,
                        principalTable: "ventas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pagos_venta_medio_pago_id",
                table: "pagos_venta",
                column: "medio_pago_id");

            migrationBuilder.CreateIndex(
                name: "ix_pagos_venta_venta_id",
                table: "pagos_venta",
                column: "venta_id");

            migrationBuilder.CreateIndex(
                name: "ix_venta_detalle_producto_id",
                table: "venta_detalle",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "ix_venta_detalle_venta_id",
                table: "venta_detalle",
                column: "venta_id");

            migrationBuilder.CreateIndex(
                name: "ix_ventas_cliente_id",
                table: "ventas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_ventas_usuario_anulo_id",
                table: "ventas",
                column: "usuario_anulo_id");

            migrationBuilder.CreateIndex(
                name: "ix_ventas_usuario_autorizo_saldo_id",
                table: "ventas",
                column: "usuario_autorizo_saldo_id");

            migrationBuilder.CreateIndex(
                name: "ix_ventas_usuario_id",
                table: "ventas",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pagos_venta");

            migrationBuilder.DropTable(
                name: "venta_detalle");

            migrationBuilder.DropTable(
                name: "ventas");
        }
    }
}
